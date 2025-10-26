
# DERNIÈRE ANALYSE SONAR 
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=sarusman_BlazorGameQuest1234&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=sarusman_BlazorGameQuest1234)

# NOM DES MEMEBRES DU BINOME
Godwin KANLINSOU

Sarusman SATKUNARAJAH SARUSMAN

# Version 2 - Cahier des charges

## Démmarrage VERSION 2
`dotnet build`
`cd BlazorGame.Client`
`dotnet run`

<img width="1600" height="1243" alt="hLZDRjj64BxhAHRA8R9XMxOJfqxX6gKeob6YlufoWYBEOIMkPJhKhhfSsb427me41GeKRT6aADgBUsgYOs_jfHUzILwWVeJExbBIIgJi01LjsDHmvfl_cQK-3mNc8ke56U6BWA2hov___lUl7z3gQY70Bna_m3s2rb4fY5uWxpRkzqc0SXNI4H4dA8z6ttQuB-zNLbSpcV2vJ_kOunvguyxpBcSHzM" src="https://github.com/user-attachments/assets/cc60276c-d95c-4d55-8752-a0af412e07d8" />


# Version 1 - Cahier des charges

https://github.com/sarusman/BlazorGameQuest1234/tree/V1

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

## 5. Installer xUnit
dotnet build
dotnet test

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
