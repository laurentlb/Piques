module Network

open System.Text

// Some nice extension methods
type System.Net.Sockets.NetworkStream with
    member ns.AsyncWriteString (str: string) =
        ns.AsyncWrite(Encoding.ASCII.GetBytes(str), 0, String.length str)

    member ns.AsyncReadString = async {
        let bytes = Array.create 1024 0uy
        let! n = ns.AsyncRead(bytes, 0, 256)
        return Encoding.ASCII.GetString(bytes, 0, n)
    }

    member ns.WriteString (str: string) =
        ns.Write(Encoding.ASCII.GetBytes(str), 0, String.length str)

    member ns.ReadString =
        let bytes = Array.create 1024 0uy
        let n = ns.Read(bytes, 0, 256)
        Encoding.ASCII.GetString(bytes, 0, n)
