"""
Конфигурация приложения
"""
from pydantic_settings import BaseSettings
from typing import List


class Settings(BaseSettings):
    """Настройки приложения"""
    
    # База данных
    # Для SQLite (по умолчанию - локальная БД в файле)
    DATABASE_URL: str = "sqlite:///./smartphones.db"
    
    # Для PostgreSQL (если нужно, раскомментируйте и укажите свои данные)
    # DATABASE_URL: str = "postgresql://user:password@localhost:5432/smartphones_db"
    
    # CORS
    CORS_ORIGINS: List[str] = [
        "http://localhost:5173",  # Vite dev server
        "http://localhost:3000",  # Альтернативный порт
    ]
    
    # Логирование
    LOG_LEVEL: str = "INFO"
    
    # Парсер
    PARSER_DELAY_MIN: float = 1.0
    PARSER_DELAY_MAX: float = 3.0
    
    class Config:
        env_file = ".env"
        case_sensitive = True


settings = Settings()

