# User Manual – EasySave

## Introduction

EasySave is a backup application that allows users to copy files and folders easily. This manual will guide you through using the application—from installation to managing your backups.

## Installation

### Prerequisites

- Windows 10 or later
- .NET 8.0 or a compatible version
- Sufficient disk space to store backups

### Installation Steps

1. Clone the GitHub repository.
2. Run the program by executing the command `dotnet run` from `<REPO_ROOT>\EasySave\Easysave`.

## Usage

### Launching the Application

Launch `EasySave.exe` from your installation directory. A command-line interface will appear with the main menu:
![Main Menu](Doc/1.png)

### Creating a Backup

1. Select **1 - Create a Backup Task** from the main menu.
2. Fill in the requested information:
   - **Backup Task Name**: Enter a name to identify the backup.
   - **Source Directory**: Provide the absolute path of the folder or files to back up.
   - **Target Directory**: Provide the absolute path where the backup will be stored.
   - **Backup Type**:
     - `0` for a full backup (copies all files).
     - `1` for a differential backup (copies only modified files).

Example interface:
![Backup Task](Doc/2.png)

### Executing a Backup

1. Select **2 - Update One or More Backup Tasks**.
2. A list of available backups will be displayed, for example:
![Update](Doc/3.png)
3. Enter the numbers corresponding to the backups to execute.
4. The application will perform the copy operation based on the defined type.

### Viewing Existing Backups

1. Select **3 - View the List of Backup Tasks**.
2. A list of existing backups with their details will be displayed, for example:
![List](Doc/4.png)

### Deleting a Backup

1. Select **4 - Delete a Backup Task**.
2. A list of backups will be displayed, for example:
![Delete](Doc/5.png)
3. Enter the number corresponding to the backup to delete.
4. Confirm the deletion.

### Changing the Language

1. Select **5 - Change Language** from the main menu.
2. Choose a language from the displayed list.

## Log Files and State Management

- **Storage Locations:**
  - Log files are stored at: `[UserPath]\AppData\Roaming\EasySave\Logs`
  - The backup state is stored in: `[UserPath]\AppData\Roaming\EasySave\state.json`
- **Log File Format:**
  - Users can choose the log file format (XML or JSON) directly from the console settings.

## Support and Assistance

- Refer to the provided documentation.
- Check the log files if issues occur.
- Contact the support team via the official website.

## Conclusion

EasySave simplifies backup management by automating the process and offering an intuitive interface. With its advanced options, it meets the needs of users looking to protect their data efficiently.
# Manuel Utilisateur – EasySave

## Introduction

EasySave est une application de sauvegarde permettant aux utilisateurs de copier des fichiers et des dossiers de manière simple et efficace. Ce manuel vous guidera dans l'utilisation de l'application, depuis l'installation jusqu'à la gestion de vos sauvegardes.

## Installation

### Prérequis

- Windows 10 ou supérieur
- .NET 8.0 ou version compatible
- Espace disque suffisant pour stocker les sauvegardes

### Étapes d'installation

1. Cloner le dépôt GitHub.
2. Exécutez le programme en lançant la commande `dotnet run` depuis `<RACINE_DU_REPO>\EasySave\Easysave`.

## Utilisation

### Lancement de l'application

Lancez `EasySave.exe` depuis votre répertoire d'installation. Une interface en ligne de commande s'affichera avec le menu principal :
![Menu principal](Doc/1.png)

### Création d'une sauvegarde

1. Sélectionnez **1 - Créer un travail de sauvegarde** dans le menu principal.
2. Remplissez les informations demandées :
   - **Nom du travail de sauvegarde** : entrez un nom pour identifier la sauvegarde.
   - **Répertoire source** : indiquez le chemin absolu du dossier ou des fichiers à sauvegarder.
   - **Répertoire cible** : indiquez le chemin absolu où la sauvegarde sera stockée.
   - **Type de sauvegarde** :
     - `0` pour une sauvegarde complète (copie tous les fichiers).
     - `1` pour une sauvegarde différentielle (copie uniquement les fichiers modifiés).

Exemple d'interface :
![sauvegarde](Doc/2.png)

### Exécution d'une sauvegarde

1. Sélectionnez **2 - Mettre à jour un ou plusieurs travaux de sauvegarde**.
2. Une liste des sauvegardes disponibles s'affiche, par exemple :
![Maj](Doc/3.png)
3. Entrez les numéros correspondant aux sauvegardes à exécuter.
4. L'application effectue les copies selon le type défini.

### Consultation des sauvegardes existantes

1. Sélectionnez **3 - Voir la liste des travaux de sauvegarde**.
2. Une liste des sauvegardes existantes s'affiche avec leurs détails, par exemple :
![List](Doc/4.png)

### Suppression d'une sauvegarde

1. Sélectionnez **4 - Supprimer un travail de sauvegarde**.
2. Une liste des sauvegardes s'affiche, par exemple :
![delete](Doc/5.png)
3. Entrez le numéro correspondant à la sauvegarde à supprimer.
4. Confirmez la suppression.

### Modification de la langue

1. Sélectionnez **5 - Changer la langue** dans le menu principal.
2. Choisissez une langue parmi celles disponibles dans la liste affichée.

## Gestion des fichiers journaux et états

- Les fichiers journaux (logs) et l'état des sauvegardes sont stockés dans :
  - `[CheminUtilisateur]\AppData\Roaming\EasySave\Logs` pour les fichiers logs.
  - `[CheminUtilisateur]\AppData\Roaming\EasySave\state.json` pour l'état des sauvegardes.
- **Configuration du format des logs :**
  - L'utilisateur peut choisir le format du fichier log (XML ou JSON) directement depuis les paramètres de la console.

## Support et Assistance

- Consultez la documentation fournie.
- Vérifiez les fichiers logs en cas de problème.
- Contactez l'équipe de support via le site officiel.

## Conclusion

EasySave simplifie la gestion des sauvegardes en automatisant le processus et en offrant une interface intuitive. Grâce à ses options avancées, il répond aux besoins des utilisateurs souhaitant protéger efficacement leurs données.
