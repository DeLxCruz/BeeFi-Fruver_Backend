# Script para configurar certificados HTTPS de desarrollo

Write-Host "🔐 Configurando certificados HTTPS de desarrollo..." -ForegroundColor Cyan

# Limpiar certificados anteriores
Write-Host "`n1. Limpiando certificados anteriores..." -ForegroundColor Yellow
dotnet dev-certs https --clean

# Generar nuevo certificado
Write-Host "`n2. Generando nuevo certificado..." -ForegroundColor Yellow
dotnet dev-certs https

# Confiar en el certificado (requiere privilegios de administrador)
Write-Host "`n3. Confiando en el certificado..." -ForegroundColor Yellow
Write-Host "   (Se mostrará un diálogo de seguridad - haz clic en SÍ)" -ForegroundColor Gray
dotnet dev-certs https --trust

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n✅ Certificados configurados correctamente!" -ForegroundColor Green
    Write-Host "`nAhora puedes:" -ForegroundColor Cyan
    Write-Host "  1. Ejecutar: dotnet run" -ForegroundColor White
    Write-Host "  2. Navegar a: https://localhost:7248/health-ui" -ForegroundColor White
} else {
    Write-Host "`n❌ Error al configurar certificados" -ForegroundColor Red
    Write-Host "Intenta ejecutar este script como Administrador" -ForegroundColor Yellow
}
