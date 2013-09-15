module Network

open System.Net
open System.Net.Sockets
open System.Text

// Some nice extension methods
type System.Net.Sockets.NetworkStream with
    member ns.AsyncWriteString (str: string) =
        ns.AsyncWrite(Encoding.UTF8.GetBytes(str), 0, String.length str)

    member ns.WriteString (str: string) =
        ns.Write(Encoding.UTF8.GetBytes(str), 0, String.length str)

    member ns.AsyncReadString = async {
        let bytes = Array.create 1024 0uy
        let! n = ns.AsyncRead(bytes, 0, 256)
        return Encoding.UTF8.GetString(bytes, 0, n)
    }

// Generic & asynchronous server function
let rec establishServer f port =
    let server = TcpListener(IPAddress.Any, port)
    server.Start()
    while true do
        let client = server.AcceptTcpClient()
        printfn "New client"
        f (client.GetStream()) |> Async.StartImmediate
