COMPOSE_FILE := deploy/docker/docker-compose.yml
ENV_FILE := deploy/docker/.env

.PHONY: up down logs status reset-db seed build

up: ## Start all services
	@./scripts/dev.sh up

down: ## Stop all services
	@./scripts/dev.sh down

logs: ## Tail all logs
	@./scripts/dev.sh logs

status: ## Show container status
	@./scripts/dev.sh status

reset-db: ## Drop and recreate database
	@./scripts/dev.sh reset-db

seed: ## Run seed data
	@./scripts/dev.sh seed

build: ## Rebuild all Docker images
	@./scripts/dev.sh build

help: ## Show this help
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-20s\033[0m %s\n", $$1, $$2}'
