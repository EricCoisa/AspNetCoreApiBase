@echo off
setlocal EnableDelayedExpansion

set "ENVIRONMENT=%~1"
set "NO_MIGRATE=%~2"
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

echo Uso: setup-configuration.bat [Development^|Docker^|Production^|Release] [--no-migrate]
echo.
echo Exemplos:
echo   setup-configuration.bat Development              # Com migracoes automaticas
echo   setup-configuration.bat Development --no-migrate # Sem migracoes automaticas  
echo   setup-configuration.bat Docker                   # Para containers
echo   setup-configuration.bat Production               # Orientacoes para producao
echo   setup-configuration.bat Release                  # Orientacoes para producao (Visual Studio Release)
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
dotnet user-secrets set "DatabaseSettings:ConnectionString" "Data Source=./appdb.sqlite"
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
echo [Bonus] Criando launchSettings.json para Visual Studio...
powershell -Command "Copy-Item 'CoreApiBase\Properties\launchSettings.template.json' 'CoreApiBase\Properties\launchSettings.json' -Force; Write-Host '[OK] launchSettings.json criado a partir do template'"

echo.
echo =========================================
echo   [OK] DESENVOLVIMENTO CONFIGURADO!
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
echo [1/5] Criando secrets para Docker...
if not exist secrets mkdir secrets

echo %JWT_KEY% > secrets\jwt_secret
echo http://localhost:8080 > secrets\jwt_issuer  
echo http://localhost:8080 > secrets\jwt_audience

echo [2/5] Configurando banco de dados para Docker...
echo.
echo =========================================
echo   CONFIGURACAO DE BANCO DE DADOS
echo =========================================
echo.
echo Escolha o tipo de banco para Docker:
echo.
echo   1. Volume isolado (padrao)
echo      - Banco Docker: /app/data/app.sqlite
echo      - Banco Local:  ./CoreApiBase/appdb.sqlite
echo      - Resultado:    Bancos separados e independentes
echo.
echo   2. Banco compartilhado
echo      - Banco Docker: /app/data/appdb.sqlite
echo      - Banco Local:  ./CoreApiBase/appdb.sqlite  
echo      - Resultado:    Mesmo banco usado em ambos os ambientes
echo.
set /p "DOCKER_DB_CHOICE=Escolha [1-2] (padrao=1): "

if "%DOCKER_DB_CHOICE%"=="" set "DOCKER_DB_CHOICE=1"

if "%DOCKER_DB_CHOICE%"=="2" (
    echo Data Source=/app/data/appdb.sqlite > secrets\db_connection
    
    echo [2.1/5] Configurando docker-compose.yml para banco compartilhado...
    
    REM Editar docker-compose.yml automaticamente
    powershell -Command "(Get-Content docker-compose.yml) -replace '^(\s*- sqlite_data:/app/data)$', '      # $1' | Set-Content docker-compose.yml"
    powershell -Command "(Get-Content docker-compose.yml) -replace '^(\s*#\s*- \./CoreApiBase:/app/data)$', '      - ./CoreApiBase:/app/data' | Set-Content docker-compose.yml"
    
    echo [OK] Connection string configurada para banco compartilhado
    echo [OK] docker-compose.yml configurado automaticamente
    echo.
    set "SHARED_DB=true"
) else (
    echo Data Source=/app/data/app.sqlite > secrets\db_connection
    
    echo [2.1/5] Configurando docker-compose.yml para volume isolado...
    
    REM Editar docker-compose.yml automaticamente  
    powershell -Command "(Get-Content docker-compose.yml) -replace '^(\s*#\s*- sqlite_data:/app/data)$', '      - sqlite_data:/app/data' | Set-Content docker-compose.yml"
    powershell -Command "(Get-Content docker-compose.yml) -replace '^(\s*- \./CoreApiBase:/app/data)$', '      # $1' | Set-Content docker-compose.yml"
    
    echo [OK] Connection string configurada para volume isolado (padrao)
    echo [OK] docker-compose.yml configurado automaticamente
    set "SHARED_DB=false"
)

echo http://localhost:3000,http://localhost:4200,http://localhost:8080 > secrets\cors_origins

echo [3/5] Configurando permissoes dos secrets...
REM No Windows, os arquivos já são criados com permissões adequadas

echo [4/5] Verificando arquivos criados...
dir secrets

echo [5/5] Docker secrets configurados

echo.
echo [Bonus] Configurando Visual Studio Docker...
powershell -ExecutionPolicy Bypass -File configure-launch-settings.ps1
echo.
echo =========================================
echo   [OK] DOCKER CONFIGURADO!
echo =========================================
echo.
echo Agora execute:
echo   docker-compose up -d
echo.
echo Acesse:
echo   Swagger: http://localhost:8080/swagger
echo   Health:  http://localhost:8080/health
echo.
echo Para parar: docker-compose down
echo.
if "%DOCKER_DB_CHOICE%"=="1" (
    echo [INFO] Banco isolado: dados Docker separados do desenvolvimento
) else (
    echo [INFO] Banco compartilhado: dados sincronizados entre ambientes
)
echo.
goto :end

:setup_prod
echo =========================================
echo   CONFIGURACAO PARA PRODUCAO
echo =========================================
echo.

echo [0/6] Criando launchSettings.json para Visual Studio...
powershell -Command "Copy-Item 'CoreApiBase\Properties\launchSettings.template.json' 'CoreApiBase\Properties\launchSettings.json' -Force; Write-Host '[OK] launchSettings.json criado a partir do template'"
echo.

echo [ATENCAO] Em producao use chaves personalizadas e seguras!
echo.

echo [1/6] Configure sua chave JWT (minimo 64 caracteres):
set /p "PROD_JWT_KEY=JWT SecretKey: "
if "%PROD_JWT_KEY%"=="" (
    echo [ERRO] Chave JWT obrigatoria para producao!
    goto :end
)

echo [2/6] Configure a connection string da base de dados:
set /p "PROD_DB_CONN=Database Connection: "
if "%PROD_DB_CONN%"=="" (
    echo [ERRO] Connection string obrigatoria para producao!
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
echo   [OK] PRODUCAO CONFIGURADA!
echo =========================================
echo.
echo [ARQUIVO] production.env
echo.
echo [PROXIMOS PASSOS]:
echo 1. Revise o arquivo production.env
echo 2. Configure as variaveis em seu servidor
echo 3. NUNCA commite o arquivo production.env no Git
echo 4. Use HTTPS em producao
echo 5. Configure backups da base de dados
echo.
echo [DOCKER]:
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
    echo   [JWT] SUA CHAVE JWT GERADA:
    echo   %JWT_KEY%
    echo =========================================
    echo   [IMPORTANTE] Salve esta chave em local seguro!
    echo.
)
echo [DICA] Para reconfigurar, execute novamente
echo    setup-configuration.bat ^<Environment^>
echo.
pause
