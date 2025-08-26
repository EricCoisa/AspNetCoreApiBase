# Script para configurar launchS# Salvar o arquivo
$newJsonContent | Set-Content 'CoreApiBase\Properties\launchSettings.json' -Encoding UTF8

Write-Host "[OK] Visual Studio Docker configurado com volume isolado" -ForegroundColor Green com formatação correta

# Copiar template
Copy-Item 'CoreApiBase\Properties\launchSettings.template.json' 'CoreApiBase\Properties\launchSettings.json' -Force

# Ler secrets
$jwtSecret = (Get-Content 'secrets\jwt_secret' -Raw).Trim()
$dbConnection = (Get-Content 'secrets\db_connection' -Raw).Trim()
$corsOrigins = (Get-Content 'secrets\cors_origins' -Raw).Trim()
$jwtIssuer = (Get-Content 'secrets\jwt_issuer' -Raw).Trim()
$jwtAudience = (Get-Content 'secrets\jwt_audience' -Raw).Trim()

# Ler o JSON e fazer manipulação
$jsonContent = Get-Content 'CoreApiBase\Properties\launchSettings.json' -Raw

# Docker sempre usa volumes isolados
$dockerVolume = "coreapi_dev_data:/app/data"
$dockerVolumeProduction = "coreapi_prod_data:/app/data"

# Procurar pela linha que contém ASPNETCORE_URLS na seção Container e adicionar as variáveis após ela
$replacement = @"
        "ASPNETCORE_URLS": "http://+:8080",
        "JWT_SECRET_KEY": "$jwtSecret",
        "DATABASE_CONNECTION_STRING": "$dbConnection",
        "CORS_ALLOWED_ORIGINS": "$corsOrigins",
        "JWT_ISSUER": "$jwtIssuer",
        "JWT_AUDIENCE": "$jwtAudience",
        "DatabaseSettings__AutoMigrate": "true"
"@

$newJsonContent = $jsonContent -replace '"ASPNETCORE_URLS": "http://\+:8080"', $replacement

# Atualizar dockerfileRunArguments para Development
$newJsonContent = $newJsonContent -replace '--rm -v coreapi_dev_data:/app/data"', "--rm $dockerVolume`""

# Atualizar dockerfileRunArguments para Production
$newJsonContent = $newJsonContent -replace '--rm -v coreapi_prod_data:/app/data"', "--rm $dockerVolumeProduction`""

# Salvar o arquivo
$newJsonContent | Set-Content 'CoreApiBase\Properties\launchSettings.json' -Encoding UTF8

$dbTypeText = if ($DatabaseType -eq "shared") { "banco compartilhado" } else { "volume isolado" }
Write-Host "[OK] Visual Studio Docker configurado com $dbTypeText" -ForegroundColor Green
