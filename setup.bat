@echo off
chcp 65001 >nul
echo ========================================
echo УСТАНОВКА ЗАВИСИМОСТЕЙ
echo ========================================
echo.

REM Проверка Python
python --version >nul 2>&1
if errorlevel 1 (
    echo [ОШИБКА] Python не найден! Установите Python 3.8+
    pause
    exit /b 1
)

REM Проверка Node.js
node --version >nul 2>&1
if errorlevel 1 (
    echo [ОШИБКА] Node.js не найден! Установите Node.js
    pause
    exit /b 1
)

echo [1/3] Установка зависимостей бэкенда (Python)...
echo.

cd api
if not exist venv (
    echo Создание виртуального окружения...
    python -m venv venv
)

echo Активация виртуального окружения...
call venv\Scripts\activate.bat

echo Установка Python пакетов...
pip install --upgrade pip
pip install -r requirements.txt

if errorlevel 1 (
    echo [ОШИБКА] Ошибка при установке зависимостей Python
    cd ..
    pause
    exit /b 1
)

echo.
echo [2/3] Установка зависимостей фронтенда (Node.js)...
echo.

cd ..\frontend
if not exist node_modules (
    echo Установка npm пакетов (это может занять несколько минут)...
    call npm install
) else (
    echo node_modules уже существует, обновление зависимостей...
    call npm install
)

if errorlevel 1 (
    echo [ОШИБКА] Ошибка при установке зависимостей Node.js
    cd ..
    pause
    exit /b 1
)

cd ..

echo.
echo ========================================
echo ✅ УСТАНОВКА ЗАВЕРШЕНА УСПЕШНО!
echo ========================================
echo.
echo Для запуска приложения используйте: start.bat
echo.
pause
