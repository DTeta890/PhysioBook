#!/usr/bin/env bash
set -euo pipefail

COMPOSE_FILE="deploy/docker/docker-compose.yml"
ENV_FILE="deploy/docker/.env"

# Ensure .env exists
if [ ! -f "$ENV_FILE" ]; then
  echo "Creating .env from .env.example..."
  cp deploy/docker/.env.example "$ENV_FILE"
fi

case "${1:-help}" in
  up)
    echo "Starting all services..."
    docker compose -f "$COMPOSE_FILE" --env-file "$ENV_FILE" up -d
    echo "Services started. API: http://localhost:5000 | Frontend: http://localhost:3000"
    echo "RabbitMQ: http://localhost:15672 | MinIO: http://localhost:9001"
    ;;
  down)
    echo "Stopping all services..."
    docker compose -f "$COMPOSE_FILE" --env-file "$ENV_FILE" down
    ;;
  logs)
    docker compose -f "$COMPOSE_FILE" --env-file "$ENV_FILE" logs -f ${2:-}
    ;;
  status)
    docker compose -f "$COMPOSE_FILE" --env-file "$ENV_FILE" ps
    ;;
  reset-db)
    echo "Resetting database..."
    docker compose -f "$COMPOSE_FILE" --env-file "$ENV_FILE" down -v postgres
    docker compose -f "$COMPOSE_FILE" --env-file "$ENV_FILE" up -d postgres
    echo "Database reset complete."
    ;;
  seed)
    echo "Running seed data..."
    docker compose -f "$COMPOSE_FILE" --env-file "$ENV_FILE" exec postgres \
      psql -U physiobook -d physiobook -f /docker-entrypoint-initdb.d/init-db.sql
    echo "Seed complete."
    ;;
  build)
    echo "Rebuilding all images..."
    docker compose -f "$COMPOSE_FILE" --env-file "$ENV_FILE" build
    ;;
  *)
    echo "Usage: ./scripts/dev.sh <command>"
    echo ""
    echo "Commands:"
    echo "  up        Start all services"
    echo "  down      Stop all services"
    echo "  logs      Tail logs (optionally: logs <service>)"
    echo "  status    Show container status"
    echo "  reset-db  Drop and recreate database"
    echo "  seed      Run seed data"
    echo "  build     Rebuild all Docker images"
    ;;
esac
