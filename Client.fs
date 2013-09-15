module Client

open Control.CommonExtensions
open System
open System.Drawing
open System.Net.Sockets
open System.Threading
open System.Windows.Forms
open System.Collections.Generic

open Game
open Network

let form = new Form(Text = "Batailles et piques", Width = 700, Height = 500)
let topText = new Label(Text = "Non connecté", Left = 50, Top = 10, AutoSize = true)
let textBox = new TextBox(Height = 400, Width = 700, Top = 350, Multiline = true)

let mutable players : Player list = []
let mutable myId = -1
let selectedButtons = new HashSet<int * int>()

let tcp = new TcpClient()
tcp.Connect("localhost", 3000)


let mailbox = new MailboxProcessor<Messages.ForServer>(fun inbox ->
    async {
        while true do
            let! msg = inbox.Receive()
            do! tcp.GetStream().AsyncWriteString(msg.ToString())
    })
mailbox.Start()

let select (bt: Button) key =
    if selectedButtons.Contains(key) then
        bt.Font <- new Font(bt.Font, FontStyle.Regular)
        selectedButtons.Remove(key) |> ignore
    else
        bt.Font <- new Font(bt.Font, FontStyle.Bold)
        selectedButtons.Add(key) |> ignore

let sendOrder _ =
    let sel = Seq.toList selectedButtons
    match Messages.Action.TryAnalyse myId sel with
    | Some msg -> mailbox.Post(Messages.DoAction(myId, sel))
    | _ -> MessageBox.Show("Action invalide") |> ignore

let updateDisplay () =
    form.Controls.Clear()
    selectedButtons.Clear()
    form.Controls.Add(topText)
    let mutable top = 50

    if myId >= 0 then
        // Action button
        let button = new Button(Text = "Action", Left = 100, Top = top)
        button.MouseClick.Add(sendOrder)
        form.Controls.Add(button)
        top <- top + 50

        // Display hand
        let player = players.[myId]
        let hand = player.Hand
        for i in 0 .. hand.Length - 1 do
            form.Controls.Add(new Label(Text = "Ta main", Top = top))
            let button = new Button(Text = hand.[i].ToString(), Left = 100 + i * 100, Top = top)
            button.MouseClick.Add(fun x -> select button (-1, i))
            form.Controls.Add(button)
        top <- top + 50

    // Player buttons
    let mutable cardId = 0
    for pid in 0 .. players.Length - 1 do
        let p = players.[pid]
        let name = if pid = myId then p.Name + "*" else p.Name
        let name = name + " " + string p.Score
        form.Controls.Add(new Label(Text = name, Top = top))

        for i in 0 .. p.InGame.Length - 1 do
            let text =
                if pid = myId || p.InGame.[i] = Card.Empty then p.InGame.[i].ToString()
                else char (cardId + int 'A') |> string
            let button = new Button(Text = text, Left = (i + 1) * 100, Top = top)
            button.MouseClick.Add(fun x -> select button (pid, i))
            form.Controls.Add(button)
            cardId <- cardId + 1
        top <- top + 50
    // form.Controls.Add(textBox)


let processMessage = function
    | Messages.Comment s -> topText.Text <- s
    | Messages.InitGame (id, names) ->
        myId <- id
        players <- [for i in names -> new Player(i)]
        updateDisplay()
    | Messages.UpdateHand li ->
        players.[myId].Hand <- li
    | Messages.UpdateScore li ->
        for p, score in List.zip players li do p.Score <- score
    | Messages.UpdateGameCards li ->
        for pi, cli in List.mapi (fun i c -> i, c) li do
            players.[pi].InGame <- List.toArray cli
        updateDisplay()
    | Messages.YourTurn -> ()
    | x -> topText.Text <- "?? " + x.ToString() 

let rec doNetwork = async {
    let text = "?"
    do! tcp.GetStream().AsyncWriteString(text)
    while true do
        let! msg = tcp.GetStream().AsyncReadString
        textBox.AppendText(msg)
        for sub in msg.Split([|'\n'|], StringSplitOptions.RemoveEmptyEntries) do
            processMessage (Messages.ForClient.Parse sub)
}

updateDisplay()
Async.StartImmediate doNetwork
Application.Run(form)