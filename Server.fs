module Server

open Control.CommonExtensions
open Microsoft.FSharp.Control
open System.Net
open System.Net.Sockets

open Network

//let gameMaster = Rules.waitForPlayers

//let gameMaster (inbox: MailboxProcessor<Messages.ForServer>) = async {
//    while true do
//        let! msg = inbox.Receive()
//        match msg with
//          | Messages.Register (name, mb) ->
//                printfn "[gm] -> %s" name
//                let p = new Game.Player(name)
//                let idClient = players.Length
//                players <- players @ [p] // TODO: mailbox
//                let msg = Messages.InitGame(idClient, [for p in players -> p.Name])
//                mb.Post(msg)
//          | _ -> printfn "[gm] ?! %s" (msg.ToString())
//}

let master = new MailboxProcessor<Messages.ForServer>(Rules.waitForPlayers)
master.Start()


let clientMailbox (stream: NetworkStream) (inbox: MailboxProcessor<Messages.ForClient>) = async {
    while true do
        let! msg = inbox.Receive()
        printfn "send: %s" (msg.ToString())
        do! stream.AsyncWriteString(msg.ToString())
}

let clientLoop (stream: NetworkStream) = async {
    try
        let! clientName = stream.AsyncReadString
        let mb = new MailboxProcessor<Messages.ForClient>(clientMailbox stream)
        mb.Start()
        master.Post(Messages.ForServer.Register(clientName, mb))
        while true do
            let! msg = stream.AsyncReadString
            master.Post(Messages.ForServer.Parse(msg))
    with _ -> do printfn "Connection closed"
}

establishServer clientLoop 3000