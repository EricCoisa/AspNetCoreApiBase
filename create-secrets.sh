#!/bin/bash
# Script para criar secrets de exemplo para desenvolvimento local
# Execute este script para gerar arquivos de secrets baseados nos exemplos

echo "Criando arquivos de secrets para desenvolvimento..."

# Criar diretório se não existir
mkdir -p secrets

# Copiar arquivos de exemplo e remover a extensão .example
cp secrets/jwt_secret.txt.example secrets/jwt_secret.txt
cp secrets/jwt_issuer.txt.example secrets/jwt_issuer.txt
cp secrets/jwt_audience.txt.example secrets/jwt_audience.txt
cp secrets/db_connection.txt.example secrets/db_connection.txt
cp secrets/cors_origins.txt.example secrets/cors_origins.txt
cp secrets/db_sa_password.txt.example secrets/db_sa_password.txt

# Definir permissões seguras (apenas para sistemas Unix/Linux)
if [[ "$OSTYPE" == "linux-gnu"* ]] || [[ "$OSTYPE" == "darwin"* ]]; then
    chmod 600 secrets/*.txt
    echo "Permissões de arquivo definidas como 600 (somente leitura para o proprietário)"
fi

echo "✅ Arquivos de secrets criados com sucesso!"
echo ""
echo "📝 IMPORTANTE:"
echo "   - Edite os arquivos secrets/*.txt com seus valores reais"
echo "   - NUNCA faça commit destes arquivos no repositório"
echo "   - Use valores diferentes para cada ambiente (dev/staging/prod)"
echo ""
echo "🔧 Para desenvolvimento local, você pode usar:"
echo "   dotnet user-secrets set \"JwtSettings:SecretKey\" \"seu-valor-aqui\""
echo ""
echo "🐳 Para Docker, os secrets serão montados automaticamente em /run/secrets/"
