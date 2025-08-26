@echo off
setlocal EnableDelayedExpansion

set "ENVIRONMENT=%~1"
if "%ENVIRONMENT%"=="" set "ENVIRONMENT=Development"

echo =================================================
echo   Configuracao de Secrets - %ENVIRONMENT%
echo =================================================
echo.

REM Função para gerar chave JWT
powershell -Command "$bytes = New-Object byte[] 64; [System.Security.Cryptography.RandomNumberGenerator]::Fill($bytes); [Convert]::ToBase64String($bytes)" > temp_jwt_key.txt
set /p JWT_KEY=<temp_jwt_key.txt
del temp_jwt_key.txt

if /i "%ENVIRONMENT%"=="Development" goto :setup_dev
if /i "%ENVIRONMENT%"=="Dev" goto :setup_dev
if /i "%ENVIRONMENT%"=="Docker" goto :setup_docker
if /i "%ENVIRONMENT%"=="Production" goto :setup_prod
if /i "%ENVIRONMENT%"=="Prod" goto :setup_prod

echo Uso: setup-configuration.bat [Development^|Docker^|Production]
echo.
echo Exemplos:
echo   setup-configuration.bat Development    # Para desenvolvimento local
echo   setup-configuration.bat Docker         # Para containers
echo   setup-configuration.bat Production     # Orientacoes para producao
pause
exit /b 1

:setup_dev
echo [1/4] Configurando JWT para desenvolvimento...
cd CoreApiBase
dotnet user-secrets set "JwtSettings:SecretKey" "%JWT_KEY%"
dotnet user-secrets set "JwtSettings:Issuer" "https://localhost:5099"
dotnet user-secrets set "JwtSettings:Audience" "https://localhost:5099"
dotnet user-secrets set "JwtSettings:ExpiryMinutes" "180"

echo [2/4] Configurando Database...
dotnet user-secrets set "DatabaseSettings:ConnectionString" "Data Source=app.sqlite"
dotnet user-secrets set "DatabaseSettings:AutoMigrate" "true"
dotnet user-secrets set "DatabaseSettings:SeedData" "true"

echo [3/4] Configurando CORS...
dotnet user-secrets set "CorsSettings:AllowedOrigins:0" "http://localhost:3000"
dotnet user-secrets set "CorsSettings:AllowedOrigins:1" "http://localhost:4200"  
dotnet user-secrets set "CorsSettings:AllowedOrigins:2" "http://localhost:5099"
dotnet user-secrets set "CorsSettings:AllowedOrigins:3" "https://localhost:5099"

echo [4/4] Verificando configuracao...
dotnet user-secrets list

cd ..
echo.
echo =========================================
echo   ✅ DESENVOLVIMENTO CONFIGURADO!
echo =========================================
echo.
echo Agora execute:
echo   cd CoreApiBase
echo   dotnet run
echo.
echo Ou abra no Visual Studio e pressione F5
echo ^(selecione profile 'http'^)
echo.
echo Swagger: http://localhost:5099/swagger
echo Health:  http://localhost:5099/api/health
echo.
goto :end

:setup_docker
echo [1/4] Criando secrets para Docker...
if not exist secrets mkdir secrets

echo !JWT_KEY!> secrets\jwt_secret
echo http://localhost:8080> secrets\jwt_issuer
echo http://localhost:8080> secrets\jwt_audience
echo Data Source=/app/data/app.sqlite> secrets\db_connection
echo http://localhost:3000,http://localhost:4200,http://localhost:8080> secrets\cors_origins

echo [2/4] Configurando permissoes dos secrets...
REM No Windows, os arquivos já são criados com permissões adequadas

echo [3/4] Verificando arquivos criados...
dir secrets

echo [4/4] Docker secrets configurados!
echo.
echo =========================================
echo   ✅ DOCKER CONFIGURADO!
echo =========================================
echo.
echo Agora execute:
echo   docker-compose up -d
echo.
echo Acesse:
echo   Swagger: http://localhost:8080/swagger
echo   Health:  http://localhost:8080/api/health
echo.
echo Para parar: docker-compose down
echo.
goto :end

:setup_prod
echo =========================================
echo   CONFIGURACAO PARA PRODUCAO
echo =========================================
echo.
echo ❌ Para producao, configure manualmente com:
echo.
echo 1. Azure Key Vault / AWS Secrets Manager
echo 2. Variaveis de ambiente seguras
echo 3. Certificados SSL validos
echo 4. Connection strings de producao
echo.
echo Exemplo de variaveis de ambiente:
echo   JWT_SECRET_KEY=^<chave-256-bits-segura^>
echo   DATABASE_CONNECTION_STRING=^<connection-string-producao^>
echo   CORS_ALLOWED_ORIGINS=^<dominios-producao^>
echo.
echo Exemplo de comando Docker para producao:
echo   docker run -e JWT_SECRET_KEY="$JWT_SECRET" \
echo              -e DATABASE_CONNECTION_STRING="$DB_CONN" \
echo              -p 80:8080 coreapi:latest
echo.
goto :end

:end
echo =================================================
echo   Configuracao concluida!
echo =================================================
echo.
echo 💡 Dica: Para reconfigurar, execute novamente
echo    setup-configuration.bat ^<Environment^>
echo.
pause
