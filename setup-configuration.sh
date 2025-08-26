#!/bin/bash

# =================================================
# Script de Configuração de Secrets
# =================================================

ENVIRONMENT=${1:-Development}

echo "================================================="
echo "  Configuração de Secrets - $ENVIRONMENT"
echo "================================================="
echo

# Função para gerar chave JWT segura
generate_jwt_key() {
    if command -v openssl &> /dev/null; then
        openssl rand -base64 64 | tr -d '\n'
    else
        # Fallback usando /dev/urandom
        head -c 64 /dev/urandom | base64 | tr -d '\n'
    fi
}

# Função para configurar desenvolvimento
setup_development() {
    echo "[1/4] Configurando JWT para desenvolvimento..."
    
    JWT_KEY=$(generate_jwt_key)
    ISSUER="https://localhost:5099"
    AUDIENCE="https://localhost:5099"
    
    cd CoreApiBase
    dotnet user-secrets set "JwtSettings:SecretKey" "$JWT_KEY"
    dotnet user-secrets set "JwtSettings:Issuer" "$ISSUER"
    dotnet user-secrets set "JwtSettings:Audience" "$AUDIENCE"
    dotnet user-secrets set "JwtSettings:ExpiryMinutes" "180"
    
    echo "[2/4] Configurando Database..."
    dotnet user-secrets set "DatabaseSettings:ConnectionString" "Data Source=app.sqlite"
    dotnet user-secrets set "DatabaseSettings:AutoMigrate" "true"
    dotnet user-secrets set "DatabaseSettings:SeedData" "true"
    
    echo "[3/4] Configurando CORS..."
    dotnet user-secrets set "CorsSettings:AllowedOrigins:0" "http://localhost:3000"
    dotnet user-secrets set "CorsSettings:AllowedOrigins:1" "http://localhost:4200"
    dotnet user-secrets set "CorsSettings:AllowedOrigins:2" "http://localhost:5099"
    dotnet user-secrets set "CorsSettings:AllowedOrigins:3" "https://localhost:5099"
    
    echo "[4/4] Verificando configuração..."
    dotnet user-secrets list
    
    cd ..
    
    echo
    echo "========================================="
    echo "  ✅ DESENVOLVIMENTO CONFIGURADO!"
    echo "========================================="
    echo
    echo "Agora execute:"
    echo "  cd CoreApiBase"
    echo "  dotnet run"
    echo
    echo "Ou abra no Visual Studio e pressione F5"
    echo "(selecione profile 'http')"
    echo
    echo "Swagger: http://localhost:5099/swagger"
    echo "Health:  http://localhost:5099/api/health"
    echo
}

