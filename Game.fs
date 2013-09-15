module Game

open System

type Card = Card of int
with
  static member King = Card 16
  static member Mine = Card 0
  static member Empty = Card -1
  member c.Value = match c with Card v -> v
  member c.IsSpy = c.Value = 1 || c.Value = 2
  member c.IsMineSweeper = c.Value = 4 || c.Value = 8

  override c.ToString() =
    match c with
    | _ when c = Card.King -> "16 (roi)"
    | _ when c = Card.Mine -> "mine"
    | _ when c = Card.Empty -> "[vide]"
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
    allCards |> List.toArray |> KnuthShuffle

let removeNth n =
    Array.mapi (fun i x -> i, x) >> Array.filter (fst >> (<>) n) >> Array.map snd

type Player(name: string) =
    let mutable hand = generateHand()
    let mutable inGame = [|Card.Empty; Card.Empty|]
    let mutable swaps = 2
    let mutable score = 0

    member p.Name = name
    member p.Hand
      with get() = hand |> Seq.take 6 |> Seq.toList
      and  set li = hand <- List.toArray li
    member p.InGame = inGame
    member p.Swaps = swaps

    member p.Swap (pig, ph) =
        assert (swaps > 0)
        swaps <- swaps - 1
        let tmp = inGame.[pig]
        inGame.[pig] <- hand.[ph]
        hand.[ph] <- tmp
    member p.PlayCard pig ph =
        assert (inGame.[pig] <> Card.Empty)
        inGame.[pig] <- hand.[ph]
        hand <- removeNth ph hand

    member p.Dead =
        inGame = [|Card.Empty; Card.Empty|] && hand.Length = 0

    member p.MustChoose =
        if Array.forall ((<>) Card.Empty) inGame then
            false
        else
            hand.Length > 0