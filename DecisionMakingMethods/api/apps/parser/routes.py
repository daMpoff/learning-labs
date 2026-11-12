"""
API роутеры для парсинга
"""
import logging
from fastapi import APIRouter, BackgroundTasks, Depends, HTTPException, Query
from sqlalchemy.orm import Session
from sqlalchemy import and_, or_
from typing import List, Optional

from database import get_db
from apps.parser.models import Smartphone
from apps.parser.schemas import SmartphoneResponse, SmartphoneCreate
from apps.parser.parsers.nix_parser import NixParser
from apps.parser.parsers.nix_parser_async import NixParserAsync
from apps.parser.parsers.nix_parser_selenium import NixParserSelenium
from apps.parser.utils.logger import (
    setup_colored_logger, 
    print_header, 
    print_progress, 
    print_success, 
    print_error, 
    print_warning,
    print_info,
    print_parsed_item
)
import asyncio

try:
    from colorama import Fore
except ImportError:
    class Fore:
        RED = ''
        GREEN = ''
        YELLOW = ''
        BLUE = ''
        CYAN = ''

logger = setup_colored_logger(__name__, logging.INFO)
router = APIRouter()

parsing_status = {
    "is_running": False,
    "progress": 0,
    "total": 0,
    "current_item": "",
    "error": None
}


def save_smartphone_to_db(db: Session, smartphone_data: dict) -> Smartphone:
    """Сохранение смартфона в БД"""
    existing = db.query(Smartphone).filter(
        Smartphone.url == smartphone_data.get('url')
    ).first()
    
    if existing:
        for key, value in smartphone_data.items():
            if hasattr(existing, key) and value is not None:
                setattr(existing, key, value)
        return existing
    else:
        smartphone = Smartphone(**smartphone_data)
        db.add(smartphone)
        return smartphone


