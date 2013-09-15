module Server

open Control.CommonExtensions
open Microsoft.FSharp.Control
open System
open System.Net
open System.Net.Sockets

open Network

let master = new MailboxProcessor<Messages.ForServer>(Rules.waitForPlayers)
master.Start()


let clientMailbox (stream: NetworkStream) (inbox: MailboxProcessor<Messages.ForClient>) = async {
    while true do
        let! msg = inbox.Receive()
        printfn "send: %s" (msg.ToString())
        do! stream.AsyncWriteString(msg.ToString())
}

let mutable names =
    ["Albrecht"; "Reimund"; "Reiner"; "Wilfried"; "Wolfram"; "Ludolf"; "Ludwig"; "Lothar"; "Heinrich"; "Dieter"; "Friedrich"; "Fritz"]
let random = new Random()

let checkName s =
    if s <> "?" then s
    else
        let name = names.[random.Next(names.Length)]
        names <- List.filter ((<>) name) names
        name

let clientLoop (stream: NetworkStream) = async {
    try
        let! name = stream.AsyncReadString
        let name = checkName name
        let mb = new MailboxProcessor<Messages.ForClient>(clientMailbox stream)
        mb.Start()
        master.Post(Messages.ForServer.Register(name, mb))
        while true do
            let! msg = stream.AsyncReadString
            master.Post(Messages.ForServer.Parse(msg))
    with _ -> do printfn "Connection closed"
}

let startAI () =
    let name = checkName "?"
    let mb = new MailboxProcessor<Messages.ForClient>(AI.play master)
    mb.Start()
    master.Post(Messages.ForServer.Register(name, mb))

startAI ()
establishServer clientLoop 3000