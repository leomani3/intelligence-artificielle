J'ai mis dans le dossier : Tout le projet unity qui est constitu� de deux sc�nes, les .exe de ces deux sc�nes dans /builds/GameScene.exe et /builds/MainScene.exe. les vid�os se trouvent � la racine du projet.

Le projet est constitu� de deux sc�nes :

-MainScene : la sc�ne de d�monstration des algorithmes. L'exploration et le chemin est affich�
	clic gauche : placement du point de d�part
	clic molette : placement du point d'arriv�e
	clic droite : placement d'un mur (il est a noter qu'il faut que les murs soient c�tes � c�tes (pas de coins qui se touche. Comme en 4 voisinnage)
	suppr : r�initialisation de la grille (efface le point de d�part, le point d'arriv�e et les murs)
	scroll : zoom In / Out
	Enter : Dijkstra
	backspace : A*
	/!\ il faut placer le d�part et l'arriv�e pour pouvoir lancer les algos

-GameScene : la sc�ne du jeu

description : Le but du joueur est de ramasser un maximum de bonus avant que "l'enemy" (repr�senter par le carr� rouge) ne le rattrape. Pour se faire il peut se d�placer sur la grille gr�ce aux fleches directionnelles. L'enemy se d�place de plus en plus vite au fur que la partie avance. Chaque rammassage de bonus ralentis l'enemi.

	clic droit : placement de mur
	fl�ches directionnelles : d�placement
	Enter : lancement du jeu.
	