def parse_and_save(db: Session, max_pages: int = None, use_async: bool = False, use_selenium: bool = True, use_page_all: bool = True):
    """Функция парсинга и сохранения в БД
    
    Args:
        db: Сессия базы данных
        max_pages: Максимальное количество страниц
        use_async: Использовать асинхронный парсер (не работает с JS-пагинацией)
        use_selenium: Использовать Selenium парсер (для JS-пагинации, рекомендуется)
        use_page_all: Использовать page=all для получения всех товаров сразу (работает только с Selenium)
    """
    global parsing_status
    
    try:
        parsing_status["is_running"] = True
        parsing_status["error"] = None
        parsing_status["progress"] = 0
        parsing_status["total"] = 0
        
        print_header("НАЧАЛО ПАРСИНГА СМАРТФОНОВ С NIX.RU")
        if use_selenium:
            print_info(f"Режим: SELENIUM (для JavaScript-пагинации)")
            if use_page_all:
                print_info(f"Используется page=all (все товары за один раз)")
        elif use_async:
            print_info(f"Режим: АСИНХРОННЫЙ (быстрый, но не работает с JS-пагинацией)")
        else:
            print_info(f"Режим: СИНХРОННЫЙ")
        print_info(f"Максимальное количество страниц: {max_pages or 'все'}")
        print()
        
        if use_selenium:
            parser = NixParserSelenium(headless=True, wait_timeout=10)
            smartphones_data = parser.parse_all_smartphones(max_pages=max_pages, use_page_all=use_page_all)
        elif use_async:
            parser = NixParserAsync(delay_min=0.3, delay_max=0.8, max_concurrent=15)
            smartphones_data = asyncio.run(parser.parse_all_smartphones(max_pages=max_pages))
        else:
            parser = NixParser(delay_min=1, delay_max=3)
            smartphones_data = parser.parse_all_smartphones(max_pages=max_pages)
        
        parsing_status["total"] = len(smartphones_data)
        print_success(f"Всего получено смартфонов: {len(smartphones_data)}")
        print()
        
        print_header("СОХРАНЕНИЕ В БАЗУ ДАННЫХ", char="-")
        saved_count = 0
        updated_count = 0
        
        import time
        save_start_time = time.time()
        
        for idx, smartphone_data in enumerate(smartphones_data, 1):
            item_start_time = time.time()
            try:
                clean_data = {k: v for k, v in smartphone_data.items() 
                              if k != 'is_smartphone' and hasattr(Smartphone, k)}
                
                if 'price' in clean_data and clean_data['price']:
                    if clean_data['price'] > 50_000_000 or clean_data['price'] < 0:
                        logger.warning(f"Пропуск товара {clean_data.get('name')} - недопустимая цена: {clean_data['price']}")
                        continue
                
                if 'ram' in clean_data and clean_data['ram']:
                    if clean_data['ram'] > 64 or clean_data['ram'] < 0:
                        clean_data['ram'] = None
                
                if 'storage' in clean_data and clean_data['storage']:
                    if clean_data['storage'] > 2048 or clean_data['storage'] < 0:
                        clean_data['storage'] = None
                
                if 'camera_main' in clean_data and clean_data['camera_main']:
                    if clean_data['camera_main'] > 500 or clean_data['camera_main'] < 0:
                        clean_data['camera_main'] = None
                
                if 'screen_width' in clean_data and clean_data['screen_width']:
                    if clean_data['screen_width'] > 10000 or clean_data['screen_width'] < 0:
                        clean_data['screen_width'] = None
                
                if 'screen_height' in clean_data and clean_data['screen_height']:
                    if clean_data['screen_height'] > 10000 or clean_data['screen_height'] < 0:
                        clean_data['screen_height'] = None
                
                if 'screen_pixels' in clean_data and clean_data['screen_pixels']:
                    if clean_data['screen_pixels'] > 100_000_000 or clean_data['screen_pixels'] < 0:
                        clean_data['screen_pixels'] = None
                
                if 'cores' in clean_data and clean_data['cores']:
                    if clean_data['cores'] > 32 or clean_data['cores'] < 1:
                        clean_data['cores'] = None
                
                existing = db.query(Smartphone).filter(
                    Smartphone.url == clean_data.get('url')
                ).first()
                
                if existing:
                    for key, value in clean_data.items():
                        if hasattr(existing, key) and value is not None:
                            setattr(existing, key, value)
                    updated_count += 1
                    status_msg = "обновлен"
                    item_name = clean_data.get('name', 'Неизвестно')
                else:
                    smartphone = Smartphone(**clean_data)
                    db.add(smartphone)
                    saved_count += 1
                    status_msg = "добавлен"
                    item_name = clean_data.get('name', 'Неизвестно')
                
                parsing_status["progress"] = idx
                parsing_status["current_item"] = item_name
                
                elapsed_time = time.time() - item_start_time
                print_parsed_item(idx, len(smartphones_data), item_name, elapsed_time)
                
            except Exception as e:
                print_error(f"Ошибка при сохранении {smartphone_data.get('name')}: {str(e)}")
                continue
        
        db.commit()
        
        total_in_db_before = db.query(Smartphone).count()
        total_save_time = time.time() - save_start_time
        
        print()
        print_header("НОРМАЛИЗАЦИЯ БАЗЫ ДАННЫХ", char="-")
        normalize_start_time = time.time()
        
        # Нормализация: удаление записей с отсутствующими критическими полями
        deleted_count = db.query(Smartphone).filter(
            or_(
                Smartphone.price.is_(None),
                Smartphone.price <= 0,
                Smartphone.screen_pixels.is_(None),
                Smartphone.ram.is_(None),
                Smartphone.camera_main.is_(None),
                Smartphone.cores.is_(None),
                Smartphone.storage.is_(None),
                Smartphone.storage <= 0
            )
        ).delete(synchronize_session=False)
        
        db.commit()
        normalize_time = time.time() - normalize_start_time
        
        total_in_db_after = db.query(Smartphone).count()
        
        print_success(f"Удалено невалидных записей: {deleted_count}")
        print_success(f"Время нормализации: {normalize_time:.2f}с")
        print()
        
        print_header("ПАРСИНГ ЗАВЕРШЕН УСПЕШНО", char="=")
        print_success(f"Добавлено новых: {saved_count}")
        print_success(f"Обновлено существующих: {updated_count}")
        print_success(f"Всего в базе до нормализации: {total_in_db_before}")
        print_success(f"Всего в базе после нормализации: {total_in_db_after}")
        print_success(f"Время сохранения в БД: {total_save_time:.2f}с")
        print_success(f"Время нормализации: {normalize_time:.2f}с")
        print()
        
        parsing_status["is_running"] = False
        parsing_status["progress"] = parsing_status["total"]
        
    except Exception as e:
        error_msg = f"Ошибка при парсинге: {str(e)}"
        print()
        print_header("ОШИБКА ПАРСИНГА", char="=", color=Fore.RED)
        print_error(error_msg)
        print()
        parsing_status["error"] = error_msg
        parsing_status["is_running"] = False
        raise


