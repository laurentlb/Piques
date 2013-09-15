module Server

open Control.CommonExtensions
open System.Net
open System.Net.Sockets

open Network


let mutable players = ["Rubix"; "Nicuvëo"] |> List.map (fun s -> new Game.Player(s))

// Handle a client
let rec server (stream: NetworkStream) = async {
    let! clientName = stream.AsyncReadString
    let p = new Game.Player(clientName)
    let idClient = players.Length
    players <- players @ [p]
    let msg = Messages.InitGame(idClient, [for p in players -> p.Name])
    //let res = "Hello " + clientName
    do printfn "Hello %s" clientName
    do! stream.AsyncWriteString (msg.ToString())
    do! server stream
}

let serverLoop stream = async {
    try
        while true do
            do! server stream
    with _ -> do printfn "Connection closed"
}

establishServer server 3000