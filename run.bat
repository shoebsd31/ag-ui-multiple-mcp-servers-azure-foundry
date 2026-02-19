@echo off
REM AG-UI Demo Workspace - Run Script (Windows Batch)
REM Starts both .NET backend (port 5018) and Next.js frontend (port 3000)

cd /d "%~dp0"

if not exist ".env" (
    echo ERROR: .env file not found.
    echo Copy .env.example to .env and fill in your Azure OpenAI credentials.
    pause
    exit /b 1
)

echo Starting AG-UI Demo Workspace...
echo   Backend:  http://localhost:5018
echo   Frontend: http://localhost:3000
echo.

REM Start backend in a new window
start "AG-UI Backend" cmd /k "cd /d %~dp0app\backend && dotnet run --project McpAguiServer --urls http://localhost:5018"

REM Wait for backend to initialize
timeout /t 5 /nobreak > nul

REM Start frontend in a new window
start "AG-UI Frontend" cmd /k "cd /d %~dp0app\frontend && set BACKEND_URL=http://localhost:5018 && pnpm dev"

echo.
echo Both services starting in separate windows.
echo Open http://localhost:3000 in your browser.
echo.
pause
