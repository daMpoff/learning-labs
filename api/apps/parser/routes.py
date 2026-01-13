"""
API роутеры для парсинга
"""
import logging
from fastapi import APIRouter, BackgroundTasks, Depends, HTTPException
from sqlalchemy.orm import Session
from typing import List

from database import get_db
from apps.parser.models import Smartphone
from apps.parser.schemas import SmartphoneResponse, SmartphoneCreate
from apps.parser.parsers.nix_parser import NixParser
from apps.parser.parsers.nix_parser_async import NixParserAsync
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

# Опциональный импорт colorama
try:
    from colorama import Fore
except ImportError:
    class Fore:
        RED = ''
        GREEN = ''
        YELLOW = ''
        BLUE = ''
        CYAN = ''

# Настройка цветного логирования
logger = setup_colored_logger(__name__, logging.INFO)

router = APIRouter()

# Глобальная переменная для отслеживания статуса парсинга
parsing_status = {
    "is_running": False,
    "progress": 0,
    "total": 0,
    "current_item": "",
    "error": None
}


def save_smartphone_to_db(db: Session, smartphone_data: dict) -> Smartphone:
    """Сохранение смартфона в БД"""
    # Удаляем служебные поля, которых нет в модели
    clean_data = {k: v for k, v in smartphone_data.items() 
                 if k != 'is_smartphone' and hasattr(Smartphone, k)}
    
    # Проверяем, существует ли уже смартфон с таким URL
    existing = db.query(Smartphone).filter(Smartphone.url == clean_data.get('url')).first()
    
    if existing:
        # Обновляем существующий
        for key, value in clean_data.items():
            if hasattr(existing, key) and value is not None:
                setattr(existing, key, value)
        db.commit()
        db.refresh(existing)
        logger.info(f"Обновлен смартфон: {existing.name}")
        return existing
    else:
        # Создаем новый
        smartphone = Smartphone(**clean_data)
        db.add(smartphone)
        db.commit()
        db.refresh(smartphone)
        logger.info(f"Добавлен новый смартфон: {smartphone.name}")
        return smartphone


