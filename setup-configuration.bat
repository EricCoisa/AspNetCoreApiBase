@echo off
setlocal EnableDelayedExpansion

set "ENVIRONMENT=%~1"
if "%ENVIRONMENT%"=="" set "ENVIRONMENT=Development"

echo =================================================
echo   Configuracao de Secrets - %ENVIRONMENT%
echo =================================================
echo.

REM Função para gerar chave JWT (compatível com PowerShell mais antigo)
powershell -Command "$rng = [System.Security.Cryptography.RNGCryptoServiceProvider]::Create(); $bytes = New-Object byte[] 64; $rng.GetBytes($bytes); [Convert]::ToBase64String($bytes)" > temp_jwt_key.txt
set /p JWT_KEY=<temp_jwt_key.txt
del temp_jwt_key.txt

if /i "%ENVIRONMENT%"=="Development" goto :setup_dev
if /i "%ENVIRONMENT%"=="Dev" goto :setup_dev
if /i "%ENVIRONMENT%"=="Docker" goto :setup_docker
if /i "%ENVIRONMENT%"=="Production" goto :setup_prod
if /i "%ENVIRONMENT%"=="Prod" goto :setup_prod
if /i "%ENVIRONMENT%"=="Release" goto :setup_prod

echo Uso: setup-configuration.bat [Development^|Docker^|Production^|Release]
echo.
echo Exemplos:
echo   setup-configuration.bat Development    # Para desenvolvimento local
echo   setup-configuration.bat Docker         # Para containers
echo   setup-configuration.bat Production     # Orientacoes para producao
echo   setup-configuration.bat Release        # Orientacoes para producao (Visual Studio Release)
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

echo %JWT_KEY% > secrets\jwt_secret
echo http://localhost:8080 > secrets\jwt_issuer  
echo http://localhost:8080 > secrets\jwt_audience
echo Data Source=/app/data/app.sqlite > secrets\db_connection
echo http://localhost:3000,http://localhost:4200,http://localhost:8080 > secrets\cors_origins

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
echo ⚠️  ATENCAO: Em producao use chaves personalizadas e seguras!
echo.

echo [1/6] Configure sua chave JWT (minimo 64 caracteres):
set /p "PROD_JWT_KEY=JWT SecretKey: "
if "%PROD_JWT_KEY%"=="" (
    echo ❌ Chave JWT obrigatoria para producao!
    goto :end
)

echo [2/6] Configure a connection string da base de dados:
set /p "PROD_DB_CONN=Database Connection: "
if "%PROD_DB_CONN%"=="" (
    echo ❌ Connection string obrigatoria para producao!
    goto :end
)

echo [3/6] Configure o dominio/URL do seu sistema:
set /p "PROD_DOMAIN=Dominio (ex: https://meuapp.com): "
if "%PROD_DOMAIN%"=="" set "PROD_DOMAIN=https://localhost"

echo [4/6] Configure dominios CORS permitidos:
set /p "PROD_CORS=CORS Origins (separados por virgula): "
if "%PROD_CORS%"=="" set "PROD_CORS=%PROD_DOMAIN%"

echo [5/6] Criando arquivo de variaveis de ambiente para producao...
echo # Configuracao de Producao - ASP.NET Core API Base > production.env
echo # Copie estas variaveis para seu ambiente de producao >> production.env
echo. >> production.env
echo JWT_SECRET_KEY=%PROD_JWT_KEY% >> production.env
echo JWT_ISSUER=%PROD_DOMAIN% >> production.env
echo JWT_AUDIENCE=%PROD_DOMAIN% >> production.env
echo DATABASE_CONNECTION_STRING=%PROD_DB_CONN% >> production.env
echo CORS_ALLOWED_ORIGINS=%PROD_CORS% >> production.env
echo. >> production.env
echo # Para Docker: >> production.env
echo # docker run --env-file production.env -p 80:8080 coreapi:latest >> production.env

echo [6/6] Configuracao de producao concluida!
echo.
echo =========================================
echo   ✅ PRODUCAO CONFIGURADA!
echo =========================================
echo.
echo 📄 Arquivo criado: production.env
echo.
echo 🔒 PROXIMOS PASSOS:
echo 1. Revise o arquivo production.env
echo 2. Configure as variaveis em seu servidor
echo 3. NUNCA commite o arquivo production.env no Git
echo 4. Use HTTPS em producao
echo 5. Configure backups da base de dados
echo.
echo 🐳 Para Docker:
echo   docker run --env-file production.env -p 80:8080 coreapi:latest
echo.
set "JWT_KEY=%PROD_JWT_KEY%"
goto :end

:end
echo =================================================
echo   Configuracao concluida!
echo =================================================
echo.
if not "%JWT_KEY%"=="" (
    echo =========================================
    echo   🔑 SUA CHAVE JWT GERADA:
    echo   %JWT_KEY%
    echo =========================================
    echo   💾 Salve esta chave em local seguro!
    echo.
)
echo 💡 Dica: Para reconfigurar, execute novamente
echo    setup-configuration.bat ^<Environment^>
echo.
pause
