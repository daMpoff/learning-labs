"""
Pydantic схемы для валидации данных
"""
from pydantic import BaseModel, HttpUrl
from typing import Optional
from datetime import datetime


class SmartphoneBase(BaseModel):
    """Базовая схема смартфона"""
    name: str
    brand: Optional[str] = None
    model: Optional[str] = None
    price: int
    price_min: Optional[int] = None
    price_max: Optional[int] = None
    screen_size: Optional[float] = None
    ram: Optional[int] = None
    storage: Optional[int] = None
    battery: Optional[int] = None
    camera_main: Optional[int] = None
    processor: Optional[str] = None
    os_version: Optional[str] = None
    device_type: Optional[str] = None
    availability: Optional[str] = None
    url: str
    image_url: Optional[str] = None


class SmartphoneCreate(SmartphoneBase):
    """Схема для создания смартфона"""
    pass


class SmartphoneResponse(SmartphoneBase):
    """Схема ответа API"""
    id: int
    parsed_at: Optional[datetime] = None
    updated_at: Optional[datetime] = None
    
    class Config:
        from_attributes = True  # Для работы с SQLAlchemy моделями


class SmartphoneWithImage(SmartphoneResponse):
    """Схема смартфона с информацией об изображении"""
    has_image: bool = False
    
    @classmethod
    def from_orm(cls, obj):
        """Создание объекта с проверкой наличия изображения"""
        data = super().from_orm(obj).dict()
        data['has_image'] = bool(data.get('image_url'))
        return cls(**data)


class SmartphoneListResponse(BaseModel):
    """Схема для списка смартфонов"""
    total: int
    items: list[SmartphoneResponse]