def parse_and_save(db: Session, max_pages: int = None, use_async: bool = True):
    """Функция парсинга и сохранения в БД
    
    Args:
        db: Сессия базы данных
        max_pages: Максимальное количество страниц
        use_async: Использовать асинхронный парсер (быстрее)
    """
    global parsing_status
    
    try:
        parsing_status["is_running"] = True
        parsing_status["error"] = None
        parsing_status["progress"] = 0
        parsing_status["total"] = 0
        
        # Красивый заголовок
        print_header("НАЧАЛО ПАРСИНГА СМАРТФОНОВ С NIX.RU")
        print_info(f"Режим: {'АСИНХРОННЫЙ (быстрый)' if use_async else 'СИНХРОННЫЙ'}")
        print_info(f"Максимальное количество страниц: {max_pages or 'все'}")
        print()
        
        if use_async:
            # Асинхронный парсер (быстрее в 5-10 раз)
            parser = NixParserAsync(delay_min=0.3, delay_max=0.8, max_concurrent=15)
            # Запускаем асинхронную функцию
            smartphones_data = asyncio.run(parser.parse_all_smartphones(max_pages=max_pages))
        else:
            # Синхронный парсер (медленнее, но стабильнее)
            parser = NixParser(delay_min=1, delay_max=3)
            smartphones_data = parser.parse_all_smartphones(max_pages=max_pages)
        
        parsing_status["total"] = len(smartphones_data)
        print_success(f"Всего получено смартфонов: {len(smartphones_data)}")
        print()
        
        # Сохранение в БД
        print_header("СОХРАНЕНИЕ В БАЗУ ДАННЫХ", char="-")
        saved_count = 0
        updated_count = 0
        
        import time
        save_start_time = time.time()
        
        for idx, smartphone_data in enumerate(smartphones_data, 1):
            item_start_time = time.time()
            try:
                # Удаляем служебные поля, которых нет в модели
                clean_data = {k: v for k, v in smartphone_data.items() 
                             if k != 'is_smartphone' and hasattr(Smartphone, k)}
                
                # Валидация и ограничение числовых полей
                # Цена должна быть в разумных пределах (до 50 млн рублей)
                if 'price' in clean_data and clean_data['price']:
                    if clean_data['price'] > 50_000_000 or clean_data['price'] < 0:
                        logger.warning(f"Пропуск товара {clean_data.get('name')} - недопустимая цена: {clean_data['price']}")
                        continue
                
                if 'price_min' in clean_data and clean_data['price_min']:
                    if clean_data['price_min'] > 50_000_000 or clean_data['price_min'] < 0:
                        clean_data['price_min'] = None
                
                if 'price_max' in clean_data and clean_data['price_max']:
                    if clean_data['price_max'] > 50_000_000 or clean_data['price_max'] < 0:
                        clean_data['price_max'] = None
                
                # Валидация других числовых полей
                # RAM: до 64 ГБ (разумный максимум)
                if 'ram' in clean_data and clean_data['ram']:
                    if clean_data['ram'] > 64 or clean_data['ram'] < 0:
                        clean_data['ram'] = None
                
                # Storage: до 2 ТБ (2048 ГБ)
                if 'storage' in clean_data and clean_data['storage']:
                    if clean_data['storage'] > 2048 or clean_data['storage'] < 0:
                        clean_data['storage'] = None
                
                # Battery: до 20000 мАч (разумный максимум)
                if 'battery' in clean_data and clean_data['battery']:
                    if clean_data['battery'] > 20000 or clean_data['battery'] < 0:
                        clean_data['battery'] = None
                
                # Camera: до 500 МП (разумный максимум)
                if 'camera_main' in clean_data and clean_data['camera_main']:
                    if clean_data['camera_main'] > 500 or clean_data['camera_main'] < 0:
                        clean_data['camera_main'] = None
                
                # Screen size: до 10 дюймов (разумный максимум для смартфона)
                if 'screen_size' in clean_data and clean_data['screen_size']:
                    if clean_data['screen_size'] > 10.0 or clean_data['screen_size'] < 0:
                        clean_data['screen_size'] = None
                
                # Проверяем наличие перед сохранением
                existing = db.query(Smartphone).filter(
                    Smartphone.url == clean_data.get('url')
                ).first()
                
                if existing:
                    # Обновляем
                    for key, value in clean_data.items():
                        if hasattr(existing, key) and value is not None:
                            setattr(existing, key, value)
                    updated_count += 1
                    status_msg = "обновлен"
                    item_name = clean_data.get('name', 'Неизвестно')
                else:
                    # Создаем новый
                    smartphone = Smartphone(**clean_data)
                    db.add(smartphone)
                    saved_count += 1
                    status_msg = "добавлен"
                    item_name = clean_data.get('name', 'Неизвестно')
                
                parsing_status["progress"] = idx
                parsing_status["current_item"] = item_name
                
                # Выводим каждый товар с временем обработки
                elapsed_time = time.time() - item_start_time
                print_parsed_item(idx, len(smartphones_data), item_name, elapsed_time)
                
            except Exception as e:
                print_error(f"Ошибка при сохранении {smartphone_data.get('name')}: {str(e)}")
                continue
        
        # Коммитим все изменения
        db.commit()
        
        total_in_db = db.query(Smartphone).count()
        total_save_time = time.time() - save_start_time
        
        # Красивый итоговый отчет
        print()
        print_header("ПАРСИНГ ЗАВЕРШЕН УСПЕШНО", char="=")
        print_success(f"Добавлено новых: {saved_count}")
        print_success(f"Обновлено существующих: {updated_count}")
        print_success(f"Всего в базе: {total_in_db}")
        print_success(f"Время сохранения в БД: {total_save_time:.2f}с")
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
    use_async: bool = True,
    db: Session = Depends(get_db)
):
    """
    Запуск парсинга смартфонов с NIX.ru
    
    Args:
        max_pages: Максимальное количество страниц для парсинга (None = все страницы)
        use_async: Использовать асинхронный парсер (True = быстрее в 5-10 раз, рекомендуется)
    """
    global parsing_status
    
    if parsing_status["is_running"]:
        raise HTTPException(
            status_code=400,
            detail="Парсинг уже выполняется"
        )
    
    print_info(f"Запрос на запуск парсинга (max_pages={max_pages}, async={use_async})")
    
    # Запускаем парсинг в фоне
    background_tasks.add_task(parse_and_save, db, max_pages, use_async)
    
    return {
        "status": "started",
        "message": "Парсинг запущен",
        "max_pages": max_pages,
        "mode": "async (fast)" if use_async else "sync (slow)"
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
    skip: int = 0,
    limit: int = 100,
    db: Session = Depends(get_db)
):
    """Получить список смартфонов"""
    smartphones = db.query(Smartphone).offset(skip).limit(limit).all()
    return smartphones


@router.get("/smartphones/count")
async def get_smartphones_count(db: Session = Depends(get_db)):
    """Получить количество смартфонов в базе"""
    count = db.query(Smartphone).count()
    return {"count": count}

