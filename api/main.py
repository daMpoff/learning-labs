"""
FastAPI приложение для системы выбора оптимального смартфона
"""
from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from config import settings
from database import init_db

app = FastAPI(
    title="Smartphone Decision Making API",
    description="API для выбора оптимального смартфона на основе метода TOPSIS",
    version="1.0.0"
)

# Настройка CORS
app.add_middleware(
    CORSMiddleware,
    allow_origins=settings.CORS_ORIGINS,
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


@app.on_event("startup")
async def startup_event():
    """Инициализация при запуске приложения"""
    # Создание таблиц в БД
    init_db()
    print("✅ База данных инициализирована")


@app.get("/")
async def root():
    """Корневой endpoint"""
    return {
        "message": "Smartphone Decision Making API",
        "version": "1.0.0",
        "docs": "/docs",
        "database": "SQLite (локальная)" if settings.DATABASE_URL.startswith("sqlite") else "PostgreSQL"
    }


@app.get("/health")
async def health_check():
    """Проверка здоровья API"""
    return {"status": "ok", "database": settings.DATABASE_URL.split("://")[0]}


# Импорт роутеров
from apps.parser.routes import router as parser_router
from apps.analysis.routes import router as analysis_router

app.include_router(parser_router, prefix="/api/parser", tags=["parser"])
app.include_router(analysis_router, prefix="/api/analysis", tags=["analysis"])

