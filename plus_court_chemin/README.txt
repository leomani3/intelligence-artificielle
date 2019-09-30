J'ai mis dans le dossier : Tout le projet unity qui est constitué de deux scènes, les .exe de ces deux scènes dans /builds/GameScene.exe et /builds/MainScene.exe. les vidéos se trouvent à la racine du projet.

Le projet est constitué de deux scènes :

-MainScene : la scène de démonstration des algorithmes. L'exploration et le chemin est affiché
	clic gauche : placement du point de départ
	clic molette : placement du point d'arrivée
	clic droite : placement d'un mur (il est a noter qu'il faut que les murs soient côtes à côtes (pas de coins qui se touche. Comme en 4 voisinnage)
	suppr : réinitialisation de la grille (efface le point de départ, le point d'arrivée et les murs)
	scroll : zoom In / Out
	Enter : Dijkstra
	backspace : A*
	/!\ il faut placer le départ et l'arrivée pour pouvoir lancer les algos

-GameScene : la scène du jeu

description : Le but du joueur est de ramasser un maximum de bonus avant que "l'enemy" (représenter par le carré rouge) ne le rattrape. Pour se faire il peut se déplacer sur la grille grâce aux fleches directionnelles. L'enemy se déplace de plus en plus vite au fur que la partie avance. Chaque rammassage de bonus ralentis l'enemi.

	clic droit : placement de mur
	flèches directionnelles : déplacement
	Enter : lancement du jeu.
	