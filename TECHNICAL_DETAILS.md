# Технические детали реализации

## 1. Парсинг сайта NIX

### 1.1. Структура URL
- Каталог смартфонов: `https://www.nix.ru/price/price_list.html?section=smartphones_mobile_phones_all`
- Пагинация: `&page=1`, `&page=2`, и т.д.
- Карточка товара: `/autocatalog/smartphone_*/...` или `/2id.php?i=...`

### 1.2. Параметры для извлечения

**На странице списка:**
- Название товара (из ссылки)
- Цена от/до (из таблицы)
- Ссылка на детальную страницу

**На детальной странице товара:**
```python
SMARTPHONE_FIELDS = {
    'name': 'Название товара (из заголовка)',
    'brand': 'Производитель (из таблицы характеристик)',
    'model': 'Модель (из таблицы характеристик)',
    'price_min': 'Минимальная цена (из списка)',
    'price_max': 'Максимальная цена (из списка)',
    'screen_size': 'Экран телефона (дюймы)',
    'ram': 'Оперативная память (ГБ)',
    'storage': 'Объем встроенной памяти (ГБ)',
    'battery': 'Емкость аккумулятора (мАч)',
    'camera_main': 'Основная камера (МП)',
    'processor': 'Процессор (из характеристик)',
    'os_version': 'ОС (из характеристик)',
    'image_url': 'URL изображения',
    'url': 'Ссылка на товар'
}
```

### 1.3. Обработка данных парсинга

```python
def parse_price(price_text):
    """Извлечение цены из текста '99 990 ₽' -> 99990"""
    return int(re.sub(r'[^\d]', '', price_text))

def parse_screen_size(text):
    """Извлечение размера экрана '6.7"' -> 6.7"""
    match = re.search(r'(\d+\.?\d*)"', text)
    return float(match.group(1)) if match else None

def parse_memory(text):
    """Извлечение объема памяти '128 ГБ' -> 128"""
    match = re.search(r'(\d+)\s*ГБ', text)
    return int(match.group(1)) if match else None

def parse_battery(text):
    """Извлечение емкости батареи '5000 мАч' -> 5000"""
    match = re.search(r'(\d+)\s*мАч', text)
    return int(match.group(1)) if match else None

def parse_camera(text):
    """Извлечение МП камеры '48 МП' -> 48"""
    match = re.search(r'(\d+)\s*МП', text)
    return int(match.group(1)) if match else None
```

---

## 2. Алгоритм TOPSIS - детальная реализация

### 2.1. Математические формулы

**Шаг 1: Нормализация матрицы**
```
rij = xij / sqrt(Σ(i=1 to m) xij²)
```

**Шаг 2: Взвешивание**
```
vij = wj * rij
где wj - вес критерия j, Σwj = 1
```

**Шаг 3: Идеальные решения**
```
PIS = (v1+, v2+, ..., vn+)
где vj+ = max(vij) для критериев максимизации
     vj+ = min(vij) для критериев минимизации

NIS = (v1-, v2-, ..., vn-)
где vj- = min(vij) для критериев максимизации
     vj- = max(vij) для критериев минимизации
```

**Шаг 4: Расстояния**
```
Si+ = sqrt(Σ(j=1 to n) (vij - vj+)²)
Si- = sqrt(Σ(j=1 to n) (vij - vj-)²)
```

**Шаг 5: Относительная близость**
```
Ci = Si- / (Si+ + Si-)
где 0 ≤ Ci ≤ 1
```

### 2.2. Типы критериев

```python
CRITERIA_TYPES = {
    'price': 'min',           # Цена - минимизация
    'screen_size': 'max',     # Экран - максимизация
    'ram': 'max',             # RAM - максимизация
    'storage': 'max',         # Память - максимизация
    'battery': 'max',         # Батарея - максимизация
    'camera_main': 'max',     # Камера - максимизация
    'rating': 'max',          # Рейтинг - максимизация
    'processor_score': 'max'  # Процессор (конвертированный) - максимизация
}
```

### 2.3. Веса критериев (по умолчанию)

```python
DEFAULT_WEIGHTS = {
    'price': 0.15,           # 15%
    'screen_size': 0.10,     # 10%
    'ram': 0.15,             # 15%
    'storage': 0.15,         # 15%
    'battery': 0.15,         # 15%
    'camera_main': 0.10,     # 10%
    'rating': 0.10,          # 10%
    'processor_score': 0.10  # 10%
}
```

---

## 3. Структура API

### 3.1. Endpoints

