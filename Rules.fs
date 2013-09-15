module Rules

let mutable players = [] // "Rubix"; "Nicuvëo"] |> List.map (fun s -> new Game.Player(s))
let mutable clients = []

type MBP = MailboxProcessor<Messages.ForServer>

let processMessage (mb: MBP) = async {
    let! msg = mb.Receive()
    match msg with
        | Messages.Register (name, cmb) ->
            printfn "[gm] -> %s" name
            let idClient = players.Length
            players <- players @ [new Game.Player(name)]
            clients <- clients @ [cmb]
            ()
}

let broadcast msg =
    for mb in clients do
        mb.Post(msg)

let comment s =
    broadcast (Messages.Comment s)

let playTurn mb i = async {
    do! processMessage mb
}

let waitForPlayers (mb: MBP) = async {
    let nbPlayers = 3
    while players.Length < nbPlayers do
        do! processMessage mb
        comment (sprintf "En attente - %d/%d joueurs" players.Length nbPlayers)

    for i, mb in List.mapi (fun i mb -> i, mb) clients do
        Messages.InitGame(i, [for p in players -> p.Name]) |> broadcast
    comment "Début de la partie"
    do! playTurn mb 0
}
