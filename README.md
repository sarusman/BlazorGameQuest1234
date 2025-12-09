
# DERNIÈRE ANALYSE SONAR 
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=sarusman_BlazorGameQuest1234&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=sarusman_BlazorGameQuest1234)

# NOM DES MEMBRES DU BINOME
Juphil Godwin KANLINSOU

Sarusman SATKUNARAJAH


# Version 5 - Cahier des chargess

- **Intégration de Keycloak** :
  - Authentification OpenID Connect
  - Attribution des rôles joueur et admin
  - Sécurisation des API
  - Création d'une page de connexion dans le projet Blazor (seuls les utilisateurs Keycloak peuvent se connecter)
  - Toutes les pages du projet web sont accessibles uniquement à un utilisateur authentifié
- **Enrichissement de la documentation Swagger** : descriptions claires pour chaque endpoint
- **Déploiement sous Docker** : tous les services sont dockerisés

- Un joueur ne voit pas le pseudo des autres joueurs.
- Un admin voit le classement général (pseudo, inventaire, détails de tous).
- La liste des salles est incluse dans le donjon (GET donjon → salles).
- Impossible de créer un joueur si le pseudo existe déjà.
- Les tests négatifs ne tiennent plus compte du client ni des SharedModels (comme vu en classe).
- Exécution des tests :
  ```sh
  dotnet test --settings BlazorGame.Tests/coverlet.runsettings
  ```

## Démarrage
`docker compose up --build`

## URLS : 

## VERSION DEPLOYÉ SUR RENDER :
- PRODUCTION (branche Prod): https://blazorgamequest1234.onrender.com
- DEVELOPPEMENT (branche develop): https://blazorgamequest1234-uat.onrender.com 

## VERSION EN LOCAL : 
- UI (Gateway) : http://localhost:5000
- Backend swagger (Serveur) : http://localhost:8080/swagger/index.html

(Même IHM)
<img width="1600" height="1243" alt="hLZDRjj64BxhAHRA8R9XMxOJfqxX6gKeob6YlufoWYBEOIMkPJhKhhfSsb427me41GeKRT6aADgBUsgYOs_jfHUzILwWVeJExbBIIgJi01LjsDHmvfl_cQK-3mNc8ke56U6BWA2hov___lUl7z3gQY70Bna_m3s2rb4fY5uWxpRkzqc0SXNI4H4dA8z6ttQuB-zNLbSpcV2vJ_kOunvguyxpBcSHzM" src="https://github.com/user-attachments/assets/cc60276c-d95c-4d55-8752-a0af412e07d8" />

## Fonctionnement de la version 5 :

### Rôles et permissions

#### Branche Admin
- Voir le classement général (pseudo, détails de tous les joueurs)
- Voir toute les parties
- Gérer les joueurs (activation, export, etc.)
- Accéder à tous les endpoints d’administration

#### Branche Utilisateur
- Jouer et créer une partie
- Voir uniquement ses propres informations
- Impossible de créer un pseudo déjà existant

> Un utilisateur doit être connecté pour jouer.

### Démarrage
- **Page d'accueil** → Nouvelle aventure → Génération d'un donjon selon la difficulté
  - Facile : 2 salles
  - Normal : 3 salles
  - Difficile : 5 salles
  - Toutes les salles sont différentes

### Jouer
- **URL** : `/donjon/{donjonId}`
- Entrer dans le donjon → Création d'une Partie
- ⚠️ Impossible de rejouer le même donjon pour le même joueur

### Salles
- **URL** : `/salle/{donjonId}/{idSalle}`
- Vidéo selon le type (Combat, Coffre, Piège, Énigme, Repos)
- Un seul choix possible par salle

## Règles de Base

**Score**
- Mis à jour par addition/soustraction d'effets
- Mort si effet mort instantanée OU score < 0
- ⚠️ À 0 : encore en vie

**Fin du jeu**
- Mort du joueur
- OU dernière salle atteinte avec score ≥ 0

**Leaderboard**
- Enregistrement du score final (≥ 0) dans les Scores
- Top 10 par score décroissant


    
## IA & outillage

**Vidéos : générées avec Google Veo 3 (boucles MP4).**

