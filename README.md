# English Documentation for the EasySave Project

## Introduction

**EasySave** is a backup application that allows users to copy files and folders easily. This project is developed in **C#** and runs in the **.NET** environment, featuring a modular architecture to enhance extensibility and maintainability.

---

## Project Architecture

EasySave is organized into several distinct modules to ensure clear separation of responsibilities.

### Main Modules

- **EasySave**: The main module containing the user interface and business logic.
- **Logger**: A dedicated module for managing logs and events.

---

## Folder Details

### **EasySave**

#### **Utils**
Contains tools and utilities such as:
- **JobsState**: Manages the state of backup jobs.
- **StateJsonReader.cs**: Reads and processes state JSON files.
- **ConsoleManager**: Handles messages and the command-line user interface.
- **Messages.cs and MessagesReader.cs**: Manages multilingual support.

#### **ConsoleManager.cs**
Manages user interaction through a command-line interface.  
Its main functionalities include:

##### **Dynamic Menus**
- **Main Menu**: Create, update, read, or delete backup tasks.
- **Language Selection Menu**: Multilingual support.

##### **Task Management**
- **CreateSaveJobMenu**: Creates a new backup task with a name, source path, target path, and type (full or differential).
- **UpdateSaveJobMenu**: Updates existing backup tasks.
- **DeleteSaveJobMenu**: Deletes a backup task and its associated files.
- **ReadSaveJobsMenu**: Displays available backup tasks with their details.

##### **Multilingual Support**
- Uses the **Messages** class to manage message translations based on the selected language.
- **LanguageSelectionMenu** method allows switching between available languages.

##### **Error Handling**
- Provides clear messages for invalid input or system errors.

#### **Backup Management**
- **DifferentialSave.cs**: Implements differential backups.
- **FullSave.cs**: Implements full backups.
- **SaveJob.cs**: Manages the parameters and execution of backup tasks.

##### **Attributes**
- `Name`, `SourcePath`, `TargetPath`, `CreationDate`, `LastUpdate`, `State`.

##### **Key Methods**
- **CreateSave()**: Creates a default full backup.
- **FullSave(sourcePath, targetPath)**: Performs a recursive full backup.
- **CreateFullSave(sourcePath, saveTargetPath)**: Copies files and folders while managing subdirectories.
- **DeleteSave()**: Deletes a backup task and its associated files.

#### **JSON Files**
- **state.json**: The main file storing the current state of backups.  
  📂 **Location**: `C:\Users\[Username]\AppData\Roaming\EasySave`.

---

### **Logger**
- **Logger.cs**: Responsible for writing logs to files or specific destinations.  
  📂 **Log File Location**: `C:\Users\[Username]\AppData\Roaming\EasySave\Logs`.

---

## Key Features

- **File and Folder Backup**:
  - **Full**: Copies the entire selected content.
  - **Differential**: Copies only the files modified since the last backup.
- **Log Management**: Ensures operation traceability.
- **Intuitive User Interface**: Command-line based.

---

## Installation and Execution

### **Prerequisites**
- **Visual Studio 2022** or later.
- **.NET 8.0** or compatible version.


# Documentation Française du projet EasySave

## Introduction

**EasySave** est une application de sauvegarde permettant aux utilisateurs de copier des fichiers et des dossiers en toute simplicité. Ce projet est développé en **C#** et s'exécute sous l'environnement **.NET**, avec une architecture modulaire pour faciliter son extensibilité et sa maintenance.

---

## Architecture du projet

EasySave est organisé en plusieurs modules distincts pour assurer une séparation des responsabilités claire.

### Modules principaux

- **EasySave** : Module principal contenant l'interface utilisateur et la logique métier.
- **Logger** : Module dédié à la gestion des logs et des événements.

---

## Détails des dossiers

### **EasySave**

#### **Utils**
Contient des outils et utilitaires comme :
- **JobsState** : Gère l'état des travaux de sauvegarde.
- **StateJsonReader.cs** : Lecture et traitement des fichiers JSON d'état.
- **ConsoleManager** : Gestion des messages et de l'interface utilisateur en ligne de commande.
- **Messages.cs and MessagesReader.cs** : Gestion multilingue.

#### **ConsoleManager.cs**
Gère l'interaction utilisateur via une interface en ligne de commande.  
Ses principales fonctionnalités incluent :

##### **Menus dynamiques**
- **Menu principal** : Créer, mettre à jour, lire ou supprimer des tâches de sauvegarde.
- **Menu de sélection de langue** : Prise en charge multilingue.

##### **Gestion des tâches**
- **CreateSaveJobMenu** : Création d'une nouvelle tâche de sauvegarde avec nom, chemin source, chemin cible et type (complète ou différentielle).
- **UpdateSaveJobMenu** : Mise à jour des tâches de sauvegarde existantes.
- **DeleteSaveJobMenu** : Suppression d'une tâche de sauvegarde et de ses fichiers associés.
- **ReadSaveJobsMenu** : Affichage des tâches de sauvegarde disponibles avec leurs détails.

##### **Support multilingue**
- Utilise la classe **Messages** pour gérer les traductions des messages selon la langue choisie.
- Méthode **LanguageSelectionMenu** pour basculer entre les langues disponibles.

##### **Gestion des erreurs**
- Messages clairs en cas d'entrée invalide ou d'erreurs système.

#### **Gestion des sauvegardes**
- **DifferentialSave.cs** : Implémentation des sauvegardes différentielles.
- **FullSave.cs** : Implémentation des sauvegardes complètes.
- **SaveJob.cs** : Gère les paramètres et l'exécution des tâches de sauvegarde.

##### **Attributs**
- `Name`, `SourcePath`, `TargetPath`, `CreationDate`, `LastUpdate`, `State`.

##### **Méthodes clés**
- **CreateSave()** : Crée une sauvegarde complète par défaut.
- **FullSave(sourcePath, targetPath)** : Effectue une sauvegarde complète récursive.
- **CreateFullSave(sourcePath, saveTargetPath)** : Copie les fichiers et dossiers tout en gérant les sous-répertoires.
- **DeleteSave()** : Supprime un travail de sauvegarde et ses fichiers associés.

#### **Fichiers JSON**
- **state.json** : Fichier principal stockant l'état actuel des sauvegardes.  
  📂 **Emplacement** : `C:\Users\[NomUtilisateur]\AppData\Roaming\EasySave`.

---

### **Logger**
- **Logger.cs** : Responsable de l'écriture des logs dans des fichiers ou des destinations spécifiques.  
  📂 **Emplacement des logs** : `C:\Users\[NomUtilisateur]\AppData\Roaming\EasySave\Logs`.

---

## Fonctionnalités principales

- **Sauvegarde de fichiers et de dossiers** :
  - **Complète** : Copie entière du contenu sélectionné.
  - **Différentielle** : Copie uniquement les fichiers modifiés depuis la dernière sauvegarde.
- **Gestion des logs** pour assurer la traçabilité des opérations.
- **Interface utilisateur intuitive** en ligne de commande.

---

## Installation et exécution

### **Prérequis**
- **Visual Studio 2022** ou supérieur.
- **.NET 8.0** ou version compatible.
