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
                for i in li do (fst players.[owner]).PlayCard(i)
                updateGame owner
            | _ -> failwith "blah"
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

let rec chooseCards mb = async {
    let mustPlay, hasPlayed = players |> List.partition (fun (p,_) -> p.MustChoose > 0)
    if not mustPlay.IsEmpty then
        for p, c in mustPlay do
            let n = p.MustChoose
            c.Post(Messages.Comment(sprintf "Choisis %d cartes de ta main." n))
        for p, c in hasPlayed do
            let people = mustPlay |> List.map (fun (p, _) -> p.Name) |> String.concat " et "
            c.Post(Messages.Comment("En attente de " + people))
        do! processMessage mb
        do! chooseCards mb
}

let rec playTurn mb i = async {
    updateWorld ()
    comment (sprintf "C'est à %s de jouer" (fst players.[i]).Name)
    let! action = getAction mb i
    match action with
      | Messages.Attack(myUnit, ennemy, [hisUnit]) ->
            let p1, p2 = fst players.[i], fst players.[ennemy]
            let u1, u2 = p1.InGame.[myUnit], p2.InGame.[hisUnit]
            let l1, l2 = getLetter i myUnit, getLetter ennemy hisUnit
            comment (sprintf "%c[%s] attaque %c[%s]" l1 (u1.ToString()) l2 (u2.ToString()))
            let alive1, alive2 = u1.Fight(u2)
            if not alive1 then p1.Kill(myUnit)
            if not alive2 then p2.Kill(hisUnit)
            updateWorld()
      | _ -> ()
    
    do! Async.Sleep(5000)
    do! chooseCards mb
    do! playTurn mb ((i + 1) % players.Length)
  }

let waitForPlayers (mb: MBP) = async {
    let nbPlayers = 2
    while players.Length < nbPlayers do
        do! processMessage mb
        comment "En attente..."
        comment "Waiting"
        comment (sprintf "En attente - %d/%d joueurs" players.Length nbPlayers)

//    for i, mb in List.mapi (fun i mb -> i, mb) clients do
    let i = ref 0
    for p, cmb in players do
        cmb.Post(Messages.InitGame(!i, [for p, _ in players -> p.Name]))
        updateGame !i
        incr i

    do! chooseCards mb
    comment "Début de la partie"
    do! playTurn mb 0
}
