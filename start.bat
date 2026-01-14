@echo off
chcp 65001 >nul
echo ========================================
echo ЗАПУСК ПРИЛОЖЕНИЯ
echo ========================================
echo.

REM Проверка виртуального окружения
if not exist api\venv (
    echo [ОШИБКА] Виртуальное окружение не найдено!
    echo Запустите setup.bat для установки зависимостей.
    pause
    exit /b 1
)

REM Проверка node_modules
if not exist frontend\node_modules (
    echo [ОШИБКА] Зависимости фронтенда не найдены!
    echo Запустите setup.bat для установки зависимостей.
    pause
    exit /b 1
)

echo Запуск бэкенда (порт 8000)...
start "Backend API Server" cmd /k "cd api && venv\Scripts\activate.bat && uvicorn main:app --reload --host 0.0.0.0 --port 8000"

echo Ожидание запуска бэкенда...
timeout /t 3 /nobreak >nul

echo Запуск фронтенда (порт 5173)...
start "Frontend Dev Server" cmd /k "cd frontend && npm run dev"

echo.
echo ========================================
echo ✅ СЕРВЕРЫ ЗАПУЩЕНЫ!
echo ========================================
echo.
echo Бэкенд API:    http://localhost:8000
echo API Docs:      http://localhost:8000/docs
echo Фронтенд:      http://localhost:5173
echo.
echo Окна консоли останутся открытыми.
echo Для остановки закройте окна консоли.
echo.
pause
