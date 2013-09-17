module Rules

type MBP = MailboxProcessor<Messages.ForServer>
let mutable players : (Game.Player * MailboxProcessor<Messages.ForClient>) list = []

let broadcast msg =
    for _, mb in players do
        mb.Post(msg)

let comment s =
    broadcast (Messages.Comment s)

let updateGame id =
    let p, cmb = players.[id]
    cmb.Post(Messages.UpdateHand(p.Hand))
    cmb.Post(Messages.UpdateScore([for p, _ in players -> p.Score]))
    cmb.Post(Messages.UpdateGameCards [for p, _ in players -> Array.toList p.InGame])

let updateWorld () =
    for i in 0 .. players.Length - 1 do updateGame i

let processMessage (mb: MBP) = async {
    let! msg = mb.Receive()
    match msg with
        | Messages.Register (name, cmb) ->
            printfn "[gm] -> %s" name
            let idClient = players.Length
            players <- players @ [new Game.Player(name), cmb]
        | Messages.DoAction (owner, sel) ->
            match Messages.Action.Analyse owner sel with
            | Messages.Choose li ->
                let p = fst players.[owner]
                if p.MustChoose = li.Length then
                    for i in List.sort li |> List.rev do p.PlayCard(i)
                updateGame owner
            | _ -> ()
}

let rec getAction (mb: MBP) from = async {
    let! msg = mb.Receive()
    match msg with
      | Messages.DoAction (owner, sel) ->
        if owner = from then return Messages.Action.Analyse owner sel
        else return! getAction mb from
      | _ -> return! getAction mb from
}

let getLetter owner unit =
    char (int 'A' + owner * 2 + unit)

let rec chooseCards mb sendMessage = async {
    let mustPlay, hasPlayed = players |> List.partition (fun (p,_) -> p.MustChoose > 0)
    if not mustPlay.IsEmpty then
        if sendMessage then
            for p, c in mustPlay do
                let n = p.MustChoose
                c.Post(Messages.SetState Messages.State.ChooseCards)
                c.Post(Messages.Comment(sprintf "Choisis %d carte(s) de ta main." n))
        for p, c in hasPlayed do
            let people = mustPlay |> List.map (fun (p, _) -> p.Name) |> String.concat " et "
            c.Post(Messages.Comment("En attente de " + people))
        do! processMessage mb
        do! chooseCards mb false
}

let rec nextPlayer i =
    let n = (i + 1) % players.Length
    if (fst players.[n]).IsDead then nextPlayer n
    else n

let reinit () =
    players <- players |> List.map (fun (p, mb) -> new Game.Player(p.Name), mb)

let rec playTurn mb i = async {
    updateWorld ()
    comment (sprintf "C'est à %s de jouer" (fst players.[i]).Name)
    (snd players.[i]).Post(Messages.SetState Messages.State.YourTurn)
    let! action = getAction mb i
    match action with
      | Messages.Attack(myUnit, ennemy, [hisUnit]) ->
            let p1, p2 = fst players.[i], fst players.[ennemy]
            let u1, u2 = p1.InGame.[myUnit], p2.InGame.[hisUnit]
            let l1, l2 = getLetter i myUnit, getLetter ennemy hisUnit
            let alive1, alive2 = u1.Fight(u2)
            let com = ref ""
            if not alive1 then
                p1.Kill(myUnit); p2.HasKilled(u1)
                com := sprintf "%c[%s] est mort. " l1 (u1.ToString())
            if not alive2 then
                p2.Kill(hisUnit); p1.HasKilled(u2)
                com := sprintf "%s%c[%s] est mort." !com l2 (u2.ToString())
            let shortCom = sprintf "%c attaque %c. %s" l1 l2 !com
            let fullCom = sprintf "%c[%s] attaque %c[%s]. %s" l1 (u1.ToString()) l2 (u2.ToString()) !com
            comment shortCom
            (snd players.[i]).Post(Messages.Comment (fullCom + "\n"))
            (snd players.[ennemy]).Post(Messages.Comment fullCom)
            updateWorld()
            printfn "%s" fullCom
      | _ -> return! playTurn mb i

    if players |> List.filter (fun (p, _) -> not p.IsDead) |> List.length <= 1 then
        let maxScore = players |> List.map (fun (p, _) -> p.Score) |> List.max
        let winners = players |> List.choose (fun (p, _) -> if p.Score = maxScore then Some p.Name else None)
                              |> String.concat " et "
        comment (sprintf "La partie a été remportée par : %s." winners)
        broadcast (Messages.SetState Messages.State.EndGame)
        do! Async.Sleep(5000)
        reinit ()
        updateWorld ()
    else
        do! Async.Sleep(5000)

    do! chooseCards mb true
    do! playTurn mb (nextPlayer i)
  }

let waitForPlayers (mb: MBP) = async {
    let nbPlayers = 3
    while players.Length < nbPlayers do
        do! processMessage mb
        comment (sprintf "En attente - %d/%d joueurs" players.Length nbPlayers)

    let i = ref 0
    for p, cmb in players do
        cmb.Post(Messages.InitGame(!i, [for p, _ in players -> p.Name]))
        incr i

    updateWorld ()
    do! chooseCards mb true
    comment "Début de la partie"
    do! playTurn mb 0
}
