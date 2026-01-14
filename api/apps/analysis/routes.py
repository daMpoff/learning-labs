"""
API роутеры для анализа данных и расчета TOPSIS
"""
import logging
from fastapi import APIRouter, Depends, HTTPException, Body
from sqlalchemy.orm import Session
from typing import Optional
from database import get_db
from apps.parser.models import Smartphone
from apps.analysis.topsis import calculate_topsis, rank_alternatives
from apps.analysis.utils import (
    get_smartphones_for_analysis,
    prepare_decision_matrix,
    get_default_weights,
    get_criteria_types,
    get_criteria_names
)
from apps.analysis.schemas import (
    TopsisWeightsRequest,
    AnalysisFilters,
    TopsisResponse,
    TopsisResultItem,
    DataStatisticsResponse
)
import numpy as np

router = APIRouter()
logger = logging.getLogger(__name__)


@router.get("/statistics", response_model=DataStatisticsResponse)
async def get_data_statistics(db: Session = Depends(get_db)):
    """Получить статистику данных для анализа"""
    smartphones = get_smartphones_for_analysis(db)
    
    if not smartphones:
        raise HTTPException(status_code=404, detail="Нет данных для анализа")
    
    prices = [p.price for p in smartphones]
    screen_pixels = [p.screen_pixels for p in smartphones]
    ram = [p.ram for p in smartphones]
    storage = [p.storage for p in smartphones]
    camera = [p.camera_main for p in smartphones]
    cores = [p.cores for p in smartphones]
    brands = set(p.brand for p in smartphones if p.brand)
    
    def get_stats(values):
        return {
            'min': float(np.min(values)),
            'max': float(np.max(values)),
            'mean': float(np.mean(values)),
            'median': float(np.median(values))
        }
    
    total = db.query(Smartphone).count()
    
    return DataStatisticsResponse(
        total_count=total,
        valid_count=len(smartphones),
        criteria_count=6,
        price_stats=get_stats(prices),
        screen_pixels_stats=get_stats(screen_pixels),
        ram_stats=get_stats(ram),
        storage_stats=get_stats(storage),
        camera_stats=get_stats(camera),
        cores_stats=get_stats(cores),
        brands_count=len(brands)
    )


@router.post("/topsis", response_model=TopsisResponse)
async def calculate_topsis_analysis(
    filters: Optional[AnalysisFilters] = Body(default=None),
    weights: Optional[TopsisWeightsRequest] = Body(default=None),
    db: Session = Depends(get_db)
):
    """
    Расчет TOPSIS для смартфонов
    
    Args:
        filters: Фильтры для выборки смартфонов (опционально)
        weights: Веса критериев (опционально, используются значения по умолчанию)
        db: Сессия базы данных
    """
    # Получение данных
    filter_dict = filters.dict(exclude_none=True) if filters else None
    smartphones = get_smartphones_for_analysis(db, filter_dict)
    
    if not smartphones:
        raise HTTPException(status_code=404, detail="Нет данных для анализа после применения фильтров")
    
    if len(smartphones) < 2:
        raise HTTPException(status_code=400, detail="Для анализа TOPSIS требуется минимум 2 альтернативы")
    
    # Подготовка матрицы решений
    decision_matrix = prepare_decision_matrix(smartphones)
    
    # Веса критериев
    if weights:
        criteria_weights = [
            weights.price,
            weights.screen_pixels,
            weights.ram,
            weights.storage,
            weights.camera_main,
            weights.cores
        ]
    else:
        criteria_weights = get_default_weights()
    
    # Типы критериев
    criteria_types = get_criteria_types()
    criteria_names = get_criteria_names()
    
    # Расчет TOPSIS
    try:
        relative_closeness, detailed_results = calculate_topsis(
            decision_matrix,
            criteria_weights,
            criteria_types
        )
        
        # Ранжирование
        rankings = rank_alternatives(relative_closeness)
        
        # Формирование результатов
        results = []
        for rank, (alt_index, ci_value) in enumerate(rankings, 1):
            phone = smartphones[alt_index]
            detail = detailed_results[alt_index]
            
            results.append(TopsisResultItem(
                smartphone_id=phone.id,
                name=phone.name,
                brand=phone.brand,
                price=phone.price,
                screen_pixels=phone.screen_pixels,
                ram=phone.ram,
                storage=phone.storage,
                camera_main=phone.camera_main,
                cores=phone.cores,
                rank=rank,
                relative_closeness=ci_value,
                distance_to_pis=detail['distance_to_pis'],
                distance_to_nis=detail['distance_to_nis']
            ))
        
        # Лучшая альтернатива
        best = results[0] if results else None
        if not best:
            raise HTTPException(status_code=500, detail="Ошибка при расчете результатов")
        
        # Топ-10
        top_10 = results[:10]
        
        # Веса в виде словаря
        weights_dict = dict(zip(criteria_names, criteria_weights))
        criteria_types_dict = dict(zip(criteria_names, criteria_types))
        
        return TopsisResponse(
            total_alternatives=len(smartphones),
            best_alternative=best,
            top_10=top_10,
            all_results=results,
            weights_used=weights_dict,
            criteria_types=criteria_types_dict
        )
        
    except Exception as e:
        logger.error(f"Ошибка при расчете TOPSIS: {str(e)}", exc_info=True)
        raise HTTPException(status_code=500, detail=f"Ошибка при расчете TOPSIS: {str(e)}")
