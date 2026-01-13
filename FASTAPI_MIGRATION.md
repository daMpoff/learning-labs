# Миграция с Django на FastAPI

## Почему FastAPI?

### Преимущества FastAPI:
- ⚡ **Быстрый** - один из самых быстрых Python фреймворков
- 🔄 **Асинхронный** - нативная поддержка async/await
- 📚 **Автоматическая документация** - Swagger UI из коробки
- 🎯 **Простота** - меньше кода, больше функциональности
- 🔒 **Type hints** - встроенная валидация через Pydantic
- 📦 **Легковесный** - меньше зависимостей

### Сравнение:

| Параметр | Django | FastAPI |
|----------|--------|--------|
| Размер | ~50MB | ~5MB |
| Производительность | Средняя | Высокая |
| Async поддержка | Частичная | Полная |
| Документация API | Нужна настройка | Автоматическая |
| Кривая обучения | Высокая | Низкая |

## Основные изменения

### 1. Структура проекта

**Было (Django):**
```
api/
├── manage.py
├── config/
│   ├── settings.py
│   ├── urls.py
│   └── wsgi.py
└── apps/
```

**Стало (FastAPI):**
```
api/
├── main.py          # Точка входа
├── config.py        # Конфигурация
├── database.py      # Настройка БД
└── apps/
```

### 2. Запуск сервера

**Django:**
```bash
python manage.py runserver
```

**FastAPI:**
```bash
uvicorn main:app --reload
```

### 3. Создание endpoints

**Django:**
```python
# views.py
from rest_framework.views import APIView
from rest_framework.response import Response

class SmartphoneList(APIView):
    def get(self, request):
        return Response({"data": []})
```

**FastAPI:**
```python
# routes.py
from fastapi import APIRouter

router = APIRouter()

@router.get("/smartphones")
async def get_smartphones():
    return {"data": []}
```

### 4. Валидация данных

**Django (DRF):**
```python
from rest_framework import serializers

class SmartphoneSerializer(serializers.Serializer):
    name = serializers.CharField()
    price = serializers.IntegerField()
```

**FastAPI:**
```python
from pydantic import BaseModel

class Smartphone(BaseModel):
    name: str
    price: int
```

### 5. CORS

**Django:**
```python
# settings.py
INSTALLED_APPS = ['corsheaders']
MIDDLEWARE = ['corsheaders.middleware.CorsMiddleware']
CORS_ALLOWED_ORIGINS = ['http://localhost:5173']
```

**FastAPI:**
```python
# main.py
from fastapi.middleware.cors import CORSMiddleware

app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:5173"],
    allow_methods=["*"],
    allow_headers=["*"],
)
```

### 6. Асинхронные задачи

**Django (Celery):**
```python
from celery import shared_task

@shared_task
def parse_smartphones():
    # Парсинг
    pass
```

**FastAPI (BackgroundTasks):**
```python
from fastapi import BackgroundTasks

@router.post("/parser/start")
async def start_parsing(background_tasks: BackgroundTasks):
    background_tasks.add_task(parse_smartphones)
    return {"status": "started"}
```

Или можно использовать Celery с FastAPI (если нужны более сложные задачи).

## Миграция кода

### Модели (SQLAlchemy вместо Django ORM)

**Django:**
```python
from django.db import models

class Smartphone(models.Model):
    name = models.CharField(max_length=200)
    price = models.DecimalField(max_digits=10, decimal_places=2)
```

**FastAPI (SQLAlchemy):**
```python
from sqlalchemy import Column, Integer, String, Numeric
from database import Base

class Smartphone(Base):
    __tablename__ = "smartphones"
    
    id = Column(Integer, primary_key=True)
    name = Column(String(200))
    price = Column(Numeric(10, 2))
```

### Сериализация

**Django:**
```python
from rest_framework import serializers

class SmartphoneSerializer(serializers.ModelSerializer):
    class Meta:
        model = Smartphone
        fields = '__all__'
```

**FastAPI:**
```python
from pydantic import BaseModel

class SmartphoneResponse(BaseModel):
    id: int
    name: str
    price: float
    
    class Config:
        from_attributes = True  # Для работы с SQLAlchemy
```

## Преимущества для нашего проекта

1. **Парсинг** - асинхронный парсинг будет быстрее
2. **API** - автоматическая документация упростит разработку frontend
3. **Производительность** - быстрая обработка запросов
4. **Простота** - меньше кода для поддержки

## Что осталось без изменений

- Парсер NIX - работает одинаково
- Алгоритм TOPSIS - не зависит от фреймворка
- База данных PostgreSQL - та же
- Frontend (Vue.js) - без изменений

## Следующие шаги

1. ✅ Создан main.py
2. ⏳ Настроить базу данных
3. ⏳ Создать модели SQLAlchemy
4. ⏳ Создать роутеры
5. ⏳ Мигрировать парсер
6. ⏳ Мигрировать TOPSIS

