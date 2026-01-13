"""
Парсер для сайта NIX.ru
Парсит только смартфоны (исключает кнопочные телефоны)
"""
import re
import time
import random
import logging
from urllib.parse import urlencode, urljoin
from typing import List, Dict, Optional

import requests
from bs4 import BeautifulSoup
from colorama import Fore, Style, init

# Инициализация colorama
init(autoreset=True)

logger = logging.getLogger(__name__)


class NixParser:
    """Парсер смартфонов с сайта NIX.ru"""
    
    BASE_URL = "https://www.nix.ru"
    LIST_URL = "https://www.nix.ru/price/price_list.html"
    
    def __init__(self, delay_min=1, delay_max=3):
        """
        Args:
            delay_min: минимальная задержка между запросами (секунды)
            delay_max: максимальная задержка между запросами (секунды)
        """
        self.delay_min = delay_min
        self.delay_max = delay_max
        self.session = requests.Session()
        self.session.headers.update({
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36'
        })
    
    def delay(self):
        """Случайная задержка между запросами"""
        time.sleep(random.uniform(self.delay_min, self.delay_max))
    
    def build_list_url(self, page='all', smartphones_only=True) -> str:
        """Построение URL страницы списка
        
        Args:
            page: номер страницы или 'all' для всех страниц
            smartphones_only: если True, добавляет фильтр только смартфоны
        
        Returns:
            URL для парсинга
        """
        params = {
            'section': 'smartphones_mobile_phones_all',
            'page': page,
            'sort': '+p6510+5528+5529',
            'store': 'msk-0_1721_1',
            'thumbnail_view': '2'
        }
        
        # Фильтр только смартфоны (исключает кнопочные телефоны)
        if smartphones_only:
            params['enums[4843][]'] = '2'
        
        return f"{self.LIST_URL}?{urlencode(params, doseq=True)}"
    
    def parse_screen_size(self, text: str) -> Optional[float]:
        """Извлечение размера экрана '6.7"' -> 6.7"""
        if not text:
            return None
        match = re.search(r'(\d+\.?\d*)\s*["\']', str(text))
        if match:
            return float(match.group(1))
        return None
    
    def parse_memory(self, text: str) -> Optional[int]:
        """Извлечение объема памяти '128 ГБ' -> 128"""
        if not text:
            return None
        # Обработка разных форматов: 128 ГБ, 128GB, 8+128 ГБ
        match = re.search(r'(\d+)\s*ГБ', str(text), re.IGNORECASE)
        if match:
            return int(match.group(1))
        match = re.search(r'(\d+)\s*GB', str(text), re.IGNORECASE)
        if match:
            return int(match.group(1))
        return None
    
    def parse_battery(self, text: str) -> Optional[int]:
        """Извлечение емкости батареи '5000 мАч' -> 5000"""
        if not text:
            return None
        match = re.search(r'(\d+)\s*мАч', str(text), re.IGNORECASE)
        if match:
            return int(match.group(1))
        match = re.search(r'(\d+)\s*mAh', str(text), re.IGNORECASE)
        if match:
            return int(match.group(1))
        return None
    
    def parse_camera(self, text: str) -> Optional[int]:
        """Извлечение МП камеры '48 МП' -> 48"""
        if not text:
            return None
        match = re.search(r'(\d+)\s*МП', str(text), re.IGNORECASE)
        if match:
            return int(match.group(1))
        match = re.search(r'(\d+)\s*MP', str(text), re.IGNORECASE)
        if match:
            return int(match.group(1))
        return None
    
    def parse_price(self, text: str) -> Optional[int]:
        """Извлечение цены '4 261' -> 4261, '4 261 руб' -> 4261"""
        if not text:
            return None
        
        text = str(text).strip()
        
        # Сначала пытаемся найти цену в формате "4 261" или "4261"
        # Ищем последовательность цифр с пробелами (разделители тысяч)
        price_match = re.search(r'(\d{1,3}(?:\s+\d{3})*)', text)
        if price_match:
            # Убираем пробелы и преобразуем
            price_str = price_match.group(1).replace(' ', '').replace('\xa0', '')
        else:
            # Если не нашли форматированное число, берем все цифры подряд
            price_str = re.sub(r'[^\d]', '', text)
        
        if not price_str:
            return None
        
        try:
            price = int(price_str)
            # Валидация: цена не должна быть больше 50 миллионов рублей (разумный максимум)
            # и не должна быть отрицательной
            if price < 0 or price > 50_000_000:
                logger.warning(f"Подозрительная цена: {price} из текста '{text}'")
                return None
            return price
        except (ValueError, OverflowError) as e:
            logger.warning(f"Не удалось преобразовать цену: '{price_str}' из текста '{text}': {e}")
            return None
    
    def parse_product_list(self, soup: BeautifulSoup) -> List[Dict]:
        """Парсинг списка товаров со страницы каталога"""
        products = []
        
        # Ищем таблицу с товарами - ищем по ссылкам на /autocatalog/ или /2id.php
        tables = soup.find_all('table')
        products_table = None
        
        for table in tables:
            # Проверяем, есть ли в таблице ссылки на товары
            links = table.find_all('a', href=re.compile(r'(/autocatalog/|/2id\.php)'))
            if links:
                products_table = table
                logger.debug(f"Найдена таблица с товарами, ссылок: {len(links)}")
                break
        
        if not products_table:
            logger.warning("Таблица с товарами не найдена")
            # Попробуем найти все ссылки на товары напрямую
            all_links = soup.find_all('a', href=re.compile(r'(/autocatalog/|/2id\.php)'))
            if all_links:
                logger.info(f"Найдено {len(all_links)} ссылок на товары вне таблицы")
                # Парсим напрямую из ссылок
                for link in all_links:
                    name = link.get_text(strip=True).replace('+', '').strip()
                    if not name:
                        continue
                    
                    url = link.get('href')
                    if not url:
                        continue
                    
                    if not url.startswith('http'):
                        url = urljoin(self.BASE_URL, url)
                    
                    # Ищем цену рядом со ссылкой
                    parent = link.find_parent('tr')
                    if parent:
                        cells = parent.find_all('td')
                        if len(cells) >= 4:
                            price_min_text = cells[3].get_text(strip=True) if len(cells) > 3 else ""
                            price_max_text = cells[4].get_text(strip=True) if len(cells) > 4 else price_min_text
                            availability = cells[1].get_text(strip=True) if len(cells) > 1 else ""
                        else:
                            price_min_text = ""
                            price_max_text = ""
                            availability = ""
                    else:
                        price_min_text = ""
                        price_max_text = ""
                        availability = ""
                    
                    price_min = self.parse_price(price_min_text)
                    price_max = self.parse_price(price_max_text)
                    price = price_min if price_min else price_max
                    
                    products.append({
                        'name': name,
                        'url': url,
                        'availability': availability,
                        'price': price,
                        'price_min': price_min,
                        'price_max': price_max,
                    })
            
            return products
        
        rows = products_table.find_all('tr')
        logger.debug(f"Найдено строк в таблице: {len(rows)}")
        
        for row in rows:
            cells = row.find_all('td')
            if len(cells) < 3:  # Минимум 3 ячейки (название, наличие, цена)
                continue
            
            # Ищем ссылку на товар
            link = row.find('a', href=re.compile(r'(/autocatalog/|/2id\.php)'))
            if not link:
                continue
            
            name = link.get_text(strip=True).replace('+', '').strip()
            if not name:
                continue
            
            url = link.get('href')
            if not url:
                continue
            
            if not url.startswith('http'):
                url = urljoin(self.BASE_URL, url)
            
            # Проверка, что это смартфон (не кнопочный телефон)
            name_lower = name.lower()
            if any(word in name_lower for word in ['кнопочный', 'button', 'клавиатурный']):
                # Пропускаем, если это явно кнопочный телефон (без слова "смартфон")
                if 'смартфон' not in name_lower:
                    continue
            
            # Наличие (обычно во второй ячейке)
            availability = ""
            if len(cells) > 1:
                availability = cells[1].get_text(strip=True)
            
            # Цены (обычно в последних ячейках таблицы)
            # На сайте NIX.ru цены обычно в предпоследней и последней колонках
            price_min_text = ""
            price_max_text = ""
            found_prices = []
            
            # Ищем все цены в ячейках, начиная с конца (цены обычно справа)
            for i in range(len(cells) - 1, max(0, len(cells) - 5), -1):  # Проверяем последние 5 ячеек
                cell_text = cells[i].get_text(strip=True)
                # Ищем числа с пробелами (формат: "12 345" или "12345")
                # Цена должна содержать минимум 4 цифры (от 1000 рублей)
                price_match = re.search(r'(\d{1,3}(?:\s+\d{3})+|\d{4,})', cell_text)
                if price_match:
                    potential_price = price_match.group(1).replace(' ', '').replace('\xa0', '')
                    # Проверяем, что это действительно цена (достаточно большое число)
                    if len(potential_price) >= 4:
                        try:
                            price_val = int(potential_price)
                            # Цена должна быть в разумных пределах (от 1000 до 50 млн)
                            if 1000 <= price_val <= 50_000_000:
                                found_prices.append((price_val, cell_text))
                        except ValueError:
                            continue
            
            # Сортируем найденные цены по значению
            if found_prices:
                found_prices.sort(key=lambda x: x[0])  # Сортируем по числовому значению
                if len(found_prices) >= 2:
                    # Первая (меньшая) - это min, последняя (большая) - это max
                    price_min_text = found_prices[0][1]
                    price_max_text = found_prices[-1][1]
                else:
                    # Если только одна цена, используем её для обоих
                    price_min_text = found_prices[0][1]
                    price_max_text = found_prices[0][1]
            
            # Если не нашли цены в ячейках, ищем в тексте всей строки
            if not price_min_text and not price_max_text:
                row_text = row.get_text()
                # Ищем все цены в формате "12 345" или "12 345 руб"
                prices = re.findall(r'(\d{1,3}(?:\s+\d{3})+|\d{4,})', row_text)
                if prices:
                    # Преобразуем в числа и сортируем
                    price_values = []
                    for p in prices:
                        try:
                            p_clean = p.replace(' ', '').replace('\xa0', '')
                            p_val = int(p_clean)
                            if 1000 <= p_val <= 50_000_000:
                                price_values.append(p_val)
                        except ValueError:
                            continue
                    
                    if price_values:
                        price_values.sort()
                        if len(price_values) >= 2:
                            price_min_text = str(price_values[0])
                            price_max_text = str(price_values[-1])
                        else:
                            price_min_text = str(price_values[0])
                            price_max_text = str(price_values[0])
            
            price_min = self.parse_price(price_min_text)
            price_max = self.parse_price(price_max_text)
            
            # Используем минимальную цену как основную
            price = price_min if price_min else price_max
            
            if not price:
                logger.warning(f"Не удалось извлечь цену для: {name}")
                continue
            
            products.append({
                'name': name,
                'url': url,
                'availability': availability,
                'price': price,
                'price_min': price_min,
                'price_max': price_max,
            })
        
        logger.info(f"Извлечено товаров из списка: {len(products)}")
        return products
    
    def parse_product_details(self, soup: BeautifulSoup, url: str) -> Dict:
        """Парсинг детальной страницы товара"""
        specs = {}
        
        # Извлечение характеристик из таблиц
        tables = soup.find_all('table')
        for table in tables:
            rows = table.find_all('tr')
            for row in rows:
                cells = row.find_all(['td', 'th'])
                if len(cells) < 2:
                    continue
                
                key = cells[0].get_text(strip=True)
                value = cells[1].get_text(strip=True)
                
                if not key or not value:
                    continue
                
                # Маппинг ключей
                if 'Производитель' in key:
                    specs['brand'] = value
                elif 'Модель' in key:
                    specs['model'] = value
                elif 'Тип оборудования' in key:
                    specs['device_type'] = value
                    # Проверка, что это смартфон
                    if 'смартфон' in value.lower() or 'smartphone' in value.lower():
                        specs['is_smartphone'] = True
                    else:
                        specs['is_smartphone'] = False
                elif 'Экран телефона' in key or 'Диагональ экрана' in key:
                    specs['screen_size'] = self.parse_screen_size(value)
                elif 'Оперативная память' in key or 'RAM' in key:
                    specs['ram'] = self.parse_memory(value)
                elif 'Объем встроенной памяти' in key or 'Встроенная память' in key:
                    specs['storage'] = self.parse_memory(value)
                elif 'Емкость аккумулятора' in key or 'Батарея' in key:
                    specs['battery'] = self.parse_battery(value)
                elif 'Основная камера' in key or ('Камера' in key and 'МП' in value):
                    specs['camera_main'] = self.parse_camera(value)
                elif 'Процессор' in key:
                    specs['processor'] = value
                elif 'ОС' in key or 'Операционная система' in key:
                    specs['os_version'] = value
        
        # Извлечение изображения товара
        img_url = None
        
        # Стратегия 1: Ищем по классам и ID
        img = (soup.find('img', {'class': re.compile(r'product|item|phone', re.I)}) or
               soup.find('img', id=re.compile(r'product|item|phone', re.I)) or
               soup.find('img', {'class': 'product-image'}) or
               soup.find('img', id='product-image'))
        
        # Стратегия 2: Ищем в основном блоке товара
        if not img:
            product_block = (soup.find('div', {'class': re.compile(r'product|item|detail', re.I)}) or
                           soup.find('div', id=re.compile(r'product|item|detail', re.I)))
            if product_block:
                img = product_block.find('img')
        
        # Стратегия 3: Ищем любое изображение с товарными ключевыми словами в src
        if not img:
            all_imgs = soup.find_all('img')
            for candidate_img in all_imgs:
                src = candidate_img.get('src', '') or candidate_img.get('data-src', '')
                if any(keyword in src.lower() for keyword in ['product', 'phone', 'smartphone', 'catalog', 'item']):
                    img = candidate_img
                    break
        
        # Стратегия 4: Берем первое большое изображение (не иконку)
        if not img:
            all_imgs = soup.find_all('img')
            for candidate_img in all_imgs:
                src = candidate_img.get('src', '') or candidate_img.get('data-src', '')
                # Пропускаем маленькие изображения (иконки, логотипы)
                if (src and 
                    not any(skip in src.lower() for skip in ['icon', 'logo', 'button', 'arrow', 'spacer']) and
                    any(ext in src.lower() for ext in ['.jpg', '.jpeg', '.png', '.webp'])):
                    img = candidate_img
                    break
        
        if img:
            img_url = img.get('src') or img.get('data-src') or img.get('data-original')
            if img_url:
                # Очистка URL от параметров размера (если есть)
                if '?' in img_url:
                    base_url = img_url.split('?')[0]
                    # Оставляем только если это не параметры размера
                    if 'width' not in img_url.lower() and 'height' not in img_url.lower():
                        img_url = img_url
                    else:
                        img_url = base_url
                
                # Преобразование относительного URL в абсолютный
                if not img_url.startswith('http'):
                    img_url = urljoin(self.BASE_URL, img_url)
                
                specs['image_url'] = img_url
                logger.debug(f"Найдено изображение: {img_url}")
            else:
                logger.debug(f"Изображение найдено, но URL пустой")
        else:
            logger.debug(f"Изображение товара не найдено на странице {url}")
        
        return specs
    
    def parse_all_smartphones(self, max_pages=None) -> List[Dict]:
        """Парсинг всех смартфонов
        
        Args:
            max_pages: максимальное количество страниц (None = все страницы)
        
        Returns:
            Список словарей с данными смартфонов
        """
        all_products = []
        current_page = 1
        
        logger.info("Начало парсинга смартфонов с NIX.ru")
        
        while True:
            if max_pages and current_page > max_pages:
                break
            
            list_url = self.build_list_url(current_page, smartphones_only=True)
            logger.info(f"Парсинг страницы {current_page}: {list_url}")
            
            try:
                response = self.session.get(list_url, timeout=30)
                response.raise_for_status()
                soup = BeautifulSoup(response.content, 'lxml')
                
                products = self.parse_product_list(soup)
                if not products:
                    logger.info(f"На странице {current_page} товаров не найдено")
                    break
                
                logger.info(f"Найдено {len(products)} товаров на странице {current_page}")
                
                # Парсинг детальных страниц
                for idx, product in enumerate(products, 1):
                    logger.info(f"Парсинг товара {idx}/{len(products)}: {product['name']}")
                    
                    try:
                        detail_response = self.session.get(product['url'], timeout=30)
                        detail_response.raise_for_status()
                        detail_soup = BeautifulSoup(detail_response.content, 'lxml')
                        specs = self.parse_product_details(detail_soup, product['url'])
                        
                        # Проверка типа устройства
                        if specs.get('is_smartphone') is False:
                            logger.info(f"Пропуск: {product['name']} - не смартфон")
                            continue
                        
                        if specs.get('device_type'):
                            if 'кнопочный' in specs['device_type'].lower():
                                logger.info(f"Пропуск: {product['name']} - кнопочный телефон")
                                continue
                        
                        # Объединение данных
                        product.update(specs)
                        all_products.append(product)
                        
                        logger.info(f"Успешно спарсен: {product['name']}")
                        
                    except Exception as e:
                        logger.error(f"Ошибка при парсинге товара {product['name']}: {str(e)}")
                        continue
                    
                    self.delay()  # Задержка между запросами
                
                # Проверка наличия следующей страницы
                next_page_link = soup.find('a', string=re.compile(r'следующая|next', re.I))
                if not next_page_link:
                    logger.info("Следующая страница не найдена")
                    break
                
                current_page += 1
                self.delay()  # Задержка между страницами
                
            except Exception as e:
                logger.error(f"Ошибка при парсинге страницы {current_page}: {str(e)}")
                break
        
        logger.info(f"Парсинг завершен. Всего получено: {len(all_products)} смартфонов")
        return all_products