# Função para configurar Docker
setup_docker() {
    echo "[1/4] Criando secrets para Docker..."
    
    mkdir -p secrets
    
    JWT_KEY=$(generate_jwt_key)
    ISSUER="http://localhost:8080"
    AUDIENCE="http://localhost:8080"
    DB_CONNECTION="Data Source=/app/data/app.sqlite"
    CORS_ORIGINS="http://localhost:3000,http://localhost:4200,http://localhost:8080"
    
    echo -n "$JWT_KEY" > secrets/jwt_secret
    echo -n "$ISSUER" > secrets/jwt_issuer
    echo -n "$AUDIENCE" > secrets/jwt_audience
    echo -n "$DB_CONNECTION" > secrets/db_connection
    echo -n "$CORS_ORIGINS" > secrets/cors_origins
    
    echo "[2/4] Configurando permissões dos secrets..."
    chmod 600 secrets/*
    
    echo "[3/4] Verificando arquivos criados..."
    ls -la secrets/
    
    echo "[4/4] Docker secrets configurados!"
    
    echo
    echo "========================================="
    echo "  ✅ DOCKER CONFIGURADO!"
    echo "========================================="
    echo
    echo "Agora execute:"
    echo "  docker-compose up -d"
    echo
    echo "Acesse:"
    echo "  Swagger: http://localhost:8080/swagger"
    echo "  Health:  http://localhost:8080/api/health"
    echo
    echo "Para parar: docker-compose down"
    echo
}

# Função para configurar produção
setup_production() {
    echo "========================================="
    echo "  CONFIGURAÇÃO PARA PRODUÇÃO"
    echo "========================================="
    echo
    echo "⚠️  ATENÇÃO: Em produção use chaves personalizadas e seguras!"
    echo

    echo "[1/6] Configure sua chave JWT (mínimo 64 caracteres):"
    read -p "JWT SecretKey: " PROD_JWT_KEY
    if [ -z "$PROD_JWT_KEY" ]; then
        echo "❌ Chave JWT obrigatória para produção!"
        exit 1
    fi

    echo "[2/6] Configure a connection string da base de dados:"
    read -p "Database Connection: " PROD_DB_CONN
    if [ -z "$PROD_DB_CONN" ]; then
        echo "❌ Connection string obrigatória para produção!"
        exit 1
    fi

    echo "[3/6] Configure o domínio/URL do seu sistema:"
    read -p "Domínio (ex: https://meuapp.com): " PROD_DOMAIN
    if [ -z "$PROD_DOMAIN" ]; then
        PROD_DOMAIN="https://localhost"
    fi

    echo "[4/6] Configure domínios CORS permitidos:"
    read -p "CORS Origins (separados por vírgula): " PROD_CORS
    if [ -z "$PROD_CORS" ]; then
        PROD_CORS="$PROD_DOMAIN"
    fi

    echo "[5/6] Criando arquivo de variáveis de ambiente para produção..."
    cat > production.env << EOF
# Configuração de Produção - ASP.NET Core API Base
# Copie estas variáveis para seu ambiente de produção

JWT_SECRET_KEY=$PROD_JWT_KEY
JWT_ISSUER=$PROD_DOMAIN
JWT_AUDIENCE=$PROD_DOMAIN
DATABASE_CONNECTION_STRING=$PROD_DB_CONN
CORS_ALLOWED_ORIGINS=$PROD_CORS

# Para Docker:
# docker run --env-file production.env -p 80:8080 coreapi:latest
EOF

    echo "[6/6] Configuração de produção concluída!"
    echo
    echo "========================================="
    echo "  ✅ PRODUÇÃO CONFIGURADA!"
    echo "========================================="
    echo
    echo "📄 Arquivo criado: production.env"
    echo
    echo "🔒 PRÓXIMOS PASSOS:"
    echo "1. Revise o arquivo production.env"
    echo "2. Configure as variáveis em seu servidor"
    echo "3. NUNCA commite o arquivo production.env no Git"
    echo "4. Use HTTPS em produção"
    echo "5. Configure backups da base de dados"
    echo
    echo "🐳 Para Docker:"
    echo "  docker run --env-file production.env -p 80:8080 coreapi:latest"
    echo
    
    JWT_KEY="$PROD_JWT_KEY"
}

# Executa configuração baseada no ambiente
case $ENVIRONMENT in
    "Development"|"Dev"|"dev")
        setup_development
        ;;
    "Docker"|"docker")
        setup_docker
        ;;
    "Production"|"Prod"|"prod")
        setup_production
        ;;
    *)
        echo "Uso: ./setup-configuration.sh [Development|Docker|Production]"
        echo
        echo "Exemplos:"
        echo "  ./setup-configuration.sh Development    # Para desenvolvimento local"
        echo "  ./setup-configuration.sh Docker         # Para containers"
        echo "  ./setup-configuration.sh Production     # Orientações para produção"
        exit 1
        ;;
esac

echo "================================================="
echo "  Configuração concluída!"
echo "================================================="
echo
if [ ! -z "$JWT_KEY" ]; then
    echo "========================================="
    echo "  🔑 SUA CHAVE JWT GERADA:"
    echo "  $JWT_KEY"
    echo "========================================="
    echo "  💾 Salve esta chave em local seguro!"
    echo
fi
echo "💡 Dica: Para reconfigurar, execute novamente"
echo "   ./setup-configuration.sh <Environment>"
echo
