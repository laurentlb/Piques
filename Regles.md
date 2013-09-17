# ﻿Batailles et piques
Jeu pour 3 à 6 joueurs.

## Matériel
Une armée est composée de 16 cartes. Il y a 13 unités numérotées de 1 à 13, le roi (avec la valeur 16) et deux champs de mines.

Les unités 1 et 2 sont des espions.
Les unités 4 et 8 sont des démineurs.

(version de test : utiliser un jeu de tarot et prendre deux atouts pour les mines)


## Préparation

Chaque joueur prend une armée et mélange les cartes. Il pioche ensuite 6 cartes. À chaque fois que le joueur joue une carte, il devra en piocher une afin de garder 6 cartes en main (si la pioche est vide, il pourra avoir moins de cartes).

Chaque joueur choisit deux cartes de sa main et les pose devant lui, faces cachées. Il pioche alors deux cartes.

Chaque joueur place aussi devant lui les cartes spéciales (tous les joueurs ont les mêmes cartes).

## Tour de jeu

Chaque joueur, à tour de rôle, choisit une carte devant lui (sauf si c’est un champ de mines) et attaque soit une carte, soit les deux cartes d’un joueur adverse. Seuls les deux joueurs qui s’affrontent peuvent regarder les cartes en jeu. Les autres joueurs savent seulement quelles cartes se battent, mais pas leur valeur.

### Combat entre deux unités
Lors d’un combat entre deux unités :
* Si un démineur attaque un champ de mines, il gagne
* Si un roi et un espion s’affrontent, l’espion gagne toujours
* Si une autre unité attaque un champ de mines, elle perd
* Sinon, l’unité de plus forte valeur gagne.

En cas d’égalité, les deux unités meurent.


### Combat à trois unités
* Résolution de la mine : si des démineurs attaquent, le champ de mines est détruit sans causer de dégâts. S’il n’y a pas de démineur, tous les attaquants sont détruits avant même de pouvoir attaquer.
* Chaque unité inflige ses blessures aux adversaires, de la façon qu’elle souhaite. Un roi ne peut jamais infliger de dégâts à un espion.

Exemples :
* 4 et 6 contre 8 : 8 meurt et tue au choix 4 ou 6.
* 4 et 6 contre 10 : tout le monde meurt
* 4 et 6 contre 12 : 4 et 6 meurent
* 1 et 5 contre roi : le roi et le 5 meurent (le roi n’a jamais le droit de tuer l’espion)

Quand une unité est morte, elle est montrée à tous les joueurs et mise dans le cimetière du joueur qui l’a tuée. Son ancien propriétaire doit alors la remplacer avec une carte de sa main puis piocher (si sa pioche n’est pas vide). À chaque fois qu’un joueur tue un roi, il peut regarder une carte en jeu de n’importe quel adversaire.


## Cartes spéciales

Les cartes spéciales sont placées devant chaque joueur. Quand une carte est utilisée, elle est défaussée.

* Travail d’équipe (deux exemplaires). Le joueur peut faire faire une attaque avec deux unités. Dans ce cas, il n’est possible d’attaquer qu’une seule unité adverse.
* Tire-au-flanc (deux exemplaires). Au lieu de faire une attaque, un joueur peut échanger une de ses deux cartes devant lui avec une carte de sa main.


## Victoire

Quand une seule personne peut jouer (les autres sont morts ou n’ont que des champs de mines), elle peut continuer à jouer pour détruire les champs de mines. La partie se termine quand elle le souhaite ou qu’elle ne peut plus jouer. Chaque joueur marque un point par unité adverse qu’il a tuée, le roi comptant double. Celui qui a le plus de points remporte la partie.


## Cas particuliers
* Il est interdit d’avoir ses deux champs de mine en jeu. Si un joueur n’a pas d’autre carte à poser, il arrête de jouer jusqu’à la fin de la partie (les autres joueurs peuvent attaquer son champ de mines s’il le souhaitent).


---

# Commentaires


L'idée de base est de jouer avec les informations partielles. Si une unité ne se déplace jamais, je peux imaginer que c'est une mine et envoyer le démineur. Si je perds un combat contre un roi, je peux retenir où il est placé et l'attaquer avec mon espion plus tard. Ou alors, j'attends que quelqu'un le fasse - du coup, je devine que la carte est un espion et je peux le tuer facilement. Bref, dès que les autres se battent, j'obtiens des informations (ex : "telle unité a tué
un 10"). Quand je perds un combat, j'obtiens encore plus d'info ("telle carte est un 12").


On peut aussi anticiper les coups des adversaires. A m'attaque, perd et voit que j'ai un roi. Comme son unité est morte, il la remplace. Il est possible que ce soit un espion qui prévoie de m'attaquer au prochain tour, je peux donc l'attaquer entre temps.


Bref, jeu à tester.
Bien sûr, il faudra sûrement ajuster le nombre et le type de cartes. On peut trouver un thème moins banal que des militaires (des choux, des toons, des grenouilles...). À partir du thème, on peut ensuite ajouter quelques règles et rendre le jeu moins brut.
