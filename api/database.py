"""
Настройка базы данных
"""
from sqlalchemy import create_engine
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import sessionmaker
from config import settings

# Создание движка БД
# Для SQLite нужно добавить check_same_thread=False
if settings.DATABASE_URL.startswith("sqlite"):
    engine = create_engine(
        settings.DATABASE_URL,
        connect_args={"check_same_thread": False},  # Только для SQLite
        echo=False  # Установите True для отладки SQL запросов
    )
else:
    # Для PostgreSQL и других
    engine = create_engine(
        settings.DATABASE_URL,
        echo=False
    )

# Сессия БД
SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)

# Базовый класс для моделей
Base = declarative_base()


def get_db():
    """
    Dependency для получения сессии БД
    Используется в FastAPI endpoints
    """
    db = SessionLocal()
    try:
        yield db
    finally:
        db.close()


def init_db():
    """Инициализация БД - создание всех таблиц"""
    Base.metadata.create_all(bind=engine)

