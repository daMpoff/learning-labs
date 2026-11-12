"""
Модели данных для парсера
"""
from sqlalchemy import Column, Integer, String, Float, DateTime, Text
from sqlalchemy.sql import func
from database import Base


class Smartphone(Base):
    """Модель смартфона"""
    __tablename__ = "smartphones"
    
    id = Column(Integer, primary_key=True, index=True)
    
    # Основная информация
    name = Column(String(500), nullable=False, index=True)
    brand = Column(String(100), index=True)
    model = Column(String(200))
    
    # Цена
    price = Column(Integer, nullable=False, index=True)
    
    # Характеристики
    screen_width = Column(Integer)  # разрешение экрана - ширина
    screen_height = Column(Integer)  # разрешение экрана - высота
    screen_pixels = Column(Integer)  # общее количество пикселей (width * height)
    ram = Column(Integer)  # ГБ
    storage = Column(Integer)  # ГБ
    camera_main = Column(Integer)  # МП
    processor = Column(String(200))
    cores = Column(Integer)  # количество ядер процессора
    os_version = Column(String(100))
    
    # Дополнительно
    device_type = Column(String(100))
    availability = Column(String(100))
    url = Column(Text, unique=True, index=True)
    image_url = Column(Text)
    
    # Метаданные
    parsed_at = Column(DateTime(timezone=True), server_default=func.now())
    updated_at = Column(DateTime(timezone=True), onupdate=func.now())
    
    def __repr__(self):
        return f"<Smartphone(id={self.id}, name='{self.name}', price={self.price})>"