```
GET  /api/smartphones/              # Список смартфонов (с фильтрами)
GET  /api/smartphones/{id}/          # Детали смартфона
GET  /api/smartphones/stats/         # Статистика данных
POST /api/parser/start/              # Запуск парсинга
GET  /api/parser/status/             # Статус парсинга
POST /api/analysis/topsis/calculate/ # Расчет TOPSIS
GET  /api/analysis/results/          # Получить результаты
GET  /api/analysis/data-stats/       # Статистика для анализа
```

### 3.2. Примеры запросов

**GET /api/smartphones/**
```json
{
  "brand": ["Samsung", "Apple"],
  "min_price": 20000,
  "max_price": 50000,
  "min_ram": 6,
  "sort_by": "price",
  "order": "asc",
  "page": 1,
  "page_size": 20
}
```

**POST /api/analysis/topsis/calculate/**
```json
{
  "weights": {
    "price": 0.20,
    "screen_size": 0.10,
    "ram": 0.15,
    "storage": 0.15,
    "battery": 0.15,
    "camera_main": 0.10,
    "rating": 0.10,
    "processor_score": 0.05
  },
  "smartphone_ids": null  # null = все смартфоны
}
```

**Response:**
```json
{
  "results": [
    {
      "smartphone_id": 1,
      "name": "Samsung Galaxy S23",
      "topsis_score": 0.875,
      "rank": 1,
      "distance_to_positive": 0.125,
      "distance_to_negative": 0.875
    },
    ...
  ],
  "best_alternative": {
    "smartphone_id": 1,
    "name": "Samsung Galaxy S23",
    "topsis_score": 0.875
  }
}
```

---

## 4. Frontend компоненты - детали

### 4.1. PhoneTable.vue

```vue
<template>
  <div class="overflow-x-auto">
    <table class="min-w-full">
      <thead>
        <tr>
          <th @click="sort('name')">Название</th>
          <th @click="sort('brand')">Бренд</th>
          <th @click="sort('price')">Цена</th>
          <th @click="sort('screen_size')">Экран</th>
          <th @click="sort('ram')">RAM</th>
          <th @click="sort('storage')">Память</th>
          <th @click="sort('battery')">Батарея</th>
          <th @click="sort('camera_main')">Камера</th>
          <th @click="sort('rating')">Рейтинг</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="phone in phones" :key="phone.id">
          <td>{{ phone.name }}</td>
          <td>{{ phone.brand }}</td>
          <td>{{ formatPrice(phone.price) }}</td>
          <!-- ... -->
        </tr>
      </tbody>
    </table>
  </div>
</template>
```

### 4.2. FiltersPanel.vue

```vue
<template>
  <div class="bg-white p-4 rounded-lg shadow">
    <h3 class="text-lg font-bold mb-4">Фильтры</h3>
    
    <!-- Бренды -->
    <div class="mb-4">
      <label>Бренды</label>
      <select multiple v-model="filters.brands">
        <option v-for="brand in availableBrands" :value="brand">
          {{ brand }}
        </option>
      </select>
    </div>
    
    <!-- Цена -->
    <div class="mb-4">
      <label>Цена: {{ filters.minPrice }} - {{ filters.maxPrice }} ₽</label>
      <input type="range" v-model="filters.minPrice" min="0" max="200000" />
      <input type="range" v-model="filters.maxPrice" min="0" max="200000" />
    </div>
    
    <!-- RAM -->
    <div class="mb-4">
      <label>RAM: {{ filters.minRam }} - {{ filters.maxRam }} ГБ</label>
      <input type="range" v-model="filters.minRam" min="2" max="16" />
      <input type="range" v-model="filters.maxRam" min="2" max="16" />
    </div>
    
    <button @click="applyFilters" class="btn-primary">Применить</button>
    <button @click="resetFilters" class="btn-secondary">Сбросить</button>
  </div>
</template>
```

### 4.3. ChartsSection.vue

**График 1: Распределение цен (Chart.js)**
```javascript
const priceChart = {
  type: 'histogram',
  data: {
    labels: priceRanges,
    datasets: [{
      label: 'Количество смартфонов',
      data: priceDistribution,
      backgroundColor: 'rgba(59, 130, 246, 0.5)'
    }]
  }
}
```

**График 2: Зависимость цены от характеристик**
```javascript
const scatterChart = {
  type: 'scatter',
  data: {
    datasets: [{
      label: 'Цена vs RAM',
      data: phones.map(p => ({x: p.ram, y: p.price}))
    }]
  }
}
```

**График 3: Сравнение топ-10 (Radar)**
```javascript
const radarChart = {
  type: 'radar',
  data: {
    labels: ['Цена', 'Экран', 'RAM', 'Память', 'Батарея', 'Камера', 'Рейтинг'],
    datasets: top10Phones.map(phone => ({
      label: phone.name,
      data: [
        normalize(phone.price, 'price'),
        normalize(phone.screen_size, 'screen'),
        normalize(phone.ram, 'ram'),
        // ...
      ]
    }))
  }
}
```

---

## 5. Обработка качественных параметров

### 5.1. Процессор → числовой рейтинг

```python
PROCESSOR_SCORES = {
    'Snapdragon 8 Gen 2': 100,
    'Snapdragon 8 Gen 1': 90,
    'Snapdragon 888': 85,
    'Snapdragon 870': 80,
    'Snapdragon 778G': 70,
    'Apple A16 Bionic': 100,
    'Apple A15 Bionic': 95,
    'Apple A14 Bionic': 85,
    'MediaTek Dimensity 9000': 90,
    'MediaTek Dimensity 8100': 75,
    'Exynos 2200': 85,
    # ... и т.д.
}

def convert_processor_to_score(processor_name):
    """Конвертация названия процессора в числовой рейтинг"""
    for proc, score in PROCESSOR_SCORES.items():
        if proc.lower() in processor_name.lower():
            return score
    return 50  # Средний рейтинг по умолчанию
```

### 5.2. ОС → числовой рейтинг

```python
OS_SCORES = {
    'Android 13': 100,
    'Android 12': 90,
    'Android 11': 80,
    'iOS 16': 100,
    'iOS 15': 90,
    'iOS 14': 80,
}
```

---

## 6. Обработка пропущенных значений

```python
def handle_missing_values(df):
    """Заполнение пропущенных значений"""
    # Для числовых полей - медиана
    numeric_cols = ['price', 'screen_size', 'ram', 'storage', 'battery', 'camera_main']
    for col in numeric_cols:
        df[col].fillna(df[col].median(), inplace=True)
    
    # Для категориальных - мода
    categorical_cols = ['processor', 'os_version']
    for col in categorical_cols:
        df[col].fillna(df[col].mode()[0] if not df[col].mode().empty else 'Unknown', inplace=True)
    
    return df
```

---

## 7. Валидация данных

```python
def validate_smartphone_data(data):
    """Валидация данных смартфона"""
    errors = []
    
    if not data.get('name'):
        errors.append('Отсутствует название')
    
    if not data.get('price') or data['price'] <= 0:
        errors.append('Некорректная цена')
    
    if data.get('ram') and (data['ram'] < 2 or data['ram'] > 32):
        errors.append('Некорректный объем RAM')
    
    if data.get('screen_size') and (data['screen_size'] < 4 or data['screen_size'] > 8):
        errors.append('Некорректный размер экрана')
    
    return errors
```

---

## 8. Оптимизация производительности

### 8.1. Индексы базы данных

```python
class Smartphone(models.Model):
    # ...
    
    class Meta:
        indexes = [
            models.Index(fields=['brand']),
            models.Index(fields=['price']),
            models.Index(fields=['ram']),
            models.Index(fields=['rating']),
        ]
```

### 8.2. Кэширование

```python
from django.core.cache import cache

def get_smartphones_cached(filters):
    cache_key = f"smartphones_{hash(str(filters))}"
    result = cache.get(cache_key)
    if result is None:
        result = Smartphone.objects.filter(**filters)
        cache.set(cache_key, result, timeout=300)  # 5 минут
    return result
```

---

## 9. Безопасность

### 9.1. Защита от перегрузки парсинга

```python
from django.core.cache import cache

def start_parsing(request):
    # Проверка, не запущен ли уже парсинг
    if cache.get('parsing_in_progress'):
        return Response({'error': 'Парсинг уже выполняется'}, status=400)
    
    cache.set('parsing_in_progress', True, timeout=3600)  # 1 час
    # Запуск задачи
```

### 9.2. Валидация входных данных API

```python
from rest_framework import serializers

class TopsisCalculateSerializer(serializers.Serializer):
    weights = serializers.DictField(
        child=serializers.FloatField(min_value=0, max_value=1)
    )
    
    def validate_weights(self, value):
        total = sum(value.values())
        if abs(total - 1.0) > 0.01:
            raise serializers.ValidationError("Сумма весов должна быть равна 1")
        return value
```

---

## 10. Логирование

```python
import logging

logger = logging.getLogger('parser')

def parse_dns_smartphones():
    logger.info('Начало парсинга DNS')
    try:
        # Парсинг
        logger.info(f'Успешно спарсено {count} смартфонов')
    except Exception as e:
        logger.error(f'Ошибка парсинга: {str(e)}', exc_info=True)
```

