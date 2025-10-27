
# DERNIÈRE ANALYSE SONAR 
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=sarusman_BlazorGameQuest1234&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=sarusman_BlazorGameQuest1234)

# NOM DES MEMEBRES DU BINOME
Juphil Godwin KANLINSOU

Sarusman SATKUNARAJAH

# Version 2 - Cahier des charges

## Démarrage VERSION 2

`docker compose up --build` <br>

## URLS : 
- UI (Gateway) : http://localhost:5000
- Backend swagger (Serveur) : http://localhost:8080/swagger/index.html

<img width="1600" height="1243" alt="hLZDRjj64BxhAHRA8R9XMxOJfqxX6gKeob6YlufoWYBEOIMkPJhKhhfSsb427me41GeKRT6aADgBUsgYOs_jfHUzILwWVeJExbBIIgJi01LjsDHmvfl_cQK-3mNc8ke56U6BWA2hov___lUl7z3gQY70Bna_m3s2rb4fY5uWxpRkzqc0SXNI4H4dA8z6ttQuB-zNLbSpcV2vJ_kOunvguyxpBcSHzM" src="https://github.com/user-attachments/assets/cc60276c-d95c-4d55-8752-a0af412e07d8" />

## Test VERSION 2

`dotnet test`

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
git clone https://github.com/sarusman/BlazorGameQuest1234.git
cd BlazorGameQuest1234
<img src=".github/images/image-2.png" alt="Cloner dépôt" width="300"/>


## 2. Restaurer les dépendances
dotnet restore

## 3. Builder le projet
dotnet build
