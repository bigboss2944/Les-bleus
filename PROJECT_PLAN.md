# Projet Fil Rouge — Les Bleus 🚴

## Cahier des charges — Plan des issues

Ce document décrit le découpage en issues GitHub du cahier des charges du projet Fil Rouge.
Il servira de référence pour la création des issues et de leurs sous-issues dans GitHub.

---

## Issue #1 — Vue d'ensemble du projet Fil Rouge

**Titre :** `[EPIC] Projet Fil Rouge — Application de gestion de vente de vélos`

**Description :**
Développement d'une solution complète de gestion de vente de vélos pour TACTfactory, décomposée en trois modules :
- Application **Librairie** (bibliothèque partagée)
- Application **UWP** (interface vendeur Windows)
- Application **ASP.NET** (portail web d'administration)

**Labels :** `epic`, `enhancement`

**Sous-issues :**
- #2 Application Librairie
- #3 Application UWP
- #4 Application ASP.NET
- #5 Contraintes techniques
- #6 Fonctionnalités optionnelles
- #7 Conventions (couleurs & nommage)

---

## Issue #2 — Application Librairie

**Titre :** `[LIB] Application Librairie — Entités et modèles de données partagés`

**Description :**
La bibliothèque partagée regroupe l'ensemble des éléments communs à l'application UWP et ASP.NET.

**Sous-tâches :**

### 2.1 Modélisation des types de produits
- [ ] Créer l'entité `ProductType` (type de produit) avec ses caractéristiques :
  - Taille, Poids, Couleur, Référence
  - Prix HT, TVA
- [ ] Implémenter les 3 variantes de types de produits :
  - `DeliverableProduct` — Produit livrable
  - `InsuredProduct` — Produit avec assurance
  - `ExchangeableProduct` — Produit avec échange
- [ ] Créer l'entité `PhysicalProduct` : produit physique unique identifié par un ID, lié à un `ProductType`

### 2.2 Gestion du stock
- [ ] Créer l'entité `Stock` représentant la quantité disponible par type de produit
- [ ] Implémenter les opérations CRUD sur le stock

### 2.3 Commandes
- [ ] Créer l'entité `Order` (commande) : liste de types de produit quantifiés, associée à un client et un vendeur
- [ ] Supporter la remise (`Discount`) sur une commande
- [ ] Lier un vendeur à l'historique de ses commandes

### 2.4 Acteurs
- [ ] Créer l'entité `Seller` (vendeur) avec historique de commandes
- [ ] Créer l'entité `Customer` (client)

**Labels :** `library`, `enhancement`

**Questions pour complément d'information :**
> - Quels sont précisément les champs de caractéristiques attendus sur un `ProductType` (taille/poids/couleur sont mentionnés — y en a-t-il d'autres) ?
> - Le prix est-il HT uniquement ou TTC (avec TVA calculée automatiquement) ?
> - La remise est-elle en pourcentage, en valeur fixe, ou les deux ?
> - Un produit physique peut-il appartenir à plusieurs types de produits ?

---

## Issue #3 — Application UWP

**Titre :** `[UWP] Application UWP — Interface vendeur Windows 10`

**Description :**
Application bureau Windows 10 permettant aux vendeurs de réaliser des ventes de produits.

**Sous-tâches :**

### 3.1 Authentification
- [ ] Implémenter l'écran de connexion (login / mot de passe)
- [ ] Sécuriser la connexion avec l'application ASP.NET (authentification simple)
- [ ] Gérer la session utilisateur côté UWP

### 3.2 Gestion des commandes
- [ ] Permettre la création d'une nouvelle commande
- [ ] Ajouter/retirer des produits dans une commande
- [ ] Afficher en temps réel le prix total de la commande à chaque modification
- [ ] Valider une commande (uniquement en mode connecté)

### 3.3 Base de données locale (SQLite)
- [ ] Mettre en place la base SQLite locale (sans Entity Framework)
- [ ] Synchroniser la base locale avec la base de données centrale (ASP.NET)
- [ ] Gérer le mode déconnecté : interdit la validation de commande

### 3.4 Expérience utilisateur
- [ ] Aucun freeze — toutes les opérations réseau doivent être asynchrones
- [ ] Indicateurs d'état (chargement, synchronisation, mode hors-ligne)

**Labels :** `uwp`, `enhancement`

**Questions pour complément d'information :**
> - Quel est le mécanisme de synchronisation entre la base locale SQLite et la base centrale ? (temps réel, à la connexion, manuel ?)
> - Les vendeurs voient-ils les commandes des autres vendeurs sur l'UWP ?
> - Comment gérer les conflits lors de la synchronisation (produit vendu simultanément par deux vendeurs) ?
> - L'UWP supporte-t-elle plusieurs vendeurs sur un même poste ?

---

## Issue #4 — Application ASP.NET

**Titre :** `[WEB] Application ASP.NET — Portail web de gestion`

**Description :**
Application web permettant la gestion globale des stocks, des vendeurs et des commandes.

**Sous-tâches :**

### 4.1 Authentification
- [ ] Implémenter le système de connexion (login / mot de passe) pour les vendeurs et administrateurs
- [ ] Restreindre l'accès à tous les utilisateurs authentifiés uniquement
- [ ] Gérer les rôles : `Vendeur` et `Administrateur`

### 4.2 Gestion du stock (Vendeur)
- [ ] Afficher la vue du stock global
- [ ] Permettre à un vendeur de faire une demande d'ajout de stock pour un type de produit

### 4.3 Consultation des commandes (Vendeur)
- [ ] Afficher toutes les commandes effectuées par tous les vendeurs

### 4.4 Administration (Administrateur)
- [ ] Créer de nouveaux comptes vendeur
- [ ] Valider ou rejeter les demandes d'ajout de stock

**Labels :** `aspnet`, `enhancement`

**Questions pour complément d'information :**
> - L'administrateur peut-il également consulter et annuler des commandes ?
> - Quels sont les champs requis pour créer un nouveau vendeur (nom, email, mot de passe, etc.) ?
> - Y a-t-il une pagination sur la liste des commandes et du stock ?
> - L'administrateur voit-il également le stock par vendeur, ou uniquement le stock global ?

---

## Issue #5 — Contraintes techniques

**Titre :** `[TECH] Contraintes techniques — Configuration et architecture`

**Description :**
Ensemble des contraintes techniques imposées par le cahier des charges.

**Sous-tâches :**

### 5.1 Bibliothèque partagée
- [ ] Cibler .NET Framework ≥ 4.6

### 5.2 Application UWP
- [ ] Cibler Windows 10 mise à jour de mai 2019 (Build 18362)
- [ ] Utiliser SQLite comme base de données locale
- [ ] Ne pas utiliser Entity Framework
- [ ] Connexion sécurisée à l'ASP.NET (login/password)
- [ ] Opérations 100% asynchrones (pas de freeze)

### 5.3 Application ASP.NET
- [ ] Compatible IIS Express (Visual Studio)
- [ ] Utiliser SQL Server LocalDb comme base de données
- [ ] Utiliser Entity Framework
- [ ] Authentification obligatoire pour tous les accès

### 5.4 Versionning
- [ ] Versionner l'ensemble du code avec Git et publier sur GitHub
- [ ] Ajouter `antoinecronier` comme collaborateur du projet

**Labels :** `technical`, `infrastructure`

**Questions pour complément d'information :**
> - Quelle version d'Entity Framework est attendue (EF6 ou EF Core) ?
> - La synchronisation UWP ↔ ASP.NET doit-elle utiliser une API REST, SignalR, ou autre ?
> - Des tests unitaires sont-ils attendus ? Si oui, quel framework de test ?

---

## Issue #6 — Fonctionnalités optionnelles

**Titre :** `[OPT] Fonctionnalités optionnelles`

**Description :**
Fonctionnalités non obligatoires pouvant être implémentées en bonus.

**Sous-tâches :**

### 6.1 Validation de commande par email
- [ ] Une commande doit attendre la validation d'un client
- [ ] Envoyer un email au client avec un lien de validation
- [ ] Valider la commande lors du clic sur le lien

### 6.2 Réapprovisionnement de stock avec délais aléatoires
- [ ] Le réapprovisionnement n'est plus instantané
- [ ] Une application génère aléatoirement des délais de réapprovisionnement
- [ ] Le délai est calculé en fonction des demandes validées par l'administrateur

**Labels :** `optional`, `enhancement`

**Questions pour complément d'information :**
> - Quel service d'envoi d'email est préconisé (SMTP, SendGrid, etc.) ?
> - Quel est l'intervalle de délai aléatoire attendu pour le réapprovisionnement (ex: 1h–5j) ?
> - Le client doit-il avoir un compte dans l'application ou uniquement une adresse email ?

---

## Issue #7 — Conventions

**Titre :** `[CONV] Conventions — Code couleur et nommage C#`

**Description :**
Respect des conventions de l'interface et du code source.

**Sous-tâches :**

### 7.1 Code couleur (Material Design)
- [ ] Couleur primaire : `#558C2F` (vert)
- [ ] Couleur secondaire : `#283593` (bleu indigo)
- [ ] Appliquer le schéma couleur dans l'application UWP (styles XAML)
- [ ] Appliquer le schéma couleur dans l'application ASP.NET (CSS/Bootstrap)

### 7.2 Conventions de nommage C# (Microsoft)
- [ ] `PascalCase` pour les classes, méthodes, propriétés publiques
- [ ] `camelCase` pour les variables locales et paramètres
- [ ] Préfixe `_` pour les champs privés (ou `camelCase` selon le guide Microsoft)
- [ ] Utiliser des régions `#region` / `#endregion` pour organiser le code
- [ ] Ajouter des commentaires XML (`///`) pour la documentation

**Labels :** `design`, `conventions`

**Questions pour complément d'information :**
> - Y a-t-il une charte graphique plus détaillée (typographie, icônes, espacements) ?
> - Le guide de style Microsoft complet doit-il être suivi (https://docs.microsoft.com/fr-fr/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) ?

---

## Résumé des questions à poser au client (TACTfactory)

Les questions suivantes doivent être soumises à Antoine CRÔNIER (TACTfactory) pour préciser les exigences :

### Sur les données / modèle
1. Quels sont tous les attributs attendus pour les caractéristiques d'un produit (taille, poids, couleur — y en a-t-il d'autres) ?
2. La remise sur commande est-elle en pourcentage, en valeur fixe, ou les deux ?
3. Un produit physique peut-il appartenir à plusieurs types de produits simultanément ?

### Sur l'architecture technique
4. Quel mécanisme de synchronisation entre l'UWP et l'ASP.NET est attendu (API REST, SignalR, autre) ?
5. Quelle version d'Entity Framework est attendue (EF6 ou EF Core) ?
6. Des tests unitaires sont-ils requis ? Si oui, quel framework (xUnit, NUnit, MSTest) ?

### Sur les fonctionnalités
7. L'administrateur peut-il annuler des commandes ?
8. Les vendeurs peuvent-ils voir les commandes des autres vendeurs sur l'UWP ?
9. Comment gérer les conflits de stock (deux vendeurs vendant le même produit simultanément) ?

### Sur les fonctionnalités optionnelles
10. Le service d'email pour la validation de commande (SMTP local ou service externe) ?
11. L'intervalle de délai aléatoire pour le réapprovisionnement (ordre de grandeur) ?

### Sur l'UX/UI
12. Y a-t-il une maquette ou charte graphique plus détaillée ?
13. Quelles langues doit supporter l'application (français uniquement ?) ?

---

*Document généré à partir du cahier des charges v1.0 — TACTfactory*
