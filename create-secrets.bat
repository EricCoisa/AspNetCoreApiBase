@echo off
REM Script para criar secrets de exemplo para desenvolvimento local (Windows)
REM Execute este script para gerar arquivos de secrets baseados nos exemplos

echo Criando arquivos de secrets para desenvolvimento...

REM Criar diretório se não existir
if not exist secrets mkdir secrets

REM Copiar arquivos de exemplo e remover a extensão .example
copy secrets\jwt_secret.txt.example secrets\jwt_secret.txt >nul
copy secrets\jwt_issuer.txt.example secrets\jwt_issuer.txt >nul
copy secrets\jwt_audience.txt.example secrets\jwt_audience.txt >nul
copy secrets\db_connection.txt.example secrets\db_connection.txt >nul
copy secrets\cors_origins.txt.example secrets\cors_origins.txt >nul
copy secrets\db_sa_password.txt.example secrets\db_sa_password.txt >nul

echo ✅ Arquivos de secrets criados com sucesso!
echo.
echo 📝 IMPORTANTE:
echo    - Edite os arquivos secrets\*.txt com seus valores reais
echo    - NUNCA faca commit destes arquivos no repositório
echo    - Use valores diferentes para cada ambiente (dev/staging/prod)
echo.
echo 🔧 Para desenvolvimento local, você pode usar:
echo    dotnet user-secrets set "JwtSettings:SecretKey" "seu-valor-aqui"
echo.
echo 🐳 Para Docker, os secrets serão montados automaticamente em /run/secrets/

pause
