# Manuel Utilisateur - EasySave

## Introduction

EasySave est une application permettant d'effectuer des sauvegardes de fichiers et de dossiers de manière simple et efficace. 
Ce manuel vous guidera dans l'utilisation de de la version 3.0 de l'application, de l'installation à la gestion de vos sauvegardes.
Cette version 3.0 donne une toute nouvelle interface à EasySave facilitant toutes les actions et donnant une vision en temps réel de l'avancement des sauvegardes.

## Installation

### Prérequis

- Windows 10 ou supérieur
- .NET 8.0 ou version compatible
- Espace disque suffisant pour stocker les sauvegardes

### Étapes d'installation

1. Clonez le repos GitHub
2. Exécutez le programme en lançant la commande 'dotnet run' depuis `<RACINE_DU_REPO>\EasySave\Easysave`

## Utilisation

### Lancement de l'application

Lancez `EasySaveGraphics3.0.exe` depuis votre répertoire d'installation. 
La fenêtre du menu de l'application est remplie

![EasySave](https://github.com/user-attachments/assets/01adca13-f68a-422e-adb2-9a0f226ab5db)




### Utilisation de l'application et Gestion des sauvegardes

![EasySaveInts](https://github.com/user-attachments/assets/6db420c7-c8e9-4d97-a4b1-3a56a9cad9f1)


1. Liste des travaux de sauvegardes
2. Exemple de travailde sauvegarde
3. Créer un travail de sauvegarde
4. Mettre à jour la liste des travaux de sauvegarde
5. Lancer / Mettre en pause une sauvegarde
6. Stopper la sauvegarde en cours
7. Supprimer le travail de sauvegarde
8. Ouvrir les paramètres
9. Chiffrer / Déchiffrer des sauvegardes


### Création d'un travail de sauvegarde

![EasySave3](https://github.com/user-attachments/assets/014ed68d-3f28-4a0a-93d1-297007a8b80b)

1. Nom du travail de sauvegarde
2. Chemin de la source
3. Chemin de l'endroit de la sauvegarde
4. Type de sauvegarde

En enregistrant, une premiere sauvegarde totale se lance afin d'enregitrer les fichiers et qui servira de base de comparaison pour les sauvegardes de type Diffetentiel


### Modifications des paramètres de l'application

![EasySave2](https://github.com/user-attachments/assets/c1fcdccf-b435-4129-b2f5-4e403a1db61a)

1. Différentes informations à remplir/ modifier
       Name = Nom de l'application 
       Clé de chiffrement = clé générée qui permet de chiffrer et déchiffrer les sauvegardes ( attention, impossible de déchiffrer sans cette clé )
       Liste des extentions à chiffrer = extentions des fichiers qu'il faut chiffrer. Tous les fichiers : "*" | Fichiers textes et pdf : .txt .pdf
       
3. Changer la langue
4. Ouvrir le fichier de parametres dans l'editeur de texte par defaut de l'ordinateur
5. Sauvegarder / Quitter

## Utilisation de la commande à Distance

La commande à distance permet de gerer les sauvegardes depuis une machine distante via une communication Sockets.

### Prérequis
- Deux machines connectées sur me même réseau
- Autorisations eventuelles de l'application

### Apperçus

Interface cliente IHM distante et serveur associé en ligne de commande.
![EasySave6](https://github.com/user-attachments/assets/2983b3bf-dfbb-4fa3-8271-cb24690689bf)

Une fois la connexion établie, le client peut désormais communiquer avec le serveur.
Il est donc possible à distance de créer un travail de sauvegarde, de lancer des sauvegardes et de supprimer un travail de sauvegarde.
![EasySave5](https://github.com/user-attachments/assets/af84096a-d56c-4977-a58a-3dde370ad550)



## Gestion des fichiers journaux et états

- Les fichiers journaux (logs) et états sont stockés dans :
  - `[CheminUtilisateur]\AppData\Roaming\EasySave\Logs` pour les fichiers logs.
  - `[CheminUtilisateur]\AppData\Roaming\EasySave\state.json` pour l'état des sauvegardes.
  - `[CheminUtilisateur]\AppData\Roaming\EasySave\settings.json` pour les paramètres de l'application.

    
## Support et Assistance

- Consultez la documentation fournie.
- Vérifiez les fichiers logs en cas de problème.
- Contactez l'équipe de support via le site officiel.

## Conclusion

EasySave simplifie la gestion des sauvegardes en automatisant le processus et en offrant une interface intuitive. Grâce à ses options avancées, il répond aux besoins des utilisateurs souhaitant protéger leurs données éfficacement.
