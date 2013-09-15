module Server

open Control.CommonExtensions
open System.Net
open System.Net.Sockets

open Network

// Generic & asynchronous server function
let rec establishServer f port =
    let server = TcpListener(IPAddress.Any, port)
    server.Start()
    while true do
        let client = server.AcceptTcpClient()
        printfn "New client"
        f (client.GetStream()) |> Async.StartImmediate

// Handle a client
let rec server (stream: NetworkStream) =
  async { try
            let! resp = stream.AsyncReadString
            let res = "Hello " + resp
            do! stream.AsyncWriteString res
            do! server stream
          with _ -> do printfn "Connection closed" }

establishServer server 3000