module Messages

open Microsoft.FSharp.Control
open Game

type ForClient =
  | InitGame of int * string list // id * names
  | UpdateHand of Card list
  | ShowFighting of int list
  | ShowCard of int * Card
  | Comment of string

with
  member m.Type =
    match m with
    | InitGame _ -> "InitGame"
    | UpdateHand _ -> "UpdateHand"
    | ShowFighting _ -> "ShowFighting"
    | ShowCard _ -> "ShowCard"
    | Comment _ -> "Comment"

  override m.ToString() =
    let args =
      match m with
        | InitGame (id, li) -> string id + "|" + String.concat "|" li
        | UpdateHand li -> [for c in li -> string c.Value] |> String.concat "|"
        | ShowFighting li -> li |> List.map string |> String.concat "|"
        | ShowCard (p, c) -> string p + "|" + string c.Value
        | Comment s -> s
    sprintf "%s|%s\n" m.Type args

  static member Parse (s: string) =
      let data = s.Split([|'|'|])
      let args = Array.toList data.[1..]
      match data.[0] with
      | "InitGame" -> InitGame(int data.[1], Array.toList data.[2..])
      | "UpdateHand" -> UpdateHand [for i in args -> Card (int i)]
      | "ShowFighting" -> ShowFighting [for i in args -> int i]
      | "ShowCard" -> ShowCard (int args.[0], Card (int args.[1]))
      | "Comment" -> Comment args.[0]
      | _ -> failwithf "Invalid network message: %s" s

type ForServer =
  | Register of string * MailboxProcessor<ForClient>
  | Action of int * (int * int) list
with
  override m.ToString() =
    match m with
    | Action (owner, li) ->
        let li = [for x, y in li -> string x + "," + string y]
        sprintf "Action|%d|%s\n" owner (String.concat "|" li)
    | Register _ -> failwith "not implemented"
  
  static member Parse (s: string) =
      let data = s.Split([|'|'|])
      let args = Array.toList data.[1..]
      match data.[0] with
      | "Action" ->
        let tuples = [for sub in data.[2..] -> let sub = sub.Split([|','|]) |> Array.map int in sub.[0], sub.[1]]
        Action(int data.[1], tuples)
      | _ -> failwithf "Invalid network message: %s" s
