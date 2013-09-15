module Messages

open Microsoft.FSharp.Control
open Game

type ForClient =
  | InitGame of int * string list // id * names
  | UpdateHand of Card list
  | UpdateScore of int list
  | UpdateGameCards of Card list list
  | ShowFighting of int list
  | ShowCard of int * Card
  | Comment of string

with
  member m.Type =
    match m with
    | InitGame _ -> "InitGame"
    | UpdateHand _ -> "UpdateHand"
    | UpdateScore _ -> "UpdateScore"
    | UpdateGameCards _ -> "UpdateGameCards"
    | ShowFighting _ -> "ShowFighting"
    | ShowCard _ -> "ShowCard"
    | Comment _ -> "Comment"

  override m.ToString() =
    let args =
      match m with
        | InitGame (id, li) -> string id + "|" + String.concat "|" li
        | UpdateHand li -> [for c in li -> string c.Value] |> String.concat "|"
        | UpdateScore li -> [for i in li -> string i] |> String.concat "|"
        | UpdateGameCards li ->
                [for pli in li ->
                    [for c in pli -> string c.Value] |> String.concat ","
                ] |> String.concat "|"
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
      | "UpdateScore" -> UpdateScore [for i in args -> int i]
      | "UpdateGameCards" ->
            let tuples = [for str in args -> [for c in str.Split([|','|]) -> Card (int c)]]
            UpdateGameCards tuples
      | "ShowFighting" -> ShowFighting [for i in args -> int i]
      | "ShowCard" -> ShowCard (int args.[0], Card (int args.[1]))
      | "Comment" -> Comment args.[0]
      | _ -> failwithf "Invalid network message: %s" s

type Action =
    | Attack of int * int * int list // unit1 -> ennemy - units
    | TeamAttack of int * int * (int * int) // unit1+unit2 -> unit3
    | Swap of int * int
    | Choose of int list
with
  static member Analyse owner sel =
    match Action.TryAnalyse owner sel with None -> failwith "bad action" | Some a -> a

  static member TryAnalyse owner sel =
    let fromHand, fromGame = List.partition (function -1, _ -> true | _, _ -> false) sel
    let mine, ennemy = List.partition (fun (pid, cid) -> pid = owner) fromGame
    let fromHand, mine = List.map snd fromHand, List.map snd mine
    match fromHand.Length, mine.Length, ennemy.Length with
      | 0, 0, 0 -> None
      | x, 0, 0 -> Choose(fromHand) |> Some
      | 1, 1, 0 -> Swap(fromHand.[0], mine.[0]) |> Some
      | 0, 2, 1 -> TeamAttack(mine.[0], mine.[1], ennemy.[0]) |> Some
      | 0, 1, (1|2) ->
        if ennemy.Length = 1 || fst ennemy.[0] = fst ennemy.[1] then // ennemies must be of the same team
            Attack(mine.[0], fst ennemy.[0], List.map snd ennemy) |> Some
        else None
      | _ -> None

type ForServer =
  | Register of string * MailboxProcessor<ForClient>
  | DoAction of int * (int * int) list
with
  override m.ToString() =
    match m with
    | DoAction (owner, li) ->
        let li = [for x, y in li -> string x + "," + string y]
        sprintf "Action|%d|%s\n" owner (String.concat "|" li)
    | Register _ -> failwith "not implemented"

  static member Parse (s: string) =
      let data = s.Split([|'|'|])
      let args = Array.toList data.[1..]
      match data.[0] with
      | "Action" ->
        let tuples = [for sub in data.[2..] -> let sub = sub.Split([|','|]) |> Array.map int in sub.[0], sub.[1]]
        DoAction(int data.[1], tuples)
      | _ -> failwithf "Invalid network message: %s" s
