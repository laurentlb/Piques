module Messages

open Game

type MessageForClient =
  | InitGame of int * string list // id * names
  | UpdateHand of Card list
  | ShowFighting of int list
  | ShowCard of int * Card

with
  member m.Type =
    match m with
    | InitGame _ -> "InitGame"
    | UpdateHand _ -> "UpdateHand"
    | ShowFighting _ -> "ShowFighting"
    | ShowCard _ -> "ShowCard"

  override m.ToString() =
    m.Type + "|" +
    match m with
    | InitGame (id, li) -> string id + "|" + String.concat "|" li
    | UpdateHand li -> [for c in li -> string c.Value] |> String.concat "|"
    | ShowFighting li -> li |> List.map string |> String.concat "|"
    | ShowCard (p, c) -> string p + "|" + string c.Value

  static member Parse (s: string) =
      let data = s.Split([|'|'|])
      let args = Array.toList data.[1..]
      match data.[0] with
      | "InitGame" -> InitGame(int data.[1], Array.toList data.[2..])
      | "UpdateHand" -> UpdateHand [for i in args -> Card (int i)]
      | "ShowFighting" -> ShowFighting [for i in args -> int i]
      | "ShowCard" -> ShowCard (int args.[0], Card (int args.[1]))
      | _ -> failwithf "Invalid network message: %s" s
