# Руководство по запуску парсинга

## Быстрый старт

### 1. Установить зависимости

```bash
cd api
pip install -r requirements.txt
```

### 2. Запустить сервер

```bash
uvicorn main:app --reload
```

Сервер запустится на `http://localhost:8000`

### 3. Запустить парсинг

#### Вариант 1: Через Swagger UI (рекомендуется)

1. Откройте браузер: http://localhost:8000/docs
2. Найдите endpoint `POST /api/parser/start`
3. Нажмите "Try it out"
4. Укажите `max_pages` (или оставьте `null` для всех страниц)
5. Нажмите "Execute"
6. Смотрите логи в консоли, где запущен сервер

#### Вариант 2: Через curl

```bash
# Запуск парсинга (все страницы)
curl -X POST "http://localhost:8000/api/parser/start"

# Запуск парсинга (только первые 5 страниц для теста)
curl -X POST "http://localhost:8000/api/parser/start?max_pages=5"
```

#### Вариант 3: Через Python скрипт

Создайте файл `test_parsing.py`:

```python
import requests

# Запуск парсинга
response = requests.post("http://localhost:8000/api/parser/start", params={"max_pages": 5})
print(response.json())

# Проверка статуса
status = requests.get("http://localhost:8000/api/parser/status")
print(status.json())
```

## Логирование

Все логи выводятся в консоль в реальном времени:

```
2024-01-13 12:00:00 - INFO - ==================================================
2024-01-13 12:00:00 - INFO - НАЧАЛО ПАРСИНГА СМАРТФОНОВ С NIX.RU
2024-01-13 12:00:00 - INFO - ==================================================
2024-01-13 12:00:01 - INFO - Парсинг страницы 1: https://www.nix.ru/...
2024-01-13 12:00:02 - INFO - Найдено 50 товаров на странице 1
2024-01-13 12:00:03 - INFO - Парсинг товара 1/50: Смартфон Samsung Galaxy...
2024-01-13 12:00:04 - INFO - Успешно спарсен: Смартфон Samsung Galaxy...
...
```

## Проверка статуса

### Через Swagger UI

1. Откройте http://localhost:8000/docs
2. Найдите `GET /api/parser/status`
3. Нажмите "Try it out" → "Execute"

### Через curl

```bash
curl http://localhost:8000/api/parser/status
```

Ответ:
```json
{
  "is_running": true,
  "progress": 25,
  "total": 100,
  "progress_percent": 25,
  "current_item": "Смартфон Samsung Galaxy S23",
  "error": null
}
```

## Проверка результатов

### Количество смартфонов в базе

```bash
curl http://localhost:8000/api/parser/smartphones/count
```

### Список смартфонов

```bash
# Первые 10 смартфонов
curl http://localhost:8000/api/parser/smartphones?skip=0&limit=10
```

### Через Swagger UI

1. Откройте http://localhost:8000/docs
2. Найдите `GET /api/parser/smartphones`
3. Нажмите "Try it out" → "Execute"

## Рекомендации

1. **Для первого теста**: используйте `max_pages=1` или `max_pages=2`
2. **Для полного парсинга**: оставьте `max_pages=null` (все страницы)
3. **Время парсинга**: ~1-3 секунды на товар (с задержками)
4. **Для 200-500 товаров**: потребуется ~10-30 минут

## Остановка парсинга

Парсинг выполняется в фоновом режиме. Чтобы остановить:
1. Остановите сервер (Ctrl+C)
2. Или дождитесь завершения

## Просмотр базы данных

После парсинга данные сохраняются в `smartphones.db`

Просмотреть можно через:
- **DB Browser for SQLite**: https://sqlitebrowser.org/
- **VS Code расширение**: SQLite Viewer
- **Python скрипт**:

```python
import sqlite3

conn = sqlite3.connect('smartphones.db')
cursor = conn.cursor()

cursor.execute("SELECT COUNT(*) FROM smartphones")
print(f"Всего смартфонов: {cursor.fetchone()[0]}")

cursor.execute("SELECT name, price FROM smartphones LIMIT 10")
for row in cursor.fetchall():
    print(f"{row[0]}: {row[1]} руб")

conn.close()
```

