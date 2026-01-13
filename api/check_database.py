"""
Скрипт для проверки базы данных смартфонов
"""
import sqlite3

conn = sqlite3.connect('smartphones.db')
cursor = conn.cursor()

print("=" * 60)
print("ПРОВЕРКА БАЗЫ ДАННЫХ СМАРТФОНОВ")
print("=" * 60)

# Общая статистика
cursor.execute('SELECT COUNT(*) FROM smartphones')
total = cursor.fetchone()[0]
print(f"\n📊 ОБЩАЯ СТАТИСТИКА:")
print(f"   Всего записей: {total}")

# Статистика цен
cursor.execute('SELECT MIN(price), MAX(price), AVG(price), COUNT(*) FROM smartphones WHERE price > 0')
stats = cursor.fetchone()
print(f"\n💰 СТАТИСТИКА ЦЕН:")
print(f"   Минимальная цена: {stats[0]:,} руб")
print(f"   Максимальная цена: {stats[1]:,} руб")
print(f"   Средняя цена: {stats[2]:,.2f} руб")
print(f"   Записей с ценой: {stats[3]}")

# Проблемные записи
cursor.execute('SELECT COUNT(*) FROM smartphones WHERE price_max < price_min AND price_max > 0 AND price_min > 0')
wrong_order = cursor.fetchone()[0]
print(f"\n⚠️  ПРОБЛЕМНЫЕ ЗАПИСИ:")
print(f"   Записей где price_max < price_min: {wrong_order}")

cursor.execute('SELECT COUNT(*) FROM smartphones WHERE price < 1000 AND price > 0')
too_low = cursor.fetchone()[0]
print(f"   Записей с ценой < 1000 руб: {too_low}")

cursor.execute('SELECT COUNT(*) FROM smartphones WHERE price > 500000')
too_high = cursor.fetchone()[0]
print(f"   Записей с ценой > 500000 руб: {too_high}")

cursor.execute('SELECT COUNT(*) FROM smartphones WHERE price IS NULL OR price = 0')
no_price = cursor.fetchone()[0]
print(f"   Записей без цены: {no_price}")

# Заполненность полей
print(f"\n📋 ЗАПОЛНЕННОСТЬ ПОЛЕЙ:")
cursor.execute('SELECT COUNT(*) FROM smartphones WHERE brand IS NOT NULL AND brand != ""')
brand_count = cursor.fetchone()[0]
print(f"   Бренд: {brand_count}/{total} ({brand_count/total*100:.1f}%)")

cursor.execute('SELECT COUNT(*) FROM smartphones WHERE model IS NOT NULL AND model != ""')
model_count = cursor.fetchone()[0]
print(f"   Модель: {model_count}/{total} ({model_count/total*100:.1f}%)")

cursor.execute('SELECT COUNT(*) FROM smartphones WHERE screen_size IS NOT NULL')
screen_count = cursor.fetchone()[0]
print(f"   Размер экрана: {screen_count}/{total} ({screen_count/total*100:.1f}%)")

cursor.execute('SELECT COUNT(*) FROM smartphones WHERE ram IS NOT NULL')
ram_count = cursor.fetchone()[0]
print(f"   RAM: {ram_count}/{total} ({ram_count/total*100:.1f}%)")

cursor.execute('SELECT COUNT(*) FROM smartphones WHERE storage IS NOT NULL')
storage_count = cursor.fetchone()[0]
print(f"   Память: {storage_count}/{total} ({storage_count/total*100:.1f}%)")

cursor.execute('SELECT COUNT(*) FROM smartphones WHERE battery IS NOT NULL')
battery_count = cursor.fetchone()[0]
print(f"   Батарея: {battery_count}/{total} ({battery_count/total*100:.1f}%)")

cursor.execute('SELECT COUNT(*) FROM smartphones WHERE camera_main IS NOT NULL')
camera_count = cursor.fetchone()[0]
print(f"   Камера: {camera_count}/{total} ({camera_count/total*100:.1f}%)")

cursor.execute('SELECT COUNT(*) FROM smartphones WHERE image_url IS NOT NULL AND image_url != ""')
image_count = cursor.fetchone()[0]
print(f"   Изображения: {image_count}/{total} ({image_count/total*100:.1f}%)")

# Топ брендов
cursor.execute('SELECT brand, COUNT(*) as cnt FROM smartphones WHERE brand IS NOT NULL GROUP BY brand ORDER BY cnt DESC LIMIT 10')
rows = cursor.fetchall()
print(f"\n🏷️  ТОП-10 БРЕНДОВ:")
for brand, count in rows:
    print(f"   {brand}: {count} шт")

# Примеры проблемных записей
if wrong_order > 0:
    cursor.execute('SELECT id, name, price, price_min, price_max FROM smartphones WHERE price_max < price_min AND price_max > 0 AND price_min > 0 LIMIT 5')
    rows = cursor.fetchall()
    print(f"\n❌ ПРИМЕРЫ ПРОБЛЕМНЫХ ЗАПИСЕЙ (price_max < price_min):")
    for r in rows:
        print(f"   ID {r[0]}: price={r[2]}, min={r[3]}, max={r[4]} | {r[1][:50]}...")

if too_low > 0:
    cursor.execute('SELECT id, name, price FROM smartphones WHERE price < 1000 AND price > 0 LIMIT 5')
    rows = cursor.fetchall()
    print(f"\n❌ ПРИМЕРЫ ЗАПИСЕЙ С НИЗКОЙ ЦЕНОЙ (< 1000 руб):")
    for r in rows:
        print(f"   ID {r[0]}: price={r[2]} руб | {r[1][:60]}...")

# Проверка диапазонов цен
cursor.execute('SELECT COUNT(*) FROM smartphones WHERE price_min IS NOT NULL AND price_max IS NOT NULL AND price_min > 0 AND price_max > 0')
has_range = cursor.fetchone()[0]
print(f"\n📈 ДИАПАЗОНЫ ЦЕН:")
print(f"   Записей с диапазоном цен (min-max): {has_range}/{total}")

if has_range > 0:
    cursor.execute('SELECT AVG(price_max - price_min) FROM smartphones WHERE price_min IS NOT NULL AND price_max IS NOT NULL AND price_min > 0 AND price_max > 0')
    avg_diff = cursor.fetchone()[0]
    print(f"   Средняя разница между max и min: {avg_diff:,.2f} руб")

# Проверка обязательных полей
cursor.execute('SELECT COUNT(*) FROM smartphones WHERE url IS NULL OR url = ""')
no_url = cursor.fetchone()[0]
print(f"\n✅ ОБЯЗАТЕЛЬНЫЕ ПОЛЯ:")
print(f"   Записей без URL: {no_url}")

cursor.execute('SELECT COUNT(*) FROM smartphones WHERE name IS NULL OR name = ""')
no_name = cursor.fetchone()[0]
print(f"   Записей без названия: {no_name}")

print("\n" + "=" * 60)
print("ПРОВЕРКА ЗАВЕРШЕНА")
print("=" * 60)

conn.close()

