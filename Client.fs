module Client

open System.Windows.Forms
open System.Drawing

let form = new Form(Text = "Batailles et piques", Width = 700, Height = 400)
let topText = new Label(Text = "Non connecté", Left = 50, Top = 10, Width = 500)

let players = ["LLB"; "Nicuvëo"; "Rubix"; "Sly"]

let updateDisplay () =
    form.Controls.Clear()
    form.Controls.Add(topText)

    // Action buttons
    form.Controls.Add(new Label(Text = "Actions", Top = 50))
    let button = new Button(Text = "Attaquer", Left = 100, Top = 50)
    button.MouseClick.Add(fun _ -> System.Windows.Forms.MessageBox.Show("test") |> ignore)
    // button.BackColor <- Color.Red;
    form.Controls.Add(button)

    let button = new Button(Text = "Tir-au-flanc", Left = 200, Top = 50)
    button.MouseClick.Add(fun _ -> System.Windows.Forms.MessageBox.Show("test") |> ignore)
    form.Controls.Add(button)

    // Display hand
    for i in 0 .. 5 do
        form.Controls.Add(new Label(Text = "Ta main", Top = 100))
        let button = new Button(Text = string(i), Left = 100 + i * 100, Top = 100)
        button.MouseClick.Add(fun _ -> System.Windows.Forms.MessageBox.Show(string(i)) |> ignore)
        form.Controls.Add(button)

    // Player buttons
    for i in 0 .. List.length players - 1 do
        let name = players.[i]
        let top = 150 + i * 50
        form.Controls.Add(new Label(Text = name, Top = top))

        let button1 = new Button(Text = "foo", Left = 100, Top = top)
        button1.MouseClick.Add(fun _ -> System.Windows.Forms.MessageBox.Show("test") |> ignore)
        form.Controls.Add(button1)
        let button2 = new Button(Text = "foo", Left = 200, Top = top)
        button2.MouseClick.Add(fun _ -> System.Windows.Forms.MessageBox.Show("test2") |> ignore)
        form.Controls.Add(button2)

let updateText message =
    topText.Text <- message

updateDisplay()
updateText "Toujours pas connecté"

open System.Net
open System.Net.Sockets
open System.Text

let tcp = new TcpClient()
tcp.Connect("localhost", 3000)
let text = "LLB"
tcp.GetStream().Write(Encoding.ASCII.GetBytes(text), 0, text.Length)

let bytes = Array.create 256 0uy
let n = tcp.GetStream().Read(bytes, 0, 256)
updateText (Encoding.ASCII.GetString(bytes, 0, n))

Application.Run(form)