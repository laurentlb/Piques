module AI

open Control.CommonExtensions
open System

type MBP = MailboxProcessor<Messages.ForServer>
let mutable myId = -1
let me = Game.Player("stupid AI")
let random = new Random()
let mutable allCards : Game.Card list list = []


let playCards (master: MBP) =
    master.Post(Messages.DoAction(myId, [-1, 0]))
    master.Post(Messages.DoAction(myId, [-1, 0; -1, 1]))
//    if me.MustChoose > 0 then
//        master.Post(Messages.DoAction(myId, [for i in 0 .. me.MustChoose - 1 -> -1, i]))
//        for i in 1 .. me.MustChoose do me.PlayCard 0

let rec attack (master: MBP) =
    let ennemy = random.Next(allCards.Length)
    if ennemy <> myId && allCards.[ennemy].Length > 0 then
        let n = random.Next(allCards.[ennemy].Length)
        let mine = random.Next(allCards.[myId].Length)
        master.Post(Messages.DoAction(myId, [myId, mine; ennemy, n]))
    else
        attack master

let play (master: MBP) (inbox: MailboxProcessor<Messages.ForClient>) = async {
    while true do
        let! msg = inbox.Receive()
        match msg with
        | Messages.InitGame (id, p) -> myId <- id
        | Messages.UpdateGameCards li ->
            allCards <- li
        | Messages.UpdateHand li -> me.Hand <- li
        | Messages.YourTurn ->
            attack master
            playCards master
            // playCards master
        | _ -> ()
}
