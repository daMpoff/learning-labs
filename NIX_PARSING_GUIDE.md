# Руководство по парсингу сайта NIX

## Структура сайта

### 1. Страница списка смартфонов

**URL для парсинга только смартфонов:**
```
https://www.nix.ru/price/price_list.html?section=smartphones_mobile_phones_all&enums[4843][]=2&page=all
```

**Параметры URL:**
- `section=smartphones_mobile_phones_all` - раздел смартфонов
- `enums[4843][]=2` - фильтр: только смартфоны (исключает кнопочные телефоны)
- `page=all` - все страницы сразу (или `page=1`, `page=2`, ... для пагинации)
- `sort=...` - сортировка
- `store=msk-0_1721_1` - магазин (опционально)
- `thumbnail_view=2` - вид отображения

**Важно:** Параметр `enums[4843][]=2` фильтрует только смартфоны, исключая кнопочные телефоны.

**Структура таблицы:**
- Колонка 1: Название товара (ссылка на детальную страницу)
- Колонка 2: Наличие (дата или "ПОСЛЕЗАВТРА")
- Колонка 3: Чекбокс для сравнения
- Колонка 4: Цена от (руб)
- Колонка 5: Цена до (руб)

**Пример HTML структуры:**
```html
<table>
  <tr>
    <td>
      <a href="/autocatalog/smartphone_bq/...">+ Смартфон ...</a>
    </td>
    <td>ПОСЛЕЗАВТРА</td>
    <td><input type="checkbox"></td>
    <td>4 261</td>
    <td>4 616</td>
  </tr>
</table>
```

### 2. Детальная страница товара

**URL формат:**
- `/autocatalog/smartphone_*/...` - для большинства товаров
- `/2id.php?i=...` - для некоторых товаров

**Характеристики находятся в таблицах:**

Основные характеристики:
- Производитель
- Модель
- Тип оборудования
- Экран телефона (размер в дюймах)
- Разрешение матрицы экрана
- Оперативная память (RAM)
- Объем встроенной памяти
- Емкость аккумулятора
- Основная камера
- Процессор
- ОС

**Пример извлечения характеристик:**

```python
def parse_specs_table(soup):
    """Извлечение характеристик из таблицы"""
    specs = {}
    tables = soup.find_all('table')
    
    for table in tables:
        rows = table.find_all('tr')
        for row in rows:
            cells = row.find_all(['td', 'th'])
            if len(cells) >= 2:
                key = cells[0].get_text(strip=True)
                value = cells[1].get_text(strip=True)
                specs[key] = value
    
    return specs
```

### 3. Парсинг данных

#### 3.1. Извлечение списка товаров

```python
def parse_product_list(soup):
    """Парсинг списка товаров со страницы каталога"""
    products = []
    table = soup.find('table')
    
    if table:
        rows = table.find_all('tr')
        for row in rows:
            cells = row.find_all('td')
            if len(cells) >= 5:
                # Название и ссылка
                link_cell = cells[0]
                link = link_cell.find('a')
                if link:
                    name = link.get_text(strip=True).replace('+', '').strip()
                    url = link.get('href')
                    if url and not url.startswith('http'):
                        url = f"https://www.nix.ru{url}"
                    
                    # Наличие
                    availability = cells[1].get_text(strip=True)
                    
                    # Цены
                    price_min = cells[3].get_text(strip=True).replace(' ', '')
                    price_max = cells[4].get_text(strip=True).replace(' ', '')
                    
                    products.append({
                        'name': name,
                        'url': url,
                        'availability': availability,
                        'price_min': int(price_min) if price_min.isdigit() else None,
                        'price_max': int(price_max) if price_max.isdigit() else None,
                    })
    
    return products
```

#### 3.2. Извлечение характеристик товара

