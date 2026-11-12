"""
Утилиты для обработки данных для TOPSIS
"""
from typing import List, Dict, Optional
from sqlalchemy.orm import Session
from apps.parser.models import Smartphone
import numpy as np


def get_smartphones_for_analysis(db: Session, filters: Optional[Dict] = None) -> List[Smartphone]:
    """
    Получение смартфонов из БД для анализа
    
    Args:
        db: Сессия базы данных
        filters: Словарь с фильтрами (опционально)
    
    Returns:
        List[Smartphone]: Список смартфонов с полными данными
    """
    query = db.query(Smartphone).filter(
        Smartphone.price.isnot(None),
        Smartphone.price > 0,
        Smartphone.screen_pixels.isnot(None),
        Smartphone.ram.isnot(None),
        Smartphone.camera_main.isnot(None),
        Smartphone.cores.isnot(None),
        Smartphone.storage.isnot(None),
        Smartphone.storage > 0
    )
    
    if filters:
        if 'min_price' in filters:
            query = query.filter(Smartphone.price >= filters['min_price'])
        if 'max_price' in filters:
            query = query.filter(Smartphone.price <= filters['max_price'])
        if 'min_ram' in filters:
            query = query.filter(Smartphone.ram >= filters['min_ram'])
        if 'min_storage' in filters:
            query = query.filter(Smartphone.storage >= filters['min_storage'])
        if 'brands' in filters and filters['brands']:
            query = query.filter(Smartphone.brand.in_(filters['brands']))
    
    return query.all()


def prepare_decision_matrix(smartphones: List[Smartphone]) -> List[List[float]]:
    """
    Подготовка матрицы решений из списка смартфонов
    
    Args:
        smartphones: Список объектов Smartphone
    
    Returns:
        List[List[float]]: Матрица решений [альтернативы × критерии]
        Порядок критериев: [price, screen_pixels, ram, storage, camera_main, cores]
    """
    matrix = []
    for phone in smartphones:
        row = [
            float(phone.price),
            float(phone.screen_pixels),
            float(phone.ram),
            float(phone.storage),
            float(phone.camera_main),
            float(phone.cores)
        ]
        matrix.append(row)
    return matrix


def get_default_weights() -> List[float]:
    """
    Получение весов критериев по умолчанию
    
    Returns:
        List[float]: Веса критериев [price, screen_pixels, ram, storage, camera_main, cores]
    """
    return [0.20, 0.15, 0.15, 0.15, 0.15, 0.20]


def get_criteria_types() -> List[str]:
    """
    Получение типов критериев
    
    Returns:
        List[str]: Типы критериев ['min', 'max', 'max', 'max', 'max', 'max']
        price - MIN (меньше лучше), остальные - MAX (больше лучше)
    """
    return ['min', 'max', 'max', 'max', 'max', 'max']


def get_criteria_names() -> List[str]:
    """
    Получение названий критериев
    
    Returns:
        List[str]: Названия критериев
    """
    return ['price', 'screen_pixels', 'ram', 'storage', 'camera_main', 'cores']
