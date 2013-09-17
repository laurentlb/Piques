# ﻿Batailles et piques
Strategy card game for 3 to 6 players.

## You will need
An army has 16 numbered cards. 13 units from 1 to 13, the King (considered a 16) and two mines.
Units 1 & 2 are spies.
Units 4 & 8 are minesweepers.

(for a test version, we recommend a Tarot with two Trumps for the mines)


## Setup
Each player takes an army, shuffles the cards and draws 6 cards.
Each time you play a card, you keep drawing until you have 6 cards in hand (unless your deck is empty).

Each player chooses two cards from her hand and puts them in front of her, face down, then draws two cards.
Each player also places the special cards in front of him, face up.

## Turn

Each player chooses one of the cards she laid down (unless it's a mine) and attacks either one or two cards from another player.
Only the players involved in the fight can look at the cards: the other only know which cards fight, live and die, but not the face value.

### Two-units fights
When two units fight:
 * if a minewseeper faces a mine, the minesweeper wins
 * if any other unit faces a mine, the mine kills it
 * if a King and a Spy fight, the Spy always wins
 * all other cards are easy: the strongest lives, the weakers dies
 * in case of a tie, both units die

### Three-units fights
 * first, the minesweepers defuse the mines.
   If there's no minesweeper attacking the mine, all attackers die.

 * Each unit deals damages to the opposing side, in any way the player wishes.
   A King can never harm a Spy.


Examples:
* 5 + 7 vs 10 : 10 dies and kills either 5 or 7
* 5 + 7 vs 12 : everybody dies
* 3 + 5 vs 10 : 3 and 5 die
* Spy + 5 vs King : King dies (Spy) and 5 dies (King is stronger, but cannot kill a Spy)

When a unit dies, it's shown to all players and put in the attacking player's Trophy pile.
Its former owner must then replace it with a card from her hand, then draw a card (unless her deck is empty).

Each time a player kills a King, he can have a look at any other card in play (laid down in front of a player, not in their hands or deck).


## Special cards

Special cards are laid in front of each player. When a special card is used, it's discarded.

* **Teamwork** (two per player): you can attack one eney unit with both units in play.
* **Slacker** (two per player): don't attack this turn and exchange a laid down card with one from your hand.


## Victory

When only one player can attack (others are dead or only have mines), she can still play to destroy landmines.
The game stops when she cannot play or when she wants to stop.

Each player gains a point for every killed unit, but two for each King.
The player with the most points wins.


## Special cases

Having two mines in play is forbidden.
If a player don't have any other card to lay down, she stops to play until the end of the game.
Other players can still attack her mines if they wish.

---

# Comments

We want to test a game with partial information. If a unit never attacks, I can think it's a mine and send minesweepers.
If I lose a fight against a King, I can remember where he is and send a Spy later.
I can also wait for someone to do it, so I know the attacker is a Spy which is easy to take down.
Every time people fight, I can get information (such unit killed a 10), everytime I lose one, I get even more intel (this one is a 12).

One can anticipate the enemy strikes: when losing against my King, they know what it is and may have replaced the dying card with a Spy, so I should probably attack it in the meantime.

We will most probably have to adjust cards' values and types, and find a somewhat more attractive theme.
From the theme, we'll be able to add a few fun rules and polish the game some more.

