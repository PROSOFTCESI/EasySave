# Documentation du projet EasySave

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
- **Localization et ConsoleManager** : Gestion des messages et de l'interface utilisateur en ligne de commande.
- **Messages.cs et Utils.cs** : Fonctions d'assistance diverses.

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
- **Support des configurations personnalisées** à l'aide de fichiers JSON.
- **Interface utilisateur intuitive** en ligne de commande.

---

## Installation et exécution

### **Prérequis**
- **Visual Studio 2022** ou supérieur.
- **.NET 8.0** ou version compatible.

---

## Conclusion

EasySave est une solution flexible et efficace pour la sauvegarde de fichiers. Grâce à son **architecture modulaire**, il est facile de l'étendre et d'ajouter de nouvelles fonctionnalités selon les besoins des utilisateurs.
