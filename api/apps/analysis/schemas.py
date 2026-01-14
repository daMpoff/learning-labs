"""
Pydantic схемы для анализа данных
"""
from pydantic import BaseModel, Field
from typing import List, Optional, Dict
from datetime import datetime


class TopsisWeightsRequest(BaseModel):
    """Схема для весов критериев в запросе TOPSIS"""
    price: float = Field(default=0.20, ge=0.0, le=1.0, description="Вес критерия цены")
    screen_pixels: float = Field(default=0.15, ge=0.0, le=1.0, description="Вес критерия разрешения экрана")
    ram: float = Field(default=0.15, ge=0.0, le=1.0, description="Вес критерия оперативной памяти")
    storage: float = Field(default=0.15, ge=0.0, le=1.0, description="Вес критерия встроенной памяти")
    camera_main: float = Field(default=0.15, ge=0.0, le=1.0, description="Вес критерия камеры")
    cores: float = Field(default=0.20, ge=0.0, le=1.0, description="Вес критерия количества ядер")


class AnalysisFilters(BaseModel):
    """Схема фильтров для анализа"""
    min_price: Optional[int] = Field(default=None, ge=0, description="Минимальная цена")
    max_price: Optional[int] = Field(default=None, ge=0, description="Максимальная цена")
    min_ram: Optional[int] = Field(default=None, ge=0, description="Минимальный объем RAM")
    min_storage: Optional[int] = Field(default=None, ge=0, description="Минимальный объем памяти")
    brands: Optional[List[str]] = Field(default=None, description="Список брендов для фильтрации")


class TopsisResultItem(BaseModel):
    """Схема элемента результата TOPSIS"""
    smartphone_id: int
    name: str
    brand: Optional[str]
    price: int
    screen_pixels: int
    ram: int
    storage: int
    camera_main: int
    cores: int
    rank: int
    relative_closeness: float = Field(ge=0.0, le=1.0, description="Относительная близость к идеальному решению")
    distance_to_pis: float = Field(ge=0.0, description="Расстояние до положительного идеального решения")
    distance_to_nis: float = Field(ge=0.0, description="Расстояние до отрицательного идеального решения")


class TopsisResponse(BaseModel):
    """Схема ответа расчета TOPSIS"""
    total_alternatives: int
    best_alternative: TopsisResultItem
    top_10: List[TopsisResultItem]
    all_results: List[TopsisResultItem]
    weights_used: Dict[str, float]
    criteria_types: Dict[str, str]


class DataStatisticsResponse(BaseModel):
    """Схема статистики данных"""
    total_count: int
    valid_count: int
    criteria_count: int
    price_stats: Dict[str, float]
    screen_pixels_stats: Dict[str, float]
    ram_stats: Dict[str, float]
    storage_stats: Dict[str, float]
    camera_stats: Dict[str, float]
    cores_stats: Dict[str, float]
    brands_count: int
