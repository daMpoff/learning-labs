# Требования к данным и структура

## 1. Минимальные требования

- **Количество записей**: 200-500 смартфонов
- **Количество параметров**: минимум 7
- **Источник**: сайт NIX (https://www.nix.ru)

## 2. Обязательные параметры смартфона

### 2.1. Количественные параметры (для TOPSIS)

| Параметр | Тип | Единица измерения | Критерий TOPSIS | Описание |
|----------|-----|-------------------|-----------------|----------|
| `price` | Decimal | Рубли | MIN | Цена смартфона |
| `screen_size` | Float | Дюймы | MAX | Диагональ экрана |
| `ram` | Integer | ГБ | MAX | Объем оперативной памяти |
| `storage` | Integer | ГБ | MAX | Объем встроенной памяти |
| `battery` | Integer | мАч | MAX | Емкость аккумулятора |
| `camera_main` | Integer | МП | MAX | Основная камера (мегапиксели) |
| `rating` | Float | Баллы (0-5) | MAX | Рейтинг товара на сайте |

### 2.2. Качественные параметры (конвертируются в количественные)

| Параметр | Тип | Конвертация | Описание |
|----------|-----|-------------|----------|
| `processor` | String | → `processor_score` (0-100) | Название процессора |
| `os_version` | String | → `os_score` (0-100) | Версия операционной системы |
| `brand` | String | → `brand_popularity` (0-100) | Бренд смартфона |

### 2.3. Дополнительные параметры (для отображения)

| Параметр | Тип | Описание |
|----------|-----|----------|
| `name` | String | Полное название смартфона |
| `brand` | String | Бренд |
| `url` | URL | Ссылка на страницу товара |
| `image_url` | URL | URL изображения смартфона |
| `parsed_at` | DateTime | Дата и время парсинга |

## 3. Пример данных

### 3.1. Пример записи в базе данных

```json
{
  "id": 1,
  "name": "Samsung Galaxy S23 Ultra 256GB",
  "brand": "Samsung",
  "price": 89990,
  "screen_size": 6.8,
  "ram": 12,
  "storage": 256,
  "battery": 5000,
  "camera_main": 200,
  "processor": "Snapdragon 8 Gen 2",
  "processor_score": 100,
  "os_version": "Android 13",
  "os_score": 100,
  "rating": 4.8,
  "url": "https://www.dns-shop.ru/product/...",
  "image_url": "https://www.dns-shop.ru/upload/...",
  "parsed_at": "2024-01-15T10:30:00Z"
}
```

### 3.2. Пример данных для расчета TOPSIS

```python
# Матрица решений (5 смартфонов × 8 критериев)
decision_matrix = [
    # price, screen, ram, storage, battery, camera, rating, processor
    [89990, 6.8, 12, 256, 5000, 200, 4.8, 100],  # Samsung S23 Ultra
    [79990, 6.7, 8, 128, 4500, 48, 4.6, 90],     # iPhone 14 Pro
    [49990, 6.6, 8, 128, 4000, 64, 4.5, 75],     # Xiaomi 13
    [34990, 6.4, 6, 128, 5000, 50, 4.3, 70],     # Realme 10 Pro
    [24990, 6.1, 4, 64, 3000, 48, 4.0, 60],      # Samsung A54
]

# Типы критериев
criteria_types = ['min', 'max', 'max', 'max', 'max', 'max', 'max', 'max']

# Веса критериев
weights = [0.15, 0.10, 0.15, 0.15, 0.15, 0.10, 0.10, 0.10]
```

## 4. Обработка пропущенных значений

### 4.1. Стратегии заполнения

| Параметр | Стратегия | Причина |
|----------|-----------|---------|
| `price` | Удаление записи | Критический параметр |
| `screen_size` | Медиана по бренду | Зависит от бренда |
| `ram` | Медиана по ценовому сегменту | Зависит от цены |
| `storage` | Медиана по ценовому сегменту | Зависит от цены |
| `battery` | Медиана всех записей | Универсальный параметр |
| `camera_main` | Медиана по бренду | Зависит от бренда |
| `rating` | Среднее значение (3.5) | Если нет рейтинга |
| `processor` | "Unknown" → score=50 | Средний рейтинг |

### 4.2. Обработка выбросов

```python
def remove_outliers(df):
    """Удаление выбросов по методу IQR"""
    numeric_cols = ['price', 'screen_size', 'ram', 'storage', 'battery']
    
    for col in numeric_cols:
        Q1 = df[col].quantile(0.25)
        Q3 = df[col].quantile(0.75)
        IQR = Q3 - Q1
        lower_bound = Q1 - 1.5 * IQR
        upper_bound = Q3 + 1.5 * IQR
        
        df = df[(df[col] >= lower_bound) & (df[col] <= upper_bound)]
    
    return df
```

## 5. Нормализация данных

### 5.1. Min-Max нормализация (для визуализации)

```python
def min_max_normalize(value, min_val, max_val):
    """Нормализация в диапазон [0, 1]"""
    if max_val == min_val:
        return 0.5
    return (value - min_val) / (max_val - min_val)
```

### 5.2. Нормализация для TOPSIS

```python
def topsis_normalize(matrix):
    """Векторная нормализация для TOPSIS"""
    normalized = []
    for j in range(len(matrix[0])):
        column = [row[j] for row in matrix]
        norm = math.sqrt(sum(x**2 for x in column))
        normalized_col = [x / norm if norm != 0 else 0 for x in column]
        normalized.append(normalized_col)
    
    # Транспонирование
    return [[normalized[j][i] for j in range(len(normalized))] 
            for i in range(len(normalized[0]))]
```

## 6. Валидация данных после парсинга

### 6.1. Проверки качества данных

```python
def validate_parsed_data(smartphones):
    """Валидация спарсенных данных"""
    issues = []
    
    # Проверка количества
    if len(smartphones) < 200:
        issues.append(f"Недостаточно данных: {len(smartphones)} < 200")
    
    # Проверка обязательных полей
    required_fields = ['name', 'price', 'ram', 'storage', 'battery']
    for phone in smartphones:
        for field in required_fields:
            if not phone.get(field):
                issues.append(f"Отсутствует поле {field} для {phone.get('name')}")
    
    # Проверка диапазонов
    for phone in smartphones:
        if phone.get('price', 0) < 5000 or phone.get('price', 0) > 200000:
            issues.append(f"Некорректная цена: {phone.get('price')}")
        
        if phone.get('ram', 0) < 2 or phone.get('ram', 0) > 32:
            issues.append(f"Некорректный RAM: {phone.get('ram')}")
        
        if phone.get('screen_size', 0) < 4 or phone.get('screen_size', 0) > 8:
            issues.append(f"Некорректный размер экрана: {phone.get('screen_size')}")
    
    return issues
```

## 7. Статистика данных

### 7.1. Базовая статистика

```python
def get_data_statistics(smartphones):
    """Получение статистики по данным"""
    df = pd.DataFrame(smartphones)
    
    stats = {
        'total_count': len(smartphones),
        'brands_count': df['brand'].nunique(),
        'price': {
            'min': df['price'].min(),
            'max': df['price'].max(),
            'mean': df['price'].mean(),
            'median': df['price'].median()
        },
        'ram_distribution': df['ram'].value_counts().to_dict(),
        'storage_distribution': df['storage'].value_counts().to_dict(),
        'missing_values': df.isnull().sum().to_dict()
    }
    
    return stats
```

## 8. Примеры фильтрации

### 8.1. Фильтр по бренду
```python
filtered = smartphones.filter(brand__in=['Samsung', 'Apple', 'Xiaomi'])
```

### 8.2. Фильтр по цене
```python
filtered = smartphones.filter(price__gte=20000, price__lte=50000)
```

### 8.3. Фильтр по характеристикам
```python
filtered = smartphones.filter(
    ram__gte=6,
    storage__gte=128,
    battery__gte=4000
)
```

### 8.4. Комбинированный фильтр
```python
filtered = smartphones.filter(
    brand__in=['Samsung', 'Apple'],
    price__gte=30000,
    price__lte=80000,
    ram__gte=8,
    storage__gte=128,
    rating__gte=4.0
)
```

## 9. Примеры сортировки

### 9.1. Сортировка по одному параметру
```python
# По возрастанию цены
sorted_phones = smartphones.order_by('price')

# По убыванию рейтинга
sorted_phones = smartphones.order_by('-rating')
```

### 9.2. Множественная сортировка
```python
# Сначала по бренду, потом по цене
sorted_phones = smartphones.order_by('brand', 'price')
```

## 10. Экспорт данных

### 10.1. Экспорт в CSV
```python
import pandas as pd

df = pd.DataFrame(list(smartphones.values()))
df.to_csv('smartphones.csv', index=False, encoding='utf-8-sig')
```

### 10.2. Экспорт в JSON
```python
import json

data = list(smartphones.values())
with open('smartphones.json', 'w', encoding='utf-8') as f:
    json.dump(data, f, ensure_ascii=False, indent=2, default=str)
```

