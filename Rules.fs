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

let rec chooseCards mb = async {
    let mustPlay, hasPlayed = players |> List.partition (fun (p,_) -> p.MustChoose)
    if not mustPlay.IsEmpty then
        for p, c in mustPlay do
            c.Post(Messages.Comment("Sélectionne cartes de ta main à jouer."))
        for p, c in hasPlayed do
            let people = mustPlay |> List.map (fun (p, _) -> p.Name) |> String.concat " et "
            c.Post(Messages.Comment("En attente de " + people))
        do! processMessage mb
        do! chooseCards mb
}

let rec playTurn mb i = async {
    comment (sprintf "C'est à %s de jouer" (fst players.[i]).Name)
    // do! processMessage mb
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