**CSS : design initial produit avec Claude AI.** : **Les médias/feuilles de style ont été générés avec l’aide d’IA puis adaptés manuellement**

**Serveur : front servi par Nginx (SPA fallback activé).**



## Tests
Automatiquement exécuté dans le CI : https://github.com/sarusman/BlazorGameQuest1234/actions
Lancer manuellement :
```sh
dotnet test --settings BlazorGame.Tests/coverlet.runsettings
```
### Coverage 93% pour Gameservice:
<img width="1300" height="496" alt="Capture d’écran 2025-12-09 à 21 26 05" src="https://github.com/user-attachments/assets/eafffd58-0e39-4379-8ea6-b6585fa6b32d" />


## Exemples de requêtes Postman

Voici quelques exemples de requêtes à utiliser dans Postman pour tester l'API :

**Authentification (Keycloak)**
```
POST http://localhost:8180/realms/blazorgame/protocol/openid-connect/token
Body (x-www-form-urlencoded):
  client_id: blazorgame-client
  grant_type: password
  username: player1
  password: player1
```

**Créer une partie**
```
POST http://localhost:8080/api/parties
Headers:
  Authorization: Bearer {access_token}
Body (JSON):
  {
    "donjonId": 1
  }
```

**Récupérer le classement**
```
GET http://localhost:8080/api/scores/leaderboard
Headers:
  Authorization: Bearer {access_token}
```

**Voir l'historique personnel**
```
GET http://localhost:8080/api/scores/history
Headers:
  Authorization: Bearer {access_token}
```

Voici quelques exemples de requêtes à utiliser dans Postman pour tester l'API :

**Authentification (Keycloak)**
```
POST http://localhost:8180/realms/blazorgame/protocol/openid-connect/token
Body (x-www-form-urlencoded):
  client_id: blazorgame-client
  grant_type: password
  username: player1
  password: player1
```

**Créer une partie**
```
POST http://localhost:8080/api/parties
Headers:
  Authorization: Bearer {access_token}
Body (JSON):
  {
    "donjonId": 1
  }
```

**Récupérer le classement**
```
GET http://localhost:8080/api/scores/leaderboard
Headers:
  Authorization: Bearer {access_token}
```

**Voir l'historique personnel**
```
GET http://localhost:8080/api/scores/history
Headers:
  Authorization: Bearer {access_token}
```


## Mise en place d’une Intégration Continue (CI)

À chaque push, le pipeline exécute automatiquement :

https://github.com/sarusman/BlazorGameQuest1234/actions
* Build du projet

* Exécution des tests unitaires

* Analyse de la qualité du code grace a  SonarCloud (maintenabilité, duplication, complexité, couverture de tests).

# Comment démarrer le projet

## 1. Cloner le dépôt
`git clone https://github.com/sarusman/BlazorGameQuest1234.git`
`cd BlazorGameQuest1234`
<img src=".github/images/image-2.png" alt="Cloner dépôt" width="300"/>


## 2. Utiliser une image Docker
`docker compose up --build`


### Informations sur le jeu
## Amplitude des Effets

| Difficulté | Amplitude |
|---|---|
| Facile | 6 |
| Normal | 12 |
| Difficile | 18 |

*Les tirages sont aléatoires avec `Random.Next(min, max)`*

## Types de Salles et Effets

### Combat
| Action | Effet |
|---|---|
| Combattre | +[4..scale] et −[2..(scale/2+1)] |
| Fuir | −[1..3] |
| Fouiller | +[4..scale] OU −[2..(scale/2+1)]* |

### Coffre
| Action | Effet |
|---|---|
| Ouvrir | 10% mort instantanée, 45% +[scale/2..scale+2], 45% −[scale/2..scale] |
| Ignorer | 0 |

### Énigme
| Action | Effet |
|---|---|
| Résoudre | 50% +[scale/2..scale], 50% −[2..(scale/2+1)]* |
| Fuir | −1 |

### Repos
| Action | Effet |
|---|---|
| Continuer | 0 |
| Se reposer | +[2..5] |

### Piège / Rencontre (défaut)
| Action | Effet |
|---|---|
| Avancer | 0 |

**\*** *Pour Fouiller, Résoudre et Ouvrir : le résultat possible est décidé à la génération de la salle, pas au clic*
