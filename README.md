# Les-bleus

Application web .NET 8 composée de deux applications ASP.NET Core MVC partageant une base SQLite.

- Application Admin: port 5100
- Application Vendeur: port 5101

Le déploiement cible principal est Raspberry Pi (ARMv7) avec Docker Compose.

## Stack technique

- .NET 8 (ASP.NET Core MVC)
- Entity Framework Core
- SQLite (base partagée)
- ASP.NET Core Identity
- Docker + Docker Compose

## Structure du repository

- `AspNet_FilRouge/`: application Admin
- `AspNet_FilRouge_Vendeur/`: application Vendeur
- `Entities/`: modèles et logique métier partagée
- `Database/`: projet lié aux accès données historiques
- `Tests/`: tests unitaires, intégration, fonctionnels
- `docker-compose.yaml`: orchestration locale/serveur

## Prérequis

- Docker Engine
- Docker Compose
- Git

Pour Raspberry Pi 32 bits, utiliser un OS ARMv7.

## Configuration

Variables d'environnement principales (dans `docker-compose.yaml`) :

- `ASPNETCORE_URLS`
- `ASPNETCORE_ENVIRONMENT`
- `Sync__SharedDbPath`
- `SeedUsers__AdminPassword`
- `SeedUsers__VendeurPassword`

## Lancer l'application

Depuis la racine du projet :

```bash
docker compose up -d --build
```

Vérifier les conteneurs :

```bash
docker ps
```

Consulter les logs :

```bash
docker logs --tail 200 -f filrouge-admin
docker logs --tail 200 -f filrouge-vendeur
```

## Déploiement sur Raspberry Pi

Depuis la machine de développement :

```bash
git push raspberry master
```

Sur la Raspberry :

```bash
cd /mnt/sauvegarde/git/CSharp/LesBleus
git pull
docker compose up -d --build
```

## Volumes et persistance

Le volume `filrouge_data` persiste :

- la base SQLite partagée
- les clés Data Protection

Cela évite les invalidations d'authentification et d'antiforgery après redémarrage.

## Dépannage

### 1. BuildKit: `error reading server preface: http2: frame too large`

Contournement immédiat :

```bash
DOCKER_BUILDKIT=0 docker compose up -d --build
```

Configuration daemon Docker (JSON valide unique) :

```json
{
  "data-root": "/mnt/NASStockage1/docker",
  "features": { "buildkit": false }
}
```

Puis redémarrer Docker :

```bash
sudo systemctl restart docker
```

### 2. Build Docker: `parent snapshot ... does not exist`

Nettoyer le cache Docker :

```bash
docker builder prune -af
docker system prune -af --volumes
```

Relancer ensuite le build.

### 3. Antiforgery: `The antiforgery token could not be decrypted`

Cause habituelle : clé Data Protection absente ou non persistée.

Vérifier que les deux apps partagent le même stockage des clés (`/data/keys`) et le même `ApplicationName`.

### 4. Login impossible en HTTP

Cause fréquente : cookie auth forcé en `Secure` alors que l'app tourne en HTTP.

Le projet applique maintenant une policy conditionnelle selon la présence d'un endpoint HTTPS.

## Tests

Exécuter tous les tests :

```bash
dotnet test
```

Exécuter uniquement les tests d'intégration :

```bash
dotnet test Tests/Les-bleus.Tests.Integration/Les-bleus.Tests.Integration.csproj
```

## Sécurité

- Remplacer les mots de passe de seed par des secrets forts en production.
- Éviter de commiter des secrets réels.
- Activer HTTPS en production lorsque possible.
