module Messages

type MessageForClient =
  | InitGame of string list
  | UpdateHand of int list
  | ShowFighting of int list
  | ShowCard of int * int  // pos * value

