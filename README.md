# DERNIÈRE ANALYSE SONAR 
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=sarusman_BlazorGameQuest1234&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=sarusman_BlazorGameQuest1234)

# Version 1 - Cahier des charges

## Identification de l’ensemble des pages pour le projet.

- Pages Client

    * Page d’accueil / Connexion
    * Tableau de bord joueur
    * Interface de jeu
    * Fin de partie
    * Classement / Historique personnel

- Pages Administrateur

    * Tableau de bord Admin
    * Historique global

- Pages d’erreur

    * Erreur 404
    * Gestion erreur authentification (Keycloak)
    * Page de configuration

## Mise en place d’une Intégration Continue (CI)

À chaque push, le pipeline exécute automatiquement :

https://github.com/sarusman/BlazorGameQuest1234/actions
* Build du projet

* Exécution des tests unitaires

* Analyse de la qualité du code via SonarCloud (maintenabilité, duplication, complexité, couverture de tests).

## Diagamme de cas d'utilisation

### Joueur
<img src=".github/images/image.png" alt="Diagramme Joueur" width="400"/>


### Admin (dev)
<img src=".github/images/image-1.png" alt="Diagramme Admin" width="400"/>

# Comment démarrer le projet

## 1. Cloner le dépôt
git clone https://github.com/ton-compte/BlazorGameQuest1234.git
cd BlazorGameQuest1234
<img src=".github/images/image-2.png" alt="Cloner dépôt" width="300"/>

## 2. Restaurer les dépendances
dotnet restore

## 3. Compiler la solution
dotnet build

## 4. Lancer les projets

### Lancer le client Blazor

<img src=".github/images/image-4.png" alt="Lancer client" width="300"/>
cd BlazorGame.Client
dotnet run
### Accessible sur : http://localhost:5000

### Lancer le service d’authentification

<img src=".github/images/image-5.png" alt="Lancer auth service" width="300"/>
cd AuthenticationServices
dotnet run
### Accessible sur : http://localhost:5001/api/auth

## 5. Urls d'utilisation
- Joueur : http://localhost:5000
- Admin : http://localhost:5000/admin