@router.post("/start")
async def start_parsing(
    background_tasks: BackgroundTasks,
    max_pages: int = None,
    use_async: bool = False,
    use_selenium: bool = True,
    use_page_all: bool = True,
    db: Session = Depends(get_db)
):
    """
    Запуск парсинга смартфонов с NIX.ru
    
    Args:
        max_pages: Максимальное количество товаров (если use_page_all=True) или страниц (если False)
        use_async: Использовать асинхронный парсер (НЕ работает с JS-пагинацией, по умолчанию False)
        use_selenium: Использовать Selenium парсер (работает с JS-пагинацией, рекомендуется, по умолчанию True)
        use_page_all: Использовать page=all для получения всех товаров сразу (работает только с Selenium, по умолчанию True)
    """
    global parsing_status
    
    if parsing_status["is_running"]:
        raise HTTPException(
            status_code=400,
            detail="Парсинг уже выполняется"
        )
    
    print_info(f"Запрос на запуск парсинга (max_pages={max_pages}, selenium={use_selenium}, page_all={use_page_all}, async={use_async})")
    
    background_tasks.add_task(parse_and_save, db, max_pages, use_async, use_selenium, use_page_all)
    
    mode = "selenium" if use_selenium else ("async" if use_async else "sync")
    if use_selenium and use_page_all:
        mode = "selenium (page=all)"
    return {
        "status": "started",
        "message": "Парсинг запущен",
        "max_pages": max_pages,
        "mode": mode
    }


@router.get("/status")
async def get_parsing_status():
    """Получить статус парсинга"""
    global parsing_status
    
    progress_percent = 0
    if parsing_status["total"] > 0:
        progress_percent = int((parsing_status["progress"] / parsing_status["total"]) * 100)
    
    return {
        "is_running": parsing_status["is_running"],
        "progress": parsing_status["progress"],
        "total": parsing_status["total"],
        "progress_percent": progress_percent,
        "current_item": parsing_status["current_item"],
        "error": parsing_status["error"]
    }


@router.get("/smartphones", response_model=List[SmartphoneResponse])
async def get_smartphones(
    skip: int = Query(default=0, ge=0),
    limit: int = Query(default=1000, ge=1, le=10000),
    min_price: Optional[int] = Query(default=None, ge=0),
    max_price: Optional[int] = Query(default=None, ge=0),
    min_ram: Optional[int] = Query(default=None, ge=0),
    min_storage: Optional[int] = Query(default=None, ge=0),
    brands: Optional[str] = Query(default=None, description="Список брендов через запятую"),
    sort_by: Optional[str] = Query(default=None, description="Поле для сортировки"),
    sort_order: Optional[str] = Query(default="asc", description="Порядок сортировки: asc или desc"),
    db: Session = Depends(get_db)
):
    """Получить список смартфонов с фильтрацией и сортировкой"""
    query = db.query(Smartphone)
    
    # Фильтры
    if min_price is not None:
        query = query.filter(Smartphone.price >= min_price)
    if max_price is not None:
        query = query.filter(Smartphone.price <= max_price)
    if min_ram is not None:
        query = query.filter(Smartphone.ram >= min_ram)
    if min_storage is not None:
        query = query.filter(Smartphone.storage >= min_storage)
    if brands:
        brand_list = [b.strip() for b in brands.split(',') if b.strip()]
        if brand_list:
            query = query.filter(Smartphone.brand.in_(brand_list))
    
    # Сортировка
    if sort_by and hasattr(Smartphone, sort_by):
        sort_column = getattr(Smartphone, sort_by)
        if sort_order == "desc":
            query = query.order_by(sort_column.desc())
        else:
            query = query.order_by(sort_column.asc())
    else:
        query = query.order_by(Smartphone.id.asc())
    
    # Пагинация
    smartphones = query.offset(skip).limit(limit).all()
    return smartphones


@router.get("/smartphones/count")
async def get_smartphones_count(db: Session = Depends(get_db)):
    """Получить количество смартфонов в базе"""
    count = db.query(Smartphone).count()
    return {"count": count}
