# AG-UI Demo Workspace - Run Script (PowerShell)
# Starts both .NET backend (port 5018) and Next.js frontend (port 3000)

$ErrorActionPreference = "Stop"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# Check .env file
if (-not (Test-Path "$scriptDir\.env")) {
    Write-Host "ERROR: .env file not found in $scriptDir" -ForegroundColor Red
    Write-Host "Copy .env.example to .env and fill in your Azure OpenAI credentials." -ForegroundColor Yellow
    exit 1
}

Write-Host "Starting AG-UI Demo Workspace..." -ForegroundColor Cyan
Write-Host "  Backend:  http://localhost:5018" -ForegroundColor Green
Write-Host "  Frontend: http://localhost:3000" -ForegroundColor Green
Write-Host ""

# Start backend in a new window
$backendJob = Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$scriptDir\backend'; Write-Host 'Starting .NET backend...' -ForegroundColor Cyan; dotnet run --project McpAguiServer --urls 'http://localhost:5018'" -PassThru

# Give backend a moment to start
Start-Sleep -Seconds 3

# Start frontend in a new window
$frontendJob = Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$scriptDir\frontend'; Write-Host 'Starting Next.js frontend...' -ForegroundColor Cyan; `$env:BACKEND_URL='http://localhost:5018'; pnpm dev" -PassThru

Write-Host ""
Write-Host "Both services starting in separate windows." -ForegroundColor Cyan
Write-Host "Open http://localhost:3000 in your browser." -ForegroundColor Green
Write-Host ""
Write-Host "Press Enter to stop both services..." -ForegroundColor Yellow
Read-Host

# Stop both processes
if (-not $backendJob.HasExited) { Stop-Process -Id $backendJob.Id -Force -ErrorAction SilentlyContinue }
if (-not $frontendJob.HasExited) { Stop-Process -Id $frontendJob.Id -Force -ErrorAction SilentlyContinue }

Write-Host "Services stopped." -ForegroundColor Cyan
