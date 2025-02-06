
# Documentation du projet EasySave

## Introduction

EasySave est une application de sauvegarde permettant aux utilisateurs de copier des fichiers et des dossiers en toute simplicité. Ce projet est développé en C# et s'exécute sous l'environnement .NET, avec une architecture modulaire pour faciliter son extensibilité et sa maintenance.

## Architecture du projet

EasySave est organisé en plusieurs modules distincts pour assurer une séparation des responsabilités claire :

### Modules principaux

- **EasySave** : Module principal contenant l'interface utilisateur et la logique métier.
- **Logger** : Module dédié à la gestion des logs et des événements.

### Détails des dossiers

#### EasySave

- **Utils** : Contient des outils et utilitaires comme :

  - `JobsState` : Gère l'état des travaux de sauvegarde.
  - `StateJsonReader.cs` : Lecture et traitement des fichiers JSON d'état.
  - `Localization` et `ConsoleManager` : Gestion des messages et de l'interface utilisateur en ligne de commande.
  - `Messages.cs` et `Utils.cs` : Fonctions d'assistance diverses.

- **ConsoleManager.cs** : Gère l'interaction utilisateur via une interface en ligne de commande. Ses principales fonctionnalités incluent :

  - **Menus dynamiques** :
    - Menu principal pour créer, mettre à jour, lire, ou supprimer des tâches de sauvegarde.
    - Menu de sélection de langue avec prise en charge multilingue.
  - **Gestion des tâches** :
    - `CreateSaveJobMenu` : Permet à l'utilisateur de créer une nouvelle tâche de sauvegarde en choisissant le nom, le chemin source, le chemin cible, et le type (complète ou différentielle).
    - `UpdateSaveJobMenu` : Permet de mettre à jour des tâches de sauvegarde existantes.
    - `DeleteSaveJobMenu` : Supprime une tâche de sauvegarde et ses fichiers associés.
    - `ReadSaveJobsMenu` : Affiche les tâches de sauvegarde disponibles avec leurs détails.
  - **Support multilingue** :
    - Utilise la classe `Messages` pour gérer les traductions des messages en fonction de la langue choisie.
    - Méthode `LanguageSelectionMenu` pour basculer entre les langues disponibles.
  - **Gestion des erreurs** : Fournit des messages clairs en cas d'entrée invalide ou d'erreurs système.

- **DifferentialSave.cs** : Implémentation des sauvegardes différentielles.

- **FullSave.cs** : Implémentation des sauvegardes complètes.

- **SaveJob.cs** : Gère les paramètres et l'exécution des tâches de sauvegarde, avec les méthodes principales décrites ci-dessous :

  - **Attributs** :
    - `Name`, `SourcePath`, `TargetPath`, `CreationDate`, `LastUpdate`, `State`.
  - **Méthodes clés** :
    - `CreateSave()` : Crée une sauvegarde complète par défaut en initialisant le répertoire cible et en appelant `FullSave`.
    - `FullSave(sourcePath, targetPath)` : Effectue une sauvegarde complète de manière récursive, copiant tous les fichiers et sous-dossiers.
    - `CreateFullSave(sourcePath, saveTargetPath)` : Copie les fichiers et dossiers avec récursivité tout en gérant les sous-répertoires.
    - `DeleteSave()` : Supprime un travail de sauvegarde ainsi que ses fichiers associés.

- **Fichiers JSON** :

  - `state.json` : Fichier principal stockant l'état actuel des sauvegardes.
  - `state.copy.json` : Copie de sauvegarde pour garantir l'intégrité des données.

#### Logger

- **Logger.cs** : Responsable de l'écriture des logs dans des fichiers ou des destinations spécifiques.

## Fonctionnalités principales

- Sauvegarde de fichiers et de dossiers :
  - **Complète** : Copie entière du contenu sélectionné.
  - **Différentielle** : Copie uniquement les fichiers modifiés depuis la dernière sauvegarde.
- Gestion des logs pour assurer la traçabilité des opérations.
- Support des configurations personnalisées à l'aide de fichiers JSON.
- Interface utilisateur intuitive en ligne de commande.

## Installation et exécution

### Prérequis

- Visual Studio 2022 ou supérieur.
- .NET 6.0 ou version compatible.

### Étapes d'installation

1. Cloner le dépôt du projet :
   ```bash
   git clone https://github.com/votre-repo/EasySave.git
   ```
2. Ouvrir `EasySave.sln` avec Visual Studio.
3. Construire la solution (`Ctrl + Shift + B`).
4. Exécuter le projet (`F5`).

## Développement et Contribution

### Structure du Code

Le projet suit une architecture modulaire avec les fichiers organisés de la manière suivante :

```
EasySave/
├── EasySave.csproj  # Projet principal
├── Utils/           # Outils et utilitaires
│   ├── JobsState.cs
│   ├── StateJsonReader.cs
│   ├── Localization/
│   ├── ConsoleManager.cs
│   ├── Messages.cs
│   └── Utils.cs
├── DifferentialSave.cs
├── FullSave.cs
├── SaveJob.cs
├── state.json
├── state.copy.json
├── Logger/
│   ├── Logger.csproj
│   └── Logger.cs
```

### Ajout de nouvelles fonctionnalités

1. Créer une branche dédiée :
   ```bash
   git checkout -b feature/nom-fonctionnalite
   ```
2. Développer la fonctionnalité en respectant la structure du projet.
3. Tester et valider les modifications.
4. Soumettre une pull request.

## Support et Maintenance

En cas de problème ou de bug, les utilisateurs peuvent :

- Consulter la documentation.
- Vérifier les logs pour identifier les erreurs.
- Signaler un problème via le gestionnaire d'issues du dépôt GitHub.

## Conclusion

EasySave est une solution flexible et efficace pour la sauvegarde de fichiers. Grâce à son architecture modulaire, il est facile de l'étendre et d'ajouter de nouvelles fonctionnalités selon les besoins des utilisateurs.
