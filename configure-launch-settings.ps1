# Script para configurar launchSettings.json com formatação correta
param()

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

# Salvar o arquivo
$newJsonContent | Set-Content 'CoreApiBase\Properties\launchSettings.json' -Encoding UTF8

Write-Host "✅ Visual Studio Docker configurado (template → arquivo real com formatação preservada)" -ForegroundColor Green
