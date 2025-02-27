# Manuel Utilisateur - EasySave

## Introduction

EasySave est une application permettant d'effectuer des sauvegardes de fichiers et de dossiers de manière simple et efficace. Ce manuel vous guidera dans l'utilisation de l'application, de l'installation à la gestion de vos sauvegardes.

## Installation

### Prérequis

- Windows 10 ou supérieur
- .NET 8.0 ou version compatible
- Espace disque suffisant pour stocker les sauvegardes

### Étapes d'installation


1. Cloner le repos GitHub
2. Exécutez le programme en lançant la commande 'dotnet run' depuis `<RACINE_DU_REPO>\EasySave\Easysave`

## Utilisation

### Lancement de l'application

Lancez `EasySave.exe` depuis votre répertoire d'installation. Une interface Graphique s'affichera avec le menu principal :
IMAGE INTERAFCE GRAPHQIEU



### Création d'une sauvegarde

1. Sur l'interface graphqiue, selectionez "Créer un travail de sauvegarde 
2. Remplissez les champs d'informations demandées :
   - **Nom du travail de sauvegarde** : entrez un nom pour identifier la sauvegarde.
   - **Répertoire source** : indiquez le chemin absolu du dossier ou des fichiers à sauvegarder.
   - **Répertoire cible** : indiquez le chemin absolu où la sauvegarde sera stockée.
   - **Type de sauvegarde** :
        -Sauvegarde complète :  Enregistre tout les fichiers à chaques sauvegarde.
        -Sauveragde : Enregistrer uniquement les modifications entre 2 sauvegardes.

Exemple de création :
![image](https://github.com/user-attachments/assets/3ea5fe67-38b4-4e3c-888e-6bd112cb92f3)


### Mise a jour d'une sauvegarde
p
1. Sélectionnez **2 - Mettre à jour un ou plusieurs travaux de sauvegarde**.
2. Une liste des sauvegardes disponibles s'affiche
3. Sélectionnez la sauvergade que vous voulez actualiser.
4. Appuyer sur le bouton en bas a gauche pour lancer la sauvegarde

![image](https://github.com/user-attachments/assets/3ea54050-5e03-44fe-9348-bc06d94f0014)


### Consultation des sauvegardes existantes

1. Sélectionnez **Voir la liste des travaux de sauvegardes**.
2. Une liste des sauvegardes existantes s'affiche avec leurs détails, par exemple :

![image](https://github.com/user-attachments/assets/c3ce5024-7ff2-405f-aefb-c81265430b8e)



### Suppression d'une sauvegarde

1. Sélectionnez **Supprimer un travail de sauvegarde**.
2. Une liste des sauvegardes s'affiche, par exemple :

![image](https://github.com/user-attachments/assets/cb2d9a09-cc1b-4543-9e77-dd3feb4c7bad)




3. Sélectionnez la sauveagrde a suprimer
4. Confirmez la suppression en cliquant sur le bouton en bas à gauche

### Modification de la langue
 
1. Sélectionnez **Changer la langue** dans le menu principal.
2. Choisissez une langue parmi celles disponibles dans la liste affichée.

## Chiffrement et déchiffrement
1. Pour chiffre et déchiffrer il vous faux utilser le bouton Chiffrer / Déchiffrer
2. Entrez ensuite le repertoire qui contient les fichier chiffrer, cela peut etre le fichier de sauvegarde.
3. Ajouter aussi la clef de chiffrement
4. Apputer sur le bouton en bas a gauche pour valider le chiffrement


## Parametrage
Pour parametrer l'application, il vous faut choisir l'option Paramètre du menu Principal
   Exemple de parametre par defaut :
   ![image](https://github.com/user-attachments/assets/f51d3885-ec96-46ec-9503-df313c9b5761)

Voici la liste des parametres que vous pouvez entrez : 
   -Nom : Le Nom de l'applicatio, la ou sera stocker les fichier de l'app dans le dossier AppData/Roaming
   -Clé de Chiffrement : C'est la clef qui permet d'encrypter et de décrypter les fichier, nous vous conesillons de la changer et de sauvegarder ou elle ce trouve.
   -Si vous soihété encrypter uniquement certains type de fichier, vous pouvez entrez leurs extension ici. L'inidcateur * correspondra a l'encryptage de tout les fichiers.
   -Application Métier : Ce champ permet de définir les application qui seront arrèté les sauvegarde si elles sont ouvertes.
   -Ouvrir les fichier de paramètre : fonction de parametrage avancé.
   -Format des Fichier de LOG : Cette section permet de changer le format des LOG, soit Json, soit XML.

## Gestion des fichiers journaux et états

- Les fichiers journaux (logs) et états sont stockés dans :
  - `[CheminUtilisateur]\AppData\Roaming\EasySave\Logs` pour les fichiers logs.
  - `[CheminUtilisateur]\AppData\Roaming\EasySave\state.json` pour l'état des sauvegardes.

## Support et Assistance

- Consultez la documentation fournie.
- Vérifiez les fichiers logs en cas de problème.
- Contactez l'équipe de support via le site officiel.

## Conclusion

EasySave simplifie la gestion des sauvegardes en automatisant le processus et en offrant une interface intuitive. Grâce à ses options avancées, il répond aux besoins des utilisateurs souhaitant protéger leurs données efficacement.
