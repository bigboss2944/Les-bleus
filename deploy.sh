#!/bin/bash
# Script de déploiement automatique pour Raspberry Pi
# À placer à la racine du dépôt sur la Raspberry
set -e

REPO_DIR="/home/pi/FilRouge" # Adapter si besoin
cd "$REPO_DIR"

echo "[1/4] Pull du dépôt..."
git pull

echo "[2/4] Build des images Docker..."
docker build --platform linux/arm64 -t filrouge:latest ./AspNet_FilRouge

docker build --platform linux/arm64 -t filrouge-vendeur:latest ./AspNet_FilRouge_Vendeur

echo "[3/4] Arrêt des anciens conteneurs..."
docker stop filrouge || true && docker rm filrouge || true
docker stop filrouge-vendeur || true && docker rm filrouge-vendeur || true

echo "[4/4] Lancement des nouveaux conteneurs..."
docker run -d --name filrouge -p 5000:80 filrouge:latest
docker run -d --name filrouge-vendeur -p 5001:80 filrouge-vendeur:latest

echo "Déploiement terminé !"
