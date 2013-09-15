module Game

open System

type Card = Card of int
with
  static member King = Card 16
  static member Mine = Card 0
  member c.Value = match c with Card v -> v
  member c.IsEmpty = c.Value = -1
  member c.IsSpy = c.Value = 1 || c.Value = 2
  member c.IsMineSweeper = c.Value = 4 || c.Value = 8

  override c.ToString() = match c with
    | _ when c = Card.King -> "16 (roi)"
    | _ when c = Card.Mine -> "mine"
    | _ when c.IsSpy -> string c.Value + "(esp)"
    | _ when c.IsMineSweeper -> string c.Value + " (dém)"
    | _ -> string c.Value


let allCards = [0; 0; 16] @ [1 .. 13] |> List.map Card

let KnuthShuffle (lst : array<'a>) =
    let Swap i j =
        let item = lst.[i]
        lst.[i] <- lst.[j]
        lst.[j] <- item
    let rnd = new Random()
    let ln = lst.Length
    [0..(ln - 2)]
    |> Seq.iter (fun i -> Swap i (rnd.Next(i, ln)))
    lst

let generateHand () =
    allCards |> List.toArray |> KnuthShuffle |> Array.toList