```python
def parse_product_details(soup, base_url):
    """Парсинг детальной страницы товара"""
    specs = {}
    
    # Извлечение характеристик из таблиц
    tables = soup.find_all('table')
    for table in tables:
        rows = table.find_all('tr')
        for row in rows:
            cells = row.find_all(['td', 'th'])
            if len(cells) >= 2:
                key = cells[0].get_text(strip=True)
                value = cells[1].get_text(strip=True)
                
                # Маппинг ключей
                if 'Производитель' in key:
                    specs['brand'] = value
                elif 'Модель' in key:
                    specs['model'] = value
                elif 'Экран телефона' in key or 'Диагональ экрана' in key:
                    specs['screen_size'] = parse_screen_size(value)
                elif 'Оперативная память' in key or 'RAM' in key:
                    specs['ram'] = parse_memory(value)
                elif 'Объем встроенной памяти' in key or 'Встроенная память' in key:
                    specs['storage'] = parse_memory(value)
                elif 'Емкость аккумулятора' in key or 'Батарея' in key:
                    specs['battery'] = parse_battery(value)
                elif 'Основная камера' in key or 'Камера' in key:
                    specs['camera_main'] = parse_camera(value)
                elif 'Процессор' in key:
                    specs['processor'] = value
                elif 'ОС' in key or 'Операционная система' in key:
                    specs['os_version'] = value
    
    # Извлечение изображения
    img = soup.find('img', {'class': 'product-image'}) or soup.find('img', id='product-image')
    if img:
        img_url = img.get('src') or img.get('data-src')
        if img_url and not img_url.startswith('http'):
            img_url = f"https://www.nix.ru{img_url}"
        specs['image_url'] = img_url
    
    return specs
```

#### 3.3. Функции парсинга значений

```python
import re

def parse_screen_size(text):
    """Извлечение размера экрана '6.7"' -> 6.7"""
    match = re.search(r'(\d+\.?\d*)\s*["\']', text)
    if match:
        return float(match.group(1))
    return None

def parse_memory(text):
    """Извлечение объема памяти '128 ГБ' -> 128"""
    # Обработка разных форматов: 128 ГБ, 128GB, 8+128 ГБ
    match = re.search(r'(\d+)\s*ГБ', text, re.IGNORECASE)
    if match:
        return int(match.group(1))
    match = re.search(r'(\d+)\s*GB', text, re.IGNORECASE)
    if match:
        return int(match.group(1))
    return None

def parse_battery(text):
    """Извлечение емкости батареи '5000 мАч' -> 5000"""
    match = re.search(r'(\d+)\s*мАч', text, re.IGNORECASE)
    if match:
        return int(match.group(1))
    match = re.search(r'(\d+)\s*mAh', text, re.IGNORECASE)
    if match:
        return int(match.group(1))
    return None

def parse_camera(text):
    """Извлечение МП камеры '48 МП' -> 48"""
    match = re.search(r'(\d+)\s*МП', text, re.IGNORECASE)
    if match:
        return int(match.group(1))
    match = re.search(r'(\d+)\s*MP', text, re.IGNORECASE)
    if match:
        return int(match.group(1))
    return None
```

### 4. Обработка пагинации

```python
def get_total_pages(soup):
    """Определение общего количества страниц"""
    pagination = soup.find('div', class_='pagination') or soup.find('div', id='pagination')
    if pagination:
        links = pagination.find_all('a')
        max_page = 1
        for link in links:
            text = link.get_text(strip=True)
            if text.isdigit():
                max_page = max(max_page, int(text))
        return max_page
    return 1

def build_list_url(page='all', smartphones_only=True):
    """Построение URL страницы списка
    
    Args:
        page: номер страницы или 'all' для всех страниц
        smartphones_only: если True, добавляет фильтр только смартфоны
    """
    base_url = "https://www.nix.ru/price/price_list.html"
    params = {
        'section': 'smartphones_mobile_phones_all',
        'page': page,
        'sort': '+p6510+5528+5529',  # Сортировка по цене
        'store': 'msk-0_1721_1',  # Магазин (опционально)
        'thumbnail_view': '2'
    }
    
    # Фильтр только смартфоны (исключает кнопочные телефоны)
    if smartphones_only:
        params['enums[4843][]'] = '2'
    
    # Формирование URL с правильной кодировкой параметров
    from urllib.parse import urlencode
    return f"{base_url}?{urlencode(params, doseq=True)}"
```

