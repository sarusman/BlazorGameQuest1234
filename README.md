
# DERNIÈRE ANALYSE SONAR 
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=sarusman_BlazorGameQuest1234&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=sarusman_BlazorGameQuest1234)

# NOM DES MEMEBRES DU BINOME
Juphil Godwin KANLINSOU

Sarusman SATKUNARAJAH

# Version 3 - Cahier des charges

## Démarrage VERSION 3

`docker compose up --build` <br>

## URLS : 
- UI (Gateway) : http://localhost:5000
- Backend swagger (Serveur) : http://localhost:8080/swagger/index.html

(Même IHM)
<img width="1600" height="1243" alt="hLZDRjj64BxhAHRA8R9XMxOJfqxX6gKeob6YlufoWYBEOIMkPJhKhhfSsb427me41GeKRT6aADgBUsgYOs_jfHUzILwWVeJExbBIIgJi01LjsDHmvfl_cQK-3mNc8ke56U6BWA2hov___lUl7z3gQY70Bna_m3s2rb4fY5uWxpRkzqc0SXNI4H4dA8z6ttQuB-zNLbSpcV2vJ_kOunvguyxpBcSHzM" src="https://github.com/user-attachments/assets/cc60276c-d95c-4d55-8752-a0af412e07d8" />

## Fonctionnement de la version 3 : 

* **Démarrer** : page d’accueil → **Nouvelle aventure** → génération d’un **donjon** selon la difficulté

  * **Facile**: 2 salles · **Normal**: 3 · **Difficile**: 5 (toutes **différentes**)

* **Jouer** : `/donjon/{donjonId}` → **Entrer** → création d’une **Partie** (on peux pas rejouer le même donjon pour le même joueur)

* **Salles** : `/salle/{donjonId}/{idSalle}`

  * Vidéo d’ambiance selon le type (Combat, Coffre, Piège, Enigme, Repos)
  * **Un choix** par salle (Combattre, Fuir, Ouvrir, etc.)

* **Règles** :

  * Score mis à jour par addition/soustraction d’effets
  * **Mort** si effet mort instantanée ou score < 0 (à 0 : encore en vie)
  * Fin quand mort ou dernière salle atteinte

* **Scores** : à la fin, enregistrement du score final (≥ 0) dans `Scores` → **Leaderboard** = top 10 par score décroissant

  * Choix possible et gain/pertes possible :

### mplitude des effets
  * **Facile** -> `6`
  * **Normal** -> 12`
  * **Difficile** -> `scale = 18`

> Les tirages sont aléatoires via `Random.Next(min, max)`.

### Par type de salle

#### Combat

| Choix     | Effets appliqués                                                             |
| --------- | ------------------------------------------------------------------------------------------------------- |
| Combattre | **+**[4..scale] (succès) **et** **−**[2..(scale/2+1)] (blessure)                                        |
| Fuir      | **−**[1..3] (on perd du temps)                                                                          |
| Fouiller  | **Soit** **+**[4..scale] **soit** **−**[2..(scale/2+1)] (piège) – déterminé à la génération de la salle |

#### Coffre

| Choix   | Effets (au moment de la génération)                                                                       |
| ------- | --------------------------------------------------------------------------------------------------------- |
| Ouvrir  | 10% **Mort instantanée** • 45% **+**[scale/2 .. (scale+2)] (trésor) • 45% **−**[scale/2 .. scale] (piège) |
| Ignorer | **0** (rien)                                                                                              |

#### Énigme

| Choix    | Effets                                                  |
| -------- | ------------------------------------------------------- |
| Résoudre | 50% **+**[scale/2 .. scale] • 50% **−**[2..(scale/2+1)] |
| Fuir     | **−1**                                                  |

#### Repos

| Choix      | Effets      |
| ---------- | ----------- |
| Continuer  | **0**       |
| Se reposer | **+**[2..5] |

#### Piège / Rencontre (cas par défaut)

| Choix   | Effets |
| ------- | ------ |
| Avancer | **0**  |

> Remarque : pour certains choix (ex. **Fouiller** en Combat, **Résoudre** en Énigme, **Ouvrir** en Coffre), le profil **gain/perte/mort** est fixé **au moment de la génération de la salle** (pas au clic), garantissant que tous les joueurs voient la même issue potentielle pour cette salle lors de la partie.

    
## IA & outillage

**Vidéos : générées avec Google Veo 3 (boucles MP4).**

**CSS : design initial produit avec Claude AI.** : **Les médias/feuilles de style ont été générés avec l’aide d’IA puis adaptés manuellement**

**Serveur : front servi par Nginx (SPA fallback activé).**


## Test VERSION 3

(Automatiquement éxécuté dans le CI : https://github.com/sarusman/BlazorGameQuest1234/actions).
Vous pouvez aussi lancer : <br>
`dotnet test`

# Version 3 - Cahier des charges
<img width="705" height="418" alt="Capture d’écran 2025-11-09 à 14 05 38" src="https://github.com/user-attachments/assets/a99d07b3-d7a1-46d8-baf0-44f4312b78db" />


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
`git clone https://github.com/sarusman/BlazorGameQuest1234.git`
`cd BlazorGameQuest1234`
<img src=".github/images/image-2.png" alt="Cloner dépôt" width="300"/>


## 2. Utiliser une image Docker
`docker compose up --build`
