"""
Скрипт для нормализации базы данных
Удаляет записи с отсутствующими обязательными полями
"""
import sys
from sqlalchemy import and_, or_
from database import SessionLocal, engine
from apps.parser.models import Smartphone, Base

def normalize_database():
    """Удаляет записи с отсутствующими обязательными полями"""
    db = SessionLocal()
    
    try:
        # Подсчет записей до очистки
        total_before = db.query(Smartphone).count()
        print(f"Всего записей в базе до очистки: {total_before}")
        
        # Находим записи с отсутствующими обязательными полями
        # price - NOT NULL, но проверим на <= 0
        # screen_pixels, ram, camera_main, cores, storage - могут быть NULL
        invalid_records = db.query(Smartphone).filter(
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
        ).all()
        
        invalid_count = len(invalid_records)
        print(f"Найдено записей с отсутствующими полями: {invalid_count}")
        
        if invalid_count > 0:
            # Удаляем записи
            for record in invalid_records:
                print(f"Удаление записи ID {record.id}: {record.name[:50]}...")
                print(f"  price={record.price}, screen_pixels={record.screen_pixels}, "
                      f"ram={record.ram}, camera_main={record.camera_main}, cores={record.cores}, "
                      f"storage={record.storage}")
                db.delete(record)
            
            db.commit()
            print(f"\nУдалено {invalid_count} записей")
        else:
            print("Нет записей для удаления")
        
        # Подсчет записей после очистки
        total_after = db.query(Smartphone).count()
        print(f"\nВсего записей в базе после очистки: {total_after}")
        print(f"Удалено: {total_before - total_after} записей")
        
    except Exception as e:
        db.rollback()
        print(f"Ошибка при нормализации базы данных: {e}")
        sys.exit(1)
    finally:
        db.close()

if __name__ == "__main__":
    print("=" * 60)
    print("НОРМАЛИЗАЦИЯ БАЗЫ ДАННЫХ")
    print("=" * 60)
    print("\nУдаление записей с отсутствующими обязательными полями:")
    print("  - price (должно быть > 0)")
    print("  - screen_pixels (не должно быть NULL)")
    print("  - ram (не должно быть NULL)")
    print("  - camera_main (не должно быть NULL)")
    print("  - cores (не должно быть NULL)")
    print("  - storage (должно быть > 0)")
    print("=" * 60)
    print()
    
    response = input("Продолжить? (yes/no): ").strip().lower()
    if response in ['yes', 'y', 'да', 'д']:
        normalize_database()
    else:
        print("Операция отменена")
