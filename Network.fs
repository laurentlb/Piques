module Network

open System.Net
open System.Net.Sockets
open System.Text

// Some nice extension methods
type System.Net.Sockets.NetworkStream with
    member ns.AsyncWriteString (str: string) =
        let bytes = Encoding.UTF8.GetBytes(str)
        ns.AsyncWrite(bytes, 0, bytes.Length)

    member ns.WriteString (str: string) =
        ns.Write(Encoding.UTF8.GetBytes(str), 0, String.length str)

    member ns.AsyncReadString = async {
        let bytes = Array.create 10240 0uy
        let! n = ns.AsyncRead(bytes, 0, 10240)
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

// nombre de cartes restantes
// todo: spy
// todo: 2 vs 1
// todo: 1 vs 2
// swap
// interdire les attaques à la mine / par le vide
// faire exploser la double mine :)
// nombre de cartes dans la pioche
