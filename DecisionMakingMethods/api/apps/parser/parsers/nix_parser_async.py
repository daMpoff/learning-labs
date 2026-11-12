"""
Асинхронный парсер для сайта NIX.ru с параллельной обработкой
Значительно быстрее обычного парсера
"""
import re
import asyncio
import logging
from urllib.parse import urlencode, urljoin
from typing import List, Dict, Optional
from concurrent.futures import ThreadPoolExecutor

import httpx
from bs4 import BeautifulSoup
import time

# Опциональный импорт colorama
try:
    from colorama import Fore, Style, init
    init(autoreset=True)
except ImportError:
    class Fore:
        CYAN = ''
        GREEN = ''
        YELLOW = ''
        BLUE = ''
        RED = ''
    class Style:
        BRIGHT = ''
        RESET_ALL = ''
init(autoreset=True)

logger = logging.getLogger(__name__)


class NixParserAsync:
    """Асинхронный парсер смартфонов с сайта NIX.ru"""
    
    BASE_URL = "https://www.nix.ru"
    LIST_URL = "https://www.nix.ru/price/price_list.html"
    
    def __init__(self, delay_min=0.5, delay_max=1.0, max_concurrent=10):
        """
        Args:
            delay_min: минимальная задержка между запросами (секунды)
            delay_max: максимальная задержка между запросами (секунды)
            max_concurrent: максимальное количество одновременных запросов
        """
        self.delay_min = delay_min
        self.delay_max = delay_max
        self.max_concurrent = max_concurrent
        self.semaphore = asyncio.Semaphore(max_concurrent)
    
    def build_list_url(self, page=1, smartphones_only=True) -> str:
        """Построение URL страницы списка
        
        Параметр page должен быть в якорной части URL (#), а не в query string
        """
        # Базовый URL с section в query string
        base_url = f"{self.LIST_URL}?section=smartphones_mobile_phones_all"
        
        # Параметры в якорной части (после #)
        anchor_params = {
            'c_id': '368',
            'fn': '368',
            'g_id': '265',
            'new_goods': '0',
            'page': str(page),
            'sort': '+p6510+5528+5529',
            'spoiler': '1',
            'store': 'msk-0_1721_1',
            'thumbnail_view': '2'
        }
        
        if smartphones_only:
            anchor_params['enums[4843][]'] = '2'
        
        anchor = urlencode(anchor_params, doseq=True)
        return f"{base_url}#{anchor}"
    
    def parse_screen_size(self, text: str) -> Optional[float]:
        """Извлечение размера экрана"""
        if not text:
            return None
        match = re.search(r'(\d+\.?\d*)\s*["\']', str(text))
        return float(match.group(1)) if match else None
    
    def parse_memory(self, text: str) -> Optional[int]:
        """Извлечение объема памяти"""
        if not text:
            return None
        match = re.search(r'(\d+)\s*ГБ', str(text), re.IGNORECASE)
        if match:
            return int(match.group(1))
        match = re.search(r'(\d+)\s*GB', str(text), re.IGNORECASE)
        return int(match.group(1)) if match else None
    
    def parse_battery(self, text: str) -> Optional[int]:
        """Извлечение емкости батареи"""
        if not text:
            return None
        match = re.search(r'(\d+)\s*мАч', str(text), re.IGNORECASE)
        if match:
            return int(match.group(1))
        match = re.search(r'(\d+)\s*mAh', str(text), re.IGNORECASE)
        return int(match.group(1)) if match else None
    
    def parse_camera(self, text: str) -> Optional[int]:
        """Извлечение МП камеры"""
        if not text:
            return None
        match = re.search(r'(\d+)\s*МП', str(text), re.IGNORECASE)
        if match:
            return int(match.group(1))
        match = re.search(r'(\d+)\s*MP', str(text), re.IGNORECASE)
        return int(match.group(1)) if match else None
    
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
        
        # Ищем таблицу с товарами
        tables = soup.find_all('table')
        products_table = None
        
        for table in tables:
            links = table.find_all('a', href=re.compile(r'(/autocatalog/|/2id\.php)'))
            if links:
                products_table = table
                break
        
        if not products_table:
            # Парсим напрямую из ссылок
            all_links = soup.find_all('a', href=re.compile(r'(/autocatalog/|/2id\.php)'))
            for link in all_links:
                name = link.get_text(strip=True).replace('+', '').strip()
                if not name:
                    continue
                
                url = link.get('href')
                if not url:
                    continue
                
                if not url.startswith('http'):
                    url = urljoin(self.BASE_URL, url)
                
                parent = link.find_parent('tr')
                price_min_text = ""
                price_max_text = ""
                availability = ""
                
                if parent:
                    cells = parent.find_all('td')
                    if len(cells) >= 4:
                        price_min_text = cells[3].get_text(strip=True) if len(cells) > 3 else ""
                        price_max_text = cells[4].get_text(strip=True) if len(cells) > 4 else price_min_text
                        availability = cells[1].get_text(strip=True) if len(cells) > 1 else ""
                
                price_min = self.parse_price(price_min_text)
                price_max = self.parse_price(price_max_text)
                price = price_min if price_min else price_max
                
                if price:
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
        
        for row in rows:
            cells = row.find_all('td')
            if len(cells) < 3:
                continue
            
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
            
            name_lower = name.lower()
            if any(word in name_lower for word in ['кнопочный', 'button', 'клавиатурный']):
                if 'смартфон' not in name_lower:
                    continue
            
            availability = cells[1].get_text(strip=True) if len(cells) > 1 else ""
            
            # Цены (обычно в последних ячейках таблицы)
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
            price = price_min if price_min else price_max
            
            if not price:
                continue
            
            products.append({
                'name': name,
                'url': url,
                'availability': availability,
                'price': price,
                'price_min': price_min,
                'price_max': price_max,
            })
        
        return products
    
    async def fetch_page(self, client: httpx.AsyncClient, url: str) -> Optional[BeautifulSoup]:
        """Асинхронная загрузка страницы"""
        async with self.semaphore:  # Ограничение одновременных запросов
            try:
                await asyncio.sleep(0.1)  # Минимальная задержка
                response = await client.get(url, timeout=30.0, follow_redirects=True)
                response.raise_for_status()
                return BeautifulSoup(response.content, 'lxml')
            except Exception as e:
                logger.error(f"Ошибка при загрузке {url}: {str(e)}")
                return None
    
    async def parse_product_details(self, client: httpx.AsyncClient, url: str) -> Dict:
        """Асинхронный парсинг детальной страницы товара"""
        soup = await self.fetch_page(client, url)
        if not soup:
            return {}
        
        specs = {}
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
                
                if 'Производитель' in key:
                    specs['brand'] = value
                elif 'Модель' in key:
                    specs['model'] = value
                elif 'Тип оборудования' in key:
                    specs['device_type'] = value
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
        
        # Стратегия 1: Ищем по ID main_photo (самый надежный способ для NIX.ru)
        img = soup.find('img', id='main_photo')
        
        # Стратегия 2: Ищем по классам и ID
        if not img:
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
                # Очищаем URL от HTML entities
                img_url = img_url.replace('&amp;', '&')
                
                # Преобразование относительного URL в абсолютный
                if not img_url.startswith('http'):
                    img_url = urljoin(self.BASE_URL, img_url)
                
                specs['image_url'] = img_url
        
        return specs
    
    async def parse_all_smartphones(self, max_pages: int = None) -> List[Dict]:
        """Асинхронный парсинг всех смартфонов с параллельной обработкой"""
        all_products = []
        current_page = 1
        
        print(f"\n{Fore.CYAN}{Style.BRIGHT}{'='*60}{Style.RESET_ALL}")
        print(f"{Fore.CYAN}{Style.BRIGHT}НАЧАЛО АСИНХРОННОГО ПАРСИНГА СМАРТФОНОВ С NIX.RU{Style.RESET_ALL}")
        print(f"{Fore.CYAN}Максимальное количество одновременных запросов: {self.max_concurrent}{Style.RESET_ALL}")
        print(f"{Fore.CYAN}{Style.BRIGHT}{'='*60}{Style.RESET_ALL}\n")
        
        headers = {
            'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36'
        }
        
        async with httpx.AsyncClient(headers=headers, timeout=30.0) as client:
            # Парсинг списков товаров по страницам
            while True:
                if max_pages and current_page > max_pages:
                    break
                
                list_url = self.build_list_url(current_page, smartphones_only=True)
                print(f"{Fore.BLUE}Парсинг страницы {current_page}...{Style.RESET_ALL}")
                
                soup = await self.fetch_page(client, list_url)
                if not soup:
                    break
                
                products = self.parse_product_list(soup)
                if not products:
                    print(f"{Fore.YELLOW}На странице {current_page} товаров не найдено{Style.RESET_ALL}")
                    break
                
                print(f"{Fore.GREEN}Найдено {len(products)} товаров на странице {current_page}{Style.RESET_ALL}")
                print(f"{Fore.CYAN}Начало парсинга детальных страниц...{Style.RESET_ALL}\n")
                
                # Параллельный парсинг детальных страниц
                tasks = []
                for product in products:
                    task = self.parse_product_details(client, product['url'])
                    tasks.append((product, task))
                
                # Выполняем все задачи параллельно с замером времени
                start_time = time.time()
                results = await asyncio.gather(*[task for _, task in tasks], return_exceptions=True)
                
                for idx, ((product, _), specs) in enumerate(zip(tasks, results), 1):
                    item_start_time = time.time()
                    
                    if isinstance(specs, Exception):
                        elapsed = time.time() - item_start_time
                        print(f"  [{idx:3d}/{len(products)}] {Fore.RED}ОШИБКА{Style.RESET_ALL} - {product['name'][:50]}... - {elapsed:.2f}с")
                        logger.error(f"Ошибка при парсинге {product['name']}: {str(specs)}")
                        continue
                    
                    if specs.get('is_smartphone') is False:
                        elapsed = time.time() - item_start_time
                        print(f"  [{idx:3d}/{len(products)}] {Fore.YELLOW}ПРОПУСК (не смартфон){Style.RESET_ALL} - {product['name'][:50]}... - {elapsed:.2f}с")
                        continue
                    
                    if specs.get('device_type') and 'кнопочный' in specs['device_type'].lower():
                        elapsed = time.time() - item_start_time
                        print(f"  [{idx:3d}/{len(products)}] {Fore.YELLOW}ПРОПУСК (кнопочный){Style.RESET_ALL} - {product['name'][:50]}... - {elapsed:.2f}с")
                        continue
                    
                    product.update(specs)
                    all_products.append(product)
                    
                    elapsed = time.time() - item_start_time
                    item_name = product.get('name', 'Неизвестно')
                    item_display = item_name[:60] + "..." if len(item_name) > 60 else item_name
                    print(f"  [{idx:3d}/{len(products)}] {Fore.GREEN}OK{Style.RESET_ALL} - {item_display} - {elapsed:.2f}с")
                
                total_time = time.time() - start_time
                print(f"\n{Fore.CYAN}Страница {current_page} обработана за {total_time:.2f}с ({len(all_products)} товаров){Style.RESET_ALL}\n")
                
                # Увеличиваем номер страницы
                current_page += 1
                
                # Если указан max_pages, проверяем лимит
                if max_pages and current_page > max_pages:
                    break
                
                # Если на странице меньше товаров, чем обычно, значит это последняя страница
                if len(products) < 50:  # Обычно на странице 100 товаров
                    break
        
        print(f"\n{Fore.CYAN}{Style.BRIGHT}{'='*60}{Style.RESET_ALL}")
        print(f"{Fore.GREEN}{Style.BRIGHT}ПАРСИНГ ЗАВЕРШЕН. Всего получено: {len(all_products)} смартфонов{Style.RESET_ALL}")
        print(f"{Fore.CYAN}{Style.BRIGHT}{'='*60}{Style.RESET_ALL}\n")
        
        return all_products

