# Projet Fil Rouge — Les Bleus 🚴

## Cahier des charges — Plan des issues

Ce document décrit le découpage en issues GitHub du cahier des charges du projet Fil Rouge.
Il servira de référence pour la création des issues et de leurs sous-issues dans GitHub.

---

## Issue #1 — Vue d'ensemble du projet Fil Rouge

**Titre :** `[EPIC] Projet Fil Rouge — Application de gestion de vente de vélos`

**Description :**
Développement d'une solution complète de gestion de vente de vélos pour TACTfactory, décomposée en quatre modules :
- Application **Librairie** (bibliothèque partagée)
- Application **Vendeur ASP.NET** (interface vendeur web)
- Application **ASP.NET** (portail web d'administration)
- Application **MAUI** (application multiplateforme vendeur)

**Labels :** `epic`, `enhancement`

**Sous-issues :**
- #2 Application Librairie
- #3 Application Vendeur ASP.NET
- #4 Application ASP.NET
- #5 Contraintes techniques
- #6 Fonctionnalités optionnelles
- #7 Conventions (couleurs & nommage)
- #8 Application MAUI

---

## Issue #2 — Application Librairie

**Titre :** `[LIB] Application Librairie — Entités et modèles de données partagés`

**Description :**
La bibliothèque partagée regroupe l'ensemble des éléments communs à l'application MAUI, Vendeur ASP.NET et ASP.NET.

**Sous-tâches :**

### 2.1 Modélisation des types de produits
- [ ] Créer l'entité `ProductType` (type de produit) avec ses caractéristiques :
  - Taille, Poids, Couleur, Référence
  - Prix HT, TVA
- [ ] Implémenter les 3 variantes de types de produits :
  - `DeliverableProduct` — Produit livrable
  - `InsuredProduct` — Produit avec assurance
  - `ExchangeableProduct` — Produit avec échange
- [ ] Créer l'entité `PhysicalProduct` : produit physique unique identifié par un ID, lié à un `ProductType` (peut appartenir à plusieurs types)

### 2.2 Gestion du stock
- [ ] Créer l'entité `Stock` représentant la quantité disponible par type de produit
- [ ] Implémenter les opérations CRUD sur le stock

### 2.3 Commandes
- [ ] Créer l'entité `Order` (commande) : liste de types de produit quantifiés, associée à un client et un vendeur
- [ ] Supporter la remise (`Discount`) en **pourcentage** sur une commande
- [ ] Lier un vendeur à l'historique de ses commandes

### 2.4 Acteurs
- [ ] Créer l'entité `Seller` (vendeur) avec historique de commandes
- [ ] Créer l'entité `Customer` (client)

**Labels :** `library`, `enhancement`

**Réponses aux questions :**
> - **Quels sont précisément les champs de caractéristiques attendus sur un `ProductType` ?**
>   → Ceux déjà présents (taille, poids, couleur, référence, prix HT, TVA) — pas besoin d'en rajouter pour l'instant.
> - **La remise est-elle en pourcentage, en valeur fixe, ou les deux ?**
>   → En pourcentage uniquement.
> - **Un produit physique peut-il appartenir à plusieurs types de produits ?**
>   → Oui.

---

## Issue #3 — Application Vendeur ASP.NET

**Titre :** `[VENDEUR] Application Vendeur ASP.NET — Interface vendeur web`

**Description :**
Application web dédiée aux vendeurs, permettant la gestion des commandes et la consultation du stock depuis un navigateur.

**Sous-tâches :**

### 3.1 Authentification
- [ ] Implémenter l'écran de connexion (login / mot de passe)
- [ ] Restreindre l'accès aux utilisateurs authentifiés uniquement
- [ ] Gérer la session utilisateur côté vendeur

### 3.2 Gestion des commandes
- [ ] Permettre la création d'une nouvelle commande
- [ ] Ajouter/retirer des produits dans une commande
- [ ] Afficher en temps réel le prix total de la commande à chaque modification
- [ ] Valider une commande
- [ ] Afficher les commandes de tous les vendeurs avec pagination

### 3.3 Consultation du stock
- [ ] Afficher la vue du stock global avec pagination
- [ ] Permettre à un vendeur de faire une demande d'ajout de stock pour un type de produit

### 3.4 Expérience utilisateur
- [ ] Interface responsive (Bootstrap)
- [ ] Appliquer la charte graphique Material Design (`#558C2F` / `#283593`)
- [ ] Thème clair et thème sombre

**Labels :** `vendeur`, `aspnet`, `enhancement`

**Réponses aux questions :**
> - **Les vendeurs voient-ils les commandes des autres vendeurs ?**
>   → Oui.
> - **Y a-t-il une pagination sur la liste des commandes et du stock ?**
>   → Oui.

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
- [ ] Afficher la vue du stock global avec pagination
- [ ] Permettre à un vendeur de faire une demande d'ajout de stock pour un type de produit

### 4.3 Consultation des commandes (Vendeur)
- [ ] Afficher toutes les commandes effectuées par tous les vendeurs avec pagination

### 4.4 Administration (Administrateur)
- [ ] Créer de nouveaux comptes vendeur (champs : nom, prénom, adresse, email, téléphone)
- [ ] Valider ou rejeter les demandes d'ajout de stock
- [ ] Annuler des commandes

**Labels :** `aspnet`, `enhancement`

**Réponses aux questions :**
> - **L'administrateur peut-il également consulter et annuler des commandes ?**
>   → Oui, l'administrateur peut annuler des commandes.
> - **Quels sont les champs requis pour créer un nouveau vendeur ?**
>   → nom, prénom, adresse, email, téléphone.
> - **Y a-t-il une pagination sur la liste des commandes et du stock ?**
>   → Oui.
> - **L'administrateur voit-il également le stock par vendeur, ou uniquement le stock global ?**
>   → Non précisé.

---

## Issue #5 — Contraintes techniques

**Titre :** `[TECH] Contraintes techniques — Configuration et architecture`

**Description :**
Ensemble des contraintes techniques imposées par le cahier des charges.

**Sous-tâches :**

### 5.1 Bibliothèque partagée
- [ ] Cibler .NET 8 (LTS)

### 5.2 Application Vendeur ASP.NET
- [ ] Cibler ASP.NET Core (dernière version LTS)
- [ ] Utiliser **Entity Framework Core (dernière version LTS)**
- [ ] Authentification obligatoire pour tous les accès
- [ ] Interface responsive avec Bootstrap

### 5.3 Application ASP.NET (admin)
- [ ] Compatible avec un serveur web standard (Kestrel / IIS)
- [ ] Utiliser SQL Server ou SQLite comme base de données
- [ ] Utiliser **Entity Framework Core (dernière version LTS)**
- [ ] Authentification obligatoire pour tous les accès
- [ ] Exposer une **API REST** pour la communication avec l'application MAUI

### 5.4 Application MAUI
- [ ] Cibler .NET MAUI (.NET 8 LTS)
- [ ] Multiplateforme : Windows, Android, iOS, macOS
- [ ] Utiliser SQLite comme base de données locale (sans Entity Framework)
- [ ] Connexion sécurisée à l'API REST de l'ASP.NET (login/password)
- [ ] Opérations 100% asynchrones (`async/await`, pas de freeze)
- [ ] Gestion du mode hors-ligne (validation de commande interdite)
- [ ] Synchronisation avec la base centrale via l'API REST

### 5.5 Tests unitaires
- [ ] Tester les entités et la logique métier de la bibliothèque partagée (calcul de prix, remises, stock)
- [ ] Tester les services et les validations de chaque application en isolation
- [ ] Utiliser un framework de test compatible .NET (xUnit recommandé)
- [ ] Utiliser un framework de mock (Moq ou NSubstitute) pour isoler les dépendances
- [ ] Viser une couverture de code significative sur la couche métier

### 5.6 Tests d'intégration
- [ ] Tester les repositories et l'accès aux données via Entity Framework Core (base de données en mémoire ou SQLite)
- [ ] Tester les endpoints de l'API REST (ASP.NET) avec `WebApplicationFactory`
- [ ] Vérifier la cohérence des opérations CRUD sur le stock et les commandes
- [ ] Tester la synchronisation entre la base SQLite locale (MAUI) et l'API REST

### 5.7 Tests fonctionnels
- [ ] Tester les scénarios utilisateur de bout en bout (création de commande, validation, annulation)
- [ ] Tester les flux d'authentification et de gestion des rôles (`Vendeur` / `Administrateur`)
- [ ] Tester le comportement en mode hors-ligne (MAUI) : blocage de la validation, synchronisation au retour en ligne
- [ ] Utiliser un outil de test UI si applicable (ex. Selenium pour ASP.NET, ou tests MAUI avec `UITest`)

### 5.8 Versionning
- [ ] Versionner l'ensemble du code avec Git et publier sur GitHub
- [ ] Ajouter `antoinecronier` comme collaborateur du projet

**Labels :** `technical`, `infrastructure`

**Réponses aux questions :**
> - **Quelle version d'Entity Framework est attendue (EF6 ou EF Core) ?**
>   → Entity Framework Core, dernière version LTS.
> - **La synchronisation UWP ↔ ASP.NET doit-elle utiliser une API REST, SignalR, ou autre ?**
>   → API REST.
> - **Des tests sont-ils attendus ? Si oui, lesquels et quel framework ?**
>   → Oui, obligatoires : tests unitaires, d'intégration et fonctionnels. Framework recommandé : xUnit.

---

## Issue #6 — Fonctionnalités optionnelles

**Titre :** `[OPT] Fonctionnalités optionnelles`

**Description :**
Fonctionnalités non obligatoires pouvant être implémentées en bonus.

**Sous-tâches :**

### 6.1 Validation de commande par email
- [ ] Une commande doit attendre la validation d'un client
- [ ] Le client doit avoir un compte dans l'application
- [ ] Envoyer un email au client avec un lien de validation
- [ ] Valider la commande lors du clic sur le lien

### 6.2 Réapprovisionnement de stock avec délais aléatoires
- [ ] Le réapprovisionnement n'est plus instantané
- [ ] Une application génère aléatoirement des délais de réapprovisionnement
- [ ] Le délai est calculé en fonction des demandes validées par l'administrateur

**Labels :** `optional`, `enhancement`

**Réponses aux questions :**
> - **Quel service d'envoi d'email est préconisé (SMTP, SendGrid, etc.) ?**
>   → À définir (un service d'envoi d'email sera utilisé).
> - **Quel est l'intervalle de délai aléatoire attendu pour le réapprovisionnement ?**
>   → À définir (à voir).
> - **Le client doit-il avoir un compte dans l'application ou uniquement une adresse email ?**
>   → Le client doit avoir un compte.

---

## Issue #7 — Conventions

**Titre :** `[CONV] Conventions — Code couleur et nommage C#`

**Description :**
Respect des conventions de l'interface et du code source.

**Sous-tâches :**

### 7.1 Code couleur (Material Design)
- [ ] Couleur primaire : `#558C2F` (vert)
- [ ] Couleur secondaire : `#283593` (bleu indigo)
- [ ] Appliquer le schéma couleur dans l'application MAUI (styles XAML)
- [ ] Appliquer le schéma couleur dans l'application ASP.NET Vendeur (CSS/Bootstrap)
- [ ] Appliquer le schéma couleur dans l'application ASP.NET admin (CSS/Bootstrap)

### 7.2 Charte graphique
- [ ] Respecter la charte graphique détaillée (typographie, icônes, espacements)
- [ ] Implémenter le thème clair
- [ ] Implémenter le thème sombre

### 7.3 Conventions de nommage C# (Microsoft)
- [ ] `PascalCase` pour les classes, méthodes, propriétés publiques
- [ ] `camelCase` pour les variables locales et paramètres
- [ ] Préfixe `_` pour les champs privés (ou `camelCase` selon le guide Microsoft)
- [ ] Utiliser des régions `#region` / `#endregion` pour organiser le code
- [ ] Ajouter des commentaires XML (`///`) pour la documentation

**Labels :** `design`, `conventions`

**Réponses aux questions :**
> - **Y a-t-il une charte graphique plus détaillée (typographie, icônes, espacements) ?**
>   → Oui.
> - **Le guide de style Microsoft complet doit-il être suivi ?**
>   → Oui, le thème sombre est également requis.

---

## Réponses du client (TACTfactory)

Les réponses suivantes ont été fournies par Antoine CRÔNIER (TACTfactory) :

### Sur les données / modèle
1. **Quels sont tous les attributs attendus pour les caractéristiques d'un produit ?**
   → Ceux déjà présents (taille, poids, couleur, référence, prix HT, TVA) — pas besoin d'en rajouter pour l'instant.
2. **La remise sur commande est-elle en pourcentage, en valeur fixe, ou les deux ?**
   → En pourcentage uniquement.
3. **Un produit physique peut-il appartenir à plusieurs types de produits simultanément ?**
   → Oui.

### Sur l'architecture technique
4. **Quel mécanisme de synchronisation entre l'application MAUI et l'ASP.NET est attendu ?**
   → API REST.
5. **Quelle version d'Entity Framework est attendue (EF6 ou EF Core) ?**
   → Entity Framework Core, dernière version LTS.
6. **Des tests sont-ils requis ? Lesquels ?**
   → Oui, et ils sont **obligatoires** : tests **unitaires**, **d'intégration** et **fonctionnels**.

### Sur les fonctionnalités
7. **L'administrateur peut-il annuler des commandes ?**
   → Oui.
8. **Les vendeurs peuvent-ils voir les commandes des autres vendeurs sur l'application MAUI et Vendeur ASP.NET ?**
   → Oui, les vendeurs voient les commandes de tous les vendeurs.
9. **Comment gérer les conflits de stock (deux vendeurs vendant le même produit simultanément) ?**
   → À définir (à voir).
10. **Quels sont les champs requis pour créer un nouveau vendeur ?**
    → nom, prénom, adresse, email, téléphone.
11. **Y a-t-il une pagination sur la liste des commandes et du stock ?**
    → Oui.

### Sur les fonctionnalités optionnelles
12. **Le service d'email pour la validation de commande (SMTP local ou service externe) ?**
    → À définir (un service d'envoi d'email sera utilisé).
13. **L'intervalle de délai aléatoire pour le réapprovisionnement (ordre de grandeur) ?**
    → À définir (à voir).
14. **Le client doit-il avoir un compte ou uniquement une adresse email ?**
    → Le client doit avoir un compte.

### Sur l'UX/UI
15. **Y a-t-il une maquette ou charte graphique plus détaillée (typographie, icônes, espacements) ?**
    → Oui.
16. **Le thème sombre est-il requis en plus du thème clair ?**
    → Oui, le thème sombre est requis.

---

## Issue #8 — Application MAUI

**Titre :** `[MAUI] Application MAUI — Interface vendeur multiplateforme`

**Description :**
Application native multiplateforme (Windows, Android, iOS, macOS) permettant aux vendeurs de gérer leurs commandes, avec support du mode hors-ligne via une base SQLite locale synchronisée avec l'API REST.

**Sous-tâches :**

### 8.1 Authentification
- [ ] Implémenter l'écran de connexion (login / mot de passe)
- [ ] Sécuriser la connexion à l'API REST de l'ASP.NET
- [ ] Gérer la session utilisateur (token JWT)
- [ ] Détecter et afficher l'état de connexion (en ligne / hors-ligne)

### 8.2 Gestion des commandes
- [ ] Permettre la création d'une nouvelle commande
- [ ] Ajouter/retirer des produits dans une commande
- [ ] Afficher en temps réel le prix total de la commande à chaque modification
- [ ] Valider une commande (uniquement en mode connecté)
- [ ] Afficher les commandes de tous les vendeurs

### 8.3 Base de données locale (SQLite)
- [ ] Mettre en place la base SQLite locale (sans Entity Framework)
- [ ] Synchroniser la base locale avec la base centrale via l'API REST
- [ ] Gérer le mode déconnecté : interdire la validation de commande
- [ ] Gérer les conflits lors de la synchronisation

### 8.4 Expérience utilisateur
- [ ] Aucun freeze — toutes les opérations réseau en `async/await`
- [ ] Indicateurs d'état (chargement, synchronisation, mode hors-ligne)
- [ ] Appliquer la charte graphique Material Design (`#558C2F` / `#283593`)
- [ ] Thème clair et thème sombre
- [ ] Interface adaptée aux formats mobile et tablette

**Labels :** `maui`, `enhancement`

**Réponses aux questions :**
> - **Quel est le mécanisme de synchronisation entre la base locale SQLite et la base centrale ?**
>   → API REST — à définir plus précisément.
> - **Les vendeurs voient-ils les commandes des autres vendeurs ?**
>   → Oui.
> - **Comment gérer les conflits lors de la synchronisation ?**
>   → À définir (à voir).
> - **L'application MAUI supporte-t-elle plusieurs vendeurs sur un même appareil ?**
>   → Non précisé.

---

*Document généré à partir du cahier des charges v1.0 — TACTfactory*
