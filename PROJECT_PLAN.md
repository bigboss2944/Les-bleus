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
- [ ] Afficher les commandes de tous les vendeurs (pas uniquement les siennes)

### 3.3 Base de données locale (SQLite)
- [ ] Mettre en place la base SQLite locale (sans Entity Framework)
- [ ] Synchroniser la base locale avec la base de données centrale (ASP.NET)
- [ ] Gérer le mode déconnecté : interdit la validation de commande

### 3.4 Expérience utilisateur
- [ ] Aucun freeze — toutes les opérations réseau doivent être asynchrones
- [ ] Indicateurs d'état (chargement, synchronisation, mode hors-ligne)

**Labels :** `uwp`, `enhancement`

**Réponses aux questions :**
> - **Quel est le mécanisme de synchronisation entre la base locale SQLite et la base centrale ?**
>   → À définir (à voir).
> - **Les vendeurs voient-ils les commandes des autres vendeurs sur l'UWP ?**
>   → Oui.
> - **Comment gérer les conflits lors de la synchronisation ?**
>   → À définir (à voir).
> - **L'UWP supporte-t-elle plusieurs vendeurs sur un même poste ?**
>   → Non précisé.

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
- [ ] Utiliser **Entity Framework Core (dernière version LTS)**
- [ ] Authentification obligatoire pour tous les accès
- [ ] Exposer une **API REST** pour la communication avec l'UWP

### 5.4 Tests unitaires
- [ ] Mettre en place des tests unitaires (obligatoires)
- [ ] Utiliser un framework de test compatible .NET (xUnit, NUnit ou MSTest)

### 5.5 Versionning
- [ ] Versionner l'ensemble du code avec Git et publier sur GitHub
- [ ] Ajouter `antoinecronier` comme collaborateur du projet

**Labels :** `technical`, `infrastructure`

**Réponses aux questions :**
> - **Quelle version d'Entity Framework est attendue (EF6 ou EF Core) ?**
>   → Entity Framework Core, dernière version LTS.
> - **La synchronisation UWP ↔ ASP.NET doit-elle utiliser une API REST, SignalR, ou autre ?**
>   → API REST.
> - **Des tests unitaires sont-ils attendus ? Si oui, quel framework de test ?**
>   → Oui, et ils sont obligatoires.

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
- [ ] Appliquer le schéma couleur dans l'application UWP (styles XAML)
- [ ] Appliquer le schéma couleur dans l'application ASP.NET (CSS/Bootstrap)

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
4. **Quel mécanisme de synchronisation entre l'UWP et l'ASP.NET est attendu ?**
   → API REST.
5. **Quelle version d'Entity Framework est attendue (EF6 ou EF Core) ?**
   → Entity Framework Core, dernière version LTS.
6. **Des tests unitaires sont-ils requis ?**
   → Oui, et ils sont **obligatoires**.

### Sur les fonctionnalités
7. **L'administrateur peut-il annuler des commandes ?**
   → Oui.
8. **Les vendeurs peuvent-ils voir les commandes des autres vendeurs sur l'UWP ?**
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

*Document généré à partir du cahier des charges v1.0 — TACTfactory*
