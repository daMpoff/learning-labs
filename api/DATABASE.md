# Настройка базы данных

## SQLite (рекомендуется для курсовой работы)

**Преимущества:**
- ✅ Не требует установки дополнительного ПО
- ✅ Все данные в одном файле (`smartphones.db`)
- ✅ Простое резервное копирование (скопировать файл)
- ✅ Достаточно для курсовой работы (200-500 записей)

**Использование:**
По умолчанию используется SQLite. Ничего настраивать не нужно!

База данных будет автоматически создана в файле `smartphones.db` при первом запуске.

## PostgreSQL (опционально)

Если нужен PostgreSQL для более сложных задач:

1. Установить PostgreSQL локально
2. Создать базу данных:
```sql
CREATE DATABASE smartphones_db;
```

3. Обновить `config.py` или создать `.env`:
```env
DATABASE_URL=postgresql://user:password@localhost:5432/smartphones_db
```

4. Установить драйвер:
```bash
pip install psycopg2-binary
```

## Структура базы данных

### Таблица `smartphones`

| Поле | Тип | Описание |
|------|-----|----------|
| id | Integer | Первичный ключ |
| name | String(500) | Название смартфона |
| brand | String(100) | Бренд |
| model | String(200) | Модель |
| price | Integer | Цена (руб) |
| price_min | Integer | Минимальная цена |
| price_max | Integer | Максимальная цена |
| screen_size | Float | Размер экрана (дюймы) |
| ram | Integer | ОЗУ (ГБ) |
| storage | Integer | Память (ГБ) |
| battery | Integer | Батарея (мАч) |
| camera_main | Integer | Камера (МП) |
| processor | String(200) | Процессор |
| os_version | String(100) | Версия ОС |
| device_type | String(100) | Тип устройства |
| availability | String(100) | Наличие |
| url | Text | Ссылка на товар |
| image_url | Text | URL изображения |
| parsed_at | DateTime | Дата парсинга |
| updated_at | DateTime | Дата обновления |

## Работа с базой данных

### Просмотр данных (SQLite)

Используйте любой SQLite клиент:
- **DB Browser for SQLite** (GUI) - https://sqlitebrowser.org/
- **SQLite CLI** - встроен в Python
- **VS Code расширение** - SQLite Viewer

### Резервное копирование (SQLite)

Просто скопируйте файл:
```bash
cp smartphones.db smartphones_backup.db
```

### Миграции

Для миграций схемы БД можно использовать Alembic (опционально):
```bash
alembic init migrations
alembic revision --autogenerate -m "Initial migration"
alembic upgrade head
```

## Рекомендации

Для курсовой работы **SQLite более чем достаточно**:
- Легко настроить
- Не требует дополнительных сервисов
- Все данные в одном файле
- Быстро работает для 200-500 записей

PostgreSQL имеет смысл использовать только если:
- Нужны сложные запросы
- Планируется масштабирование
- Требуется работа с большими объемами данных

