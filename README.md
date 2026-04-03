# Les-bleus

Application web .NET 8 composée de deux applications ASP.NET Core MVC partageant une base SQLite.

- Admin: http://localhost:5100
- Vendeur: http://localhost:5101

Le déploiement cible principal est Raspberry Pi (ARMv7) via Docker Compose.

## Stack technique

- .NET 8 (ASP.NET Core MVC)
- Entity Framework Core
- ASP.NET Core Identity
- SQLite (base partagée entre les 2 applications)
- Docker + Docker Compose

## Structure du repository

- `AspNet_FilRouge/`: application Admin
- `AspNet_FilRouge_Vendeur/`: application Vendeur
- `Entities/`: modèles et logique métier partagée
- `Database/`: composants d'accès données historiques
- `Tests/`: tests unitaires, intégration et fonctionnels
- `docker-compose.yaml`: orchestration locale/serveur

## Prérequis

### Exécution avec Docker (recommandée)

- Docker Engine
- Docker Compose
- Git

### Exécution locale sans Docker

- SDK .NET 8
- Git

## Configuration

Variables d'environnement principales (définies dans `docker-compose.yaml`) :

- `ASPNETCORE_URLS`
- `ASPNETCORE_ENVIRONMENT`
- `Sync__SharedDbPath`
- `SeedUsers__AdminPassword`
- `SeedUsers__VendeurPassword`

Comportement base de données :

- Sous Linux: SQLite (fichier partagé)
- Sous Windows: SQL Server LocalDB par défaut

Note Linux: LocalDB n'est pas supporté. Préférer SQLite (configuration actuelle) ou une instance SQL Server classique.

## Démarrage rapide (Docker)

Depuis la racine du dépôt :

```bash
docker compose up -d --build
```

Vérification :

```bash
docker ps
```

Logs :

```bash
docker logs --tail 200 -f filrouge-admin
docker logs --tail 200 -f filrouge-vendeur
```

Arrêt :

```bash
docker compose down
```

## Exécution locale sans Docker

Depuis la racine du dépôt, dans 2 terminaux :

```bash
dotnet run --project AspNet_FilRouge/AspNet_FilRouge.csproj
```

```bash
dotnet run --project AspNet_FilRouge_Vendeur/AspNet_FilRouge_Vendeur.csproj
```

Par défaut sous Linux, les applications créent/utilisent un fichier SQLite partagé `aspnet-filrouge.shared.db` à la racine du repository.

## Comptes seed

Le seed crée les rôles et utilisateurs de base au démarrage.

- Admin: utilise `SeedUsers__AdminPassword`
- Vendeur: utilise `SeedUsers__VendeurPassword`

En développement, des mots de passe par défaut peuvent être utilisés si les variables ne sont pas définies.
En production, définissez explicitement ces variables.

## Tests

Exécuter tous les tests :

```bash
dotnet test
```

Exécuter uniquement les tests unitaires :

```bash
dotnet test Tests/Les-bleus.Tests.Unit/Les-bleus.Tests.Unit.csproj
```

Exécuter uniquement les tests d'intégration :

```bash
dotnet test Tests/Les-bleus.Tests.Integration/Les-bleus.Tests.Integration.csproj
```

Exécuter uniquement les tests fonctionnels :

```bash
dotnet test Tests/Les-bleus.Tests.Functional/Les-bleus.Tests.Functional.csproj
```

## Déploiement Raspberry Pi

Option recommandée : Docker Compose sur la machine cible.

Sur la Raspberry Pi :

```bash
cd /mnt/sauvegarde/git/CSharp/LesBleus
git pull
docker compose up -d --build
```

Le script `deploy.sh` est conservé pour les déploiements manuels, mais Docker Compose reste le flux de référence.

## Persistance

Le volume Docker `filrouge_data` persiste :

- la base SQLite partagée
- les clés Data Protection (`/data/keys`)

Cela évite notamment les invalidations de cookie d'authentification et de jetons antiforgery après redémarrage.

## Dépannage

### BuildKit: error reading server preface: http2: frame too large

```bash
DOCKER_BUILDKIT=0 docker compose up -d --build
```

Puis, si nécessaire, désactiver BuildKit côté daemon Docker et redémarrer Docker.

### Build Docker: parent snapshot ... does not exist

```bash
docker builder prune -af
docker system prune -af --volumes
```

Puis relancer le build.

### Antiforgery: The antiforgery token could not be decrypted

Vérifier que les deux applications partagent le même emplacement de clés Data Protection et le même `ApplicationName`.

### Login impossible en HTTP

Vérifier la configuration cookie Secure/HTTPS et les URLs configurées dans `ASPNETCORE_URLS`.

## Sécurité

- Remplacer les mots de passe de seed par des secrets forts en production.
- Ne pas commiter de secrets réels dans le dépôt.
- Activer HTTPS en production quand possible.
