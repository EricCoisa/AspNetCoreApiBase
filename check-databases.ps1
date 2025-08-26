# =============================================================================
# Script para verificar bancos de dados local e Docker
# =============================================================================

Write-Host "🔍 VERIFICAÇÃO DOS BANCOS DE DADOS" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan
Write-Host ""

# 1. BANCO LOCAL
Write-Host "📍 BANCO LOCAL (Development)" -ForegroundColor Yellow
Write-Host "Path: CoreApiBase/appdb.sqlite" -ForegroundColor Gray
Write-Host ""

if (Test-Path "CoreApiBase/appdb.sqlite") {
    Write-Host "✅ Banco LOCAL encontrado" -ForegroundColor Green
    
    # Verificar tamanho do arquivo
    $localDbSize = (Get-Item "CoreApiBase/appdb.sqlite").Length
    Write-Host "📊 Tamanho: $([math]::Round($localDbSize/1KB, 2)) KB" -ForegroundColor Gray
    
    # Verificar tabelas (se sqlite3 estiver disponível)
    try {
        $tables = sqlite3 "CoreApiBase/appdb.sqlite" "SELECT name FROM sqlite_master WHERE type='table';" 2>$null
        if ($tables) {
            Write-Host "📋 Tabelas:" -ForegroundColor Gray
            $tables | ForEach-Object { Write-Host "   - $_" -ForegroundColor Gray }
            
            # Contar usuários
            $userCount = sqlite3 "CoreApiBase/appdb.sqlite" "SELECT COUNT(*) FROM Users;" 2>$null
            Write-Host "👥 Usuários: $userCount" -ForegroundColor Gray
            
            # Verificar migrações
            $migrations = sqlite3 "CoreApiBase/appdb.sqlite" "SELECT MigrationId FROM __EFMigrationsHistory;" 2>$null
            Write-Host "🔄 Migrações aplicadas:" -ForegroundColor Gray
            $migrations | ForEach-Object { Write-Host "   - $_" -ForegroundColor Gray }
        } else {
            Write-Host "⚠️  SQLite3 não disponível - use o Entity Framework para consultas" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "⚠️  Erro ao acessar banco: $($_.Exception.Message)" -ForegroundColor Yellow
    }
} else {
    Write-Host "❌ Banco LOCAL não encontrado" -ForegroundColor Red
    Write-Host "💡 Execute a aplicação em modo Development para criar o banco" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=================================" -ForegroundColor Cyan

# 2. CONTAINERS DOCKER
Write-Host "📍 CONTAINERS DOCKER" -ForegroundColor Yellow
Write-Host ""

try {
    $containers = docker ps -a --format "table {{.Names}}\t{{.Status}}\t{{.Image}}" | Select-Object -Skip 1
    
    if ($containers) {
        $containers | ForEach-Object {
            $parts = $_ -split '\t'
            $name = $parts[0]
            $status = $parts[1]
            $image = $parts[2]
            
            if ($name -like "*coreapi*") {
                Write-Host "🐳 Container: $name" -ForegroundColor Cyan
                Write-Host "   Status: $status" -ForegroundColor Gray
                Write-Host "   Image: $image" -ForegroundColor Gray
                
                if ($status -like "*Up*") {
                    Write-Host "   ✅ Container ativo - verificando banco..." -ForegroundColor Green
                    
                    # Verificar arquivos de banco no container
                    try {
                        $dbFiles = docker exec $name ls -la /app/data/ 2>$null
                        if ($dbFiles) {
                            Write-Host "   📁 Arquivos em /app/data/:" -ForegroundColor Gray
                            $dbFiles | ForEach-Object { 
                                if ($_ -like "*sqlite*") {
                                    Write-Host "      $($_)" -ForegroundColor Gray
                                }
                            }
                        }
                        
                        # Tentar verificar banco com dotnet (se disponível)
                        # Note: Como não temos sqlite3 no container, usaremos informações do arquivo
                        $dbInfo = docker exec $name ls -l /app/data/appdb.sqlite 2>$null
                        if ($dbInfo) {
                            Write-Host "   📊 Banco Docker encontrado: appdb.sqlite" -ForegroundColor Green
                        }
                    } catch {
                        Write-Host "   ⚠️  Erro ao acessar container: $($_.Exception.Message)" -ForegroundColor Yellow
                    }
                } else {
                    Write-Host "   ⏸️  Container parado" -ForegroundColor Yellow
                }
                Write-Host ""
            }
        }
    } else {
        Write-Host "❌ Nenhum container encontrado" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Docker não disponível: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "=================================" -ForegroundColor Cyan

# 3. VOLUMES DOCKER
Write-Host "📍 VOLUMES DOCKER" -ForegroundColor Yellow
Write-Host ""

try {
    $volumes = docker volume ls --format "table {{.Name}}" | Select-Object -Skip 1
    
    if ($volumes) {
        $volumes | ForEach-Object {
            if ($_ -like "*coreapi*" -or $_ -like "*sqlite*") {
                Write-Host "💾 Volume: $_" -ForegroundColor Cyan
                
                try {
                    $volumeInfo = docker volume inspect $_ | ConvertFrom-Json
                    Write-Host "   Path: $($volumeInfo.Mountpoint)" -ForegroundColor Gray
                    Write-Host "   Driver: $($volumeInfo.Driver)" -ForegroundColor Gray
                } catch {
                    Write-Host "   ⚠️  Erro ao inspecionar volume" -ForegroundColor Yellow
                }
                Write-Host ""
            }
        }
    } else {
        Write-Host "❌ Nenhum volume encontrado" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Erro ao listar volumes: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "=================================" -ForegroundColor Cyan

# 4. RESUMO E RECOMENDAÇÕES
Write-Host "📋 RESUMO" -ForegroundColor Yellow
Write-Host ""
Write-Host "🏠 DESENVOLVIMENTO LOCAL:" -ForegroundColor Green
Write-Host "   • Connection String: Data Source=./appdb.sqlite" -ForegroundColor Gray
Write-Host "   • Localização: CoreApiBase/appdb.sqlite" -ForegroundColor Gray
Write-Host "   • AutoMigrate: true, SeedData: true" -ForegroundColor Gray
Write-Host ""
Write-Host "🐳 DOCKER:" -ForegroundColor Blue
Write-Host "   • Connection String: Data Source=/app/data/appdb.sqlite" -ForegroundColor Gray
Write-Host "   • Localização: /app/data/appdb.sqlite (no container)" -ForegroundColor Gray
Write-Host "   • Volume: mapeado para persistência" -ForegroundColor Gray
Write-Host ""
Write-Host "💡 COMANDOS ÚTEIS:" -ForegroundColor Yellow
Write-Host "   • Ver logs do container: docker logs coreapi-dev" -ForegroundColor Gray
Write-Host "   • Entrar no container: docker exec -it coreapi-dev bash" -ForegroundColor Gray
Write-Host "   • Resetar banco local: Remove-Item CoreApiBase/appdb*" -ForegroundColor Gray
Write-Host "   • Resetar volume Docker: docker volume rm coreapi_dev_data" -ForegroundColor Gray
Write-Host ""
