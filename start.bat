@echo off
title Reservation App Launcher

echo Starting backend...
start cmd /k "cd ReservationApp.Api && dotnet run"

echo Starting frontend...
start cmd /k "cd reservationapp.frontend && npm.cmd install && npm.cmd run dev"

echo Waiting for frontend...

:waitForFrontend
powershell -NoProfile -Command ^
    "try { Invoke-WebRequest -Uri 'http://localhost:5173' -UseBasicParsing -TimeoutSec 1 | Out-Null; exit 0 } catch { exit 1 }"

if errorlevel 1 (
    timeout /t 1 /nobreak > nul
    goto waitForFrontend
)
start http://localhost:5173

echo Reservation App started.
pause