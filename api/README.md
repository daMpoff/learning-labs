# Smartphone Decision Making API

FastAPI приложение для выбора оптимального смартфона на основе метода TOPSIS.

## Особенности

- ⚡ **FastAPI** - быстрый и современный фреймворк
- 🔄 **Async/await** - асинхронная обработка запросов
- 📚 **Автоматическая документация** - Swagger UI на `/docs`
- 🗄️ **SQLite** - локальная база данных (не требует настройки!)
- 🗄️ **SQLAlchemy** - ORM для работы с БД
- 🔍 **Парсинг NIX.ru** - автоматический сбор данных о смартфонах
- 📊 **TOPSIS** - метод многокритериального принятия решений

## Установка

1. Создать виртуальное окружение:
```bash
python -m venv venv
source venv/bin/activate  # Linux/Mac
venv\Scripts\activate     # Windows
```

2. Установить зависимости:
```bash
pip install -r requirements.txt
```

3. Настроить переменные окружения (опционально):
```bash
# Для SQLite (по умолчанию) - ничего настраивать не нужно!
# База данных будет создана автоматически в файле smartphones.db

# Если нужен PostgreSQL, создайте .env файл:
# DATABASE_URL=postgresql://user:password@localhost:5432/smartphones_db
```

4. Запустить сервер:
```bash
uvicorn main:app --reload --host 0.0.0.0 --port 8000
```

API будет доступен по адресу:
- http://localhost:8000 - API
- http://localhost:8000/docs - Swagger документация
- http://localhost:8000/redoc - ReDoc документация

## Структура проекта

```
api/
├── main.py                 # Точка входа FastAPI
├── config.py              # Конфигурация
├── database.py            # Настройка БД
├── apps/
│   ├── parser/           # Парсинг данных
│   │   ├── parsers/
│   │   │   └── nix_parser.py
│   │   ├── models.py     # Модели данных
│   │   └── routes.py     # API endpoints
│   ├── analysis/         # Анализ и TOPSIS
│   │   ├── topsis.py
│   │   ├── models.py
│   │   └── routes.py
│   └── api/              # Основные API endpoints
│       ├── models.py
│       └── routes.py
└── requirements.txt
```

## API Endpoints

### Парсинг
- `POST /api/parser/start` - Запуск парсинга
- `GET /api/parser/status` - Статус парсинга

### Смартфоны
- `GET /api/smartphones` - Список смартфонов (с фильтрами)
- `GET /api/smartphones/{id}` - Детали смартфона
- `GET /api/smartphones/stats` - Статистика

### Анализ
- `POST /api/analysis/topsis/calculate` - Расчет TOPSIS
- `GET /api/analysis/results` - Результаты анализа

## Разработка

Запуск в режиме разработки с автоперезагрузкой:
```bash
uvicorn main:app --reload
```

Логирование:
```bash
uvicorn main:app --log-level debug
```