### 5. Обработка ошибок

```python
def safe_parse(func):
    """Декоратор для безопасного парсинга"""
    def wrapper(*args, **kwargs):
        try:
            return func(*args, **kwargs)
        except Exception as e:
            logger.error(f"Ошибка парсинга: {str(e)}")
            return None
    return wrapper

@safe_parse
def parse_product_safely(soup, url):
    """Безопасный парсинг товара"""
    return parse_product_details(soup, url)
```

### 6. Задержки и ограничения

```python
import time
import random

def delay():
    """Случайная задержка между запросами"""
    time.sleep(random.uniform(1, 3))

def parse_with_delay(url):
    """Парсинг с задержкой"""
    delay()
    response = requests.get(url, headers={
        'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36'
    })
    return response
```

### 7. Полный процесс парсинга

```python
def parse_nix_smartphones(page='all', smartphones_only=True):
    """Полный процесс парсинга смартфонов с NIX
    
    Args:
        page: 'all' для всех страниц или номер страницы
        smartphones_only: если True, парсит только смартфоны (исключает кнопочные)
    """
    all_products = []
    
    if page == 'all':
        # Парсинг всех страниц
        current_page = 1
        while True:
            list_url = build_list_url(current_page, smartphones_only)
            response = requests.get(list_url, headers={
                'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36'
            })
            soup = BeautifulSoup(response.content, 'lxml')
            
            products = parse_product_list(soup)
            if not products:
                break
            
            # Парсинг детальных страниц
            for product in products:
                # Проверка, что это смартфон (не кнопочный телефон)
                if smartphones_only:
                    name_lower = product['name'].lower()
                    if any(word in name_lower for word in ['кнопочный', 'button', 'клавиатурный']):
                        continue
                
                detail_response = requests.get(product['url'], headers={
                    'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36'
                })
                detail_soup = BeautifulSoup(detail_response.content, 'lxml')
                specs = parse_product_details(detail_soup, product['url'])
                
                # Проверка типа устройства из характеристик
                if smartphones_only and specs.get('device_type'):
                    if 'кнопочный' in specs['device_type'].lower() or 'button' in specs['device_type'].lower():
                        continue
                
                # Объединение данных
                product.update(specs)
                all_products.append(product)
                
                delay()  # Задержка между запросами
            
            # Проверка наличия следующей страницы
            next_page_link = soup.find('a', string='следующая→') or soup.find('a', string='next')
            if not next_page_link:
                break
            
            current_page += 1
            delay()  # Задержка между страницами
    else:
        # Парсинг одной страницы
        list_url = build_list_url(page, smartphones_only)
        response = requests.get(list_url, headers={
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36'
        })
        soup = BeautifulSoup(response.content, 'lxml')
        
        products = parse_product_list(soup)
        
        for product in products:
            if smartphones_only:
                name_lower = product['name'].lower()
                if any(word in name_lower for word in ['кнопочный', 'button', 'клавиатурный']):
                    continue
            
            detail_response = requests.get(product['url'], headers={
                'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36'
            })
            detail_soup = BeautifulSoup(detail_response.content, 'lxml')
            specs = parse_product_details(detail_soup, product['url'])
            
            if smartphones_only and specs.get('device_type'):
                if 'кнопочный' in specs['device_type'].lower():
                    continue
            
            product.update(specs)
            all_products.append(product)
            
            delay()
    
    return all_products
```

## Примечания

1. **User-Agent**: Обязательно используйте реалистичный User-Agent
2. **Задержки**: Делайте задержки между запросами (1-3 секунды)
3. **Обработка ошибок**: Всегда обрабатывайте ошибки парсинга
4. **Валидация данных**: Проверяйте извлеченные данные на корректность
5. **Кэширование**: Сохраняйте промежуточные результаты

