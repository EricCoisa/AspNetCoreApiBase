@echo off
setlocal

echo =================================================
echo   Limpeza de Secrets e Configuracoes
echo =================================================
echo.

echo [1/7] Limpando User Secrets...
cd CoreApiBase
dotnet user-secrets clear 2>nul
if %ERRORLEVEL% EQU 0 (
    echo ✅ User Secrets limpos
) else (
    echo ℹ️  Nenhum User Secret encontrado
)
cd ..

echo [2/7] Removendo arquivos de secrets locais...
if exist secrets (
    rmdir /s /q secrets
    echo ✅ Pasta secrets removida
) else (
    echo ℹ️  Pasta secrets não encontrada
)

echo [3/7] Removendo arquivo de configuração local...
if exist secrets.env (
    del secrets.env
    echo ✅ secrets.env removido
)

echo [4/7] Parando containers Docker...
docker-compose down 2>nul
docker stop coreapi-container 2>nul
docker rm coreapi-container 2>nul

echo [5/7] Removendo volumes Docker...
docker volume rm aspnetcoreapi_coreapi_data 2>nul
docker volume rm aspnetcoreapibase_coreapi_data 2>nul

echo [6/7] Limpando banco de dados local...
if exist CoreApiBase\appdb.sqlite (
    del CoreApiBase\appdb.sqlite
    echo ✅ Banco SQLite local removido
)
if exist CoreApiBase\app.sqlite (
    del CoreApiBase\app.sqlite
    echo ✅ Banco SQLite alternativo removido
)
if exist CoreApiBase\appdb-dev.sqlite (
    del CoreApiBase\appdb-dev.sqlite
    echo ✅ Banco SQLite Development removido
)

echo [7/7] Removendo configurações sensíveis dos appsettings...
powershell -Command "$json = Get-Content 'CoreApiBase\appsettings.Development.json' | ConvertFrom-Json; $json.JwtSettings.PSObject.Properties.Remove('SecretKey'); $json.DatabaseSettings.PSObject.Properties.Remove('ConnectionString'); $json | ConvertTo-Json -Depth 10 | Set-Content 'CoreApiBase\appsettings.Development.json'"
echo ✅ Configurações sensíveis removidas do appsettings.Development.json

echo.
echo =========================================
echo   ✅ LIMPEZA COMPLETA REALIZADA!
echo =========================================
echo.
echo Agora você pode testar do zero:
echo.
echo 1. Para desenvolvimento:
echo    setup-configuration.bat development
echo    cd CoreApiBase
echo    dotnet run
echo.
echo 2. Para Docker:
echo    setup-configuration.bat docker
echo    docker-compose up -d
echo.
echo 3. Verificar se tudo foi limpo:
echo    dotnet user-secrets list --project CoreApiBase
echo    dir secrets
echo.
pause
