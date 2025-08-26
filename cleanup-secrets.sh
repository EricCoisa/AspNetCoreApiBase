#!/bin/bash

# =================================================
# Script de Limpeza de Secrets e Configurações
# =================================================

echo "================================================="
echo "  Limpeza de Secrets e Configurações"
echo "================================================="
echo

echo "[1/6] Limpando User Secrets..."
cd CoreApiBase
if dotnet user-secrets clear 2>/dev/null; then
    echo "✅ User Secrets limpos"
else
    echo "ℹ️  Nenhum User Secret encontrado"
fi
cd ..

echo "[2/6] Removendo arquivos de secrets locais..."
if [ -d "secrets" ]; then
    rm -rf secrets
    echo "✅ Pasta secrets removida"
else
    echo "ℹ️  Pasta secrets não encontrada"
fi

echo "[3/6] Removendo arquivo de configuração local..."
if [ -f "secrets.env" ]; then
    rm secrets.env
    echo "✅ secrets.env removido"
fi

echo "[4/6] Parando containers Docker..."
docker-compose down 2>/dev/null
docker stop coreapi-container 2>/dev/null
docker rm coreapi-container 2>/dev/null

echo "[5/6] Removendo volumes Docker..."
docker volume rm aspnetcoreapi_coreapi_data 2>/dev/null
docker volume rm aspnetcoreapibase_coreapi_data 2>/dev/null

echo "[6/6] Limpando banco de dados local..."
if [ -f "CoreApiBase/appdb.sqlite" ]; then
    rm CoreApiBase/appdb.sqlite
    echo "✅ Banco SQLite local removido"
fi
if [ -f "CoreApiBase/app.sqlite" ]; then
    rm CoreApiBase/app.sqlite
    echo "✅ Banco SQLite alternativo removido"
fi

echo
echo "========================================="
echo "  ✅ LIMPEZA COMPLETA REALIZADA!"
echo "========================================="
echo
echo "Agora você pode testar do zero:"
echo
echo "1. Para desenvolvimento:"
echo "   ./setup-configuration.sh development"
echo "   cd CoreApiBase"
echo "   dotnet run"
echo
echo "2. Para Docker:"
echo "   ./setup-configuration.sh docker"
echo "   docker-compose up -d"
echo
echo "3. Verificar se tudo foi limpo:"
echo "   dotnet user-secrets list --project CoreApiBase"
echo "   ls -la secrets"
echo
