"""
Парсер для сайта NIX.ru с использованием Selenium
Для работы с JavaScript-пагинацией
"""
import re
import time
import logging
from urllib.parse import urlencode, urljoin
from typing import List, Dict, Optional

from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.chrome.service import Service
from selenium.common.exceptions import TimeoutException, NoSuchElementException
from bs4 import BeautifulSoup

try:
    from webdriver_manager.chrome import ChromeDriverManager
    USE_WEBDRIVER_MANAGER = True
except ImportError:
    USE_WEBDRIVER_MANAGER = False

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

logger = logging.getLogger(__name__)


class NixParserSelenium:
    """Парсер смартфонов с сайта NIX.ru с использованием Selenium"""
    
    BASE_URL = "https://www.nix.ru"
    LIST_URL = "https://www.nix.ru/price/price_list.html"
    
    def __init__(self, headless=True, wait_timeout=10):
        """
        Args:
            headless: Запускать браузер в фоновом режиме (без окна)
            wait_timeout: Таймаут ожидания загрузки элементов (секунды)
        """
        self.headless = headless
        self.wait_timeout = wait_timeout
        self.driver = None
    
    def _init_driver(self):
        """Инициализация WebDriver"""
        if self.driver is None:
            chrome_options = Options()
            if self.headless:
                chrome_options.add_argument('--headless')
            chrome_options.add_argument('--no-sandbox')
            chrome_options.add_argument('--disable-dev-shm-usage')
            chrome_options.add_argument('--disable-gpu')
            chrome_options.add_argument('--window-size=1920,1080')
            chrome_options.add_argument('user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36')
            
            try:
                if USE_WEBDRIVER_MANAGER:
                    # Автоматическая установка ChromeDriver
                    service = Service(ChromeDriverManager().install())
                    self.driver = webdriver.Chrome(service=service, options=chrome_options)
                else:
                    # Обычная инициализация (ChromeDriver должен быть в PATH)
                    self.driver = webdriver.Chrome(options=chrome_options)
            except Exception as e:
                logger.error(f"Ошибка при инициализации ChromeDriver: {e}")
                logger.error("Убедитесь, что Chrome установлен и ChromeDriver доступен")
                logger.error("Или установите webdriver-manager: pip install webdriver-manager")
                raise
    
    def _close_driver(self):
        """Закрытие WebDriver"""
        if self.driver:
            try:
                self.driver.quit()
            except:
                pass
            self.driver = None
    
    def build_list_url(self, page=1, smartphones_only=True) -> str:
        """Построение URL страницы списка
        
        Параметр page в якорной части URL (#) - обрабатывается JavaScript
        """
        base_url = f"{self.LIST_URL}?section=smartphones_mobile_phones_all"
        
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
        """Извлечение МП камеры - берет первое число (основная камера)"""
        if not text:
            return None
        text = str(text).strip()
        match = re.search(r'^(\d+)\s*(?:\+|\s+\d+)?\s*(?:млн\.?\s*пикселов?|МП|MP)', text, re.IGNORECASE)
        if match:
            return int(match.group(1))
        match = re.search(r'^(\d+)', text)
        return int(match.group(1)) if match else None
    
    def parse_screen_resolution(self, text: str) -> Optional[Dict[str, int]]:
        """Извлечение разрешения экрана '2712 x 1220' -> {'width': 2712, 'height': 1220}"""
        if not text:
            return None
        match = re.search(r'(\d+)\s*x\s*(\d+)', str(text), re.IGNORECASE)
        if match:
            return {'width': int(match.group(1)), 'height': int(match.group(2))}
        return None
    
    def parse_cores(self, text: str) -> Optional[int]:
        """Извлечение количества ядер"""
        if not text:
            return None
        match = re.search(r'(\d+)', str(text))
        return int(match.group(1)) if match else None
    
    def parse_price(self, text: str) -> Optional[int]:
        """Извлечение цены"""
        if not text:
            return None
        
        text = str(text).strip()
        price_match = re.search(r'(\d{1,3}(?:\s+\d{3})*)', text)
        if price_match:
            price_str = price_match.group(1).replace(' ', '').replace('\xa0', '')
        else:
            price_str = re.sub(r'[^\d]', '', text)
        
        if not price_str:
            return None
        
        try:
            price = int(price_str)
            if price < 0 or price > 50_000_000:
                return None
            return price
        except (ValueError, OverflowError):
            return None
    
    def parse_product_list(self, soup: BeautifulSoup) -> List[Dict]:
        """Парсинг списка товаров со страницы каталога"""
        products = []
        
        tables = soup.find_all('table')
        products_table = None
        
        for table in tables:
            links = table.find_all('a', href=re.compile(r'(/autocatalog/|/2id\.php)'))
            if links:
                products_table = table
                break
        
        if not products_table:
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
                
                if price_min and price_max:
                    price = (price_min + price_max) // 2
                else:
                    price = price_min if price_min else price_max
                
                if price:
                    # Парсинг изображения из списка
                    image_url = None
                    img = None
                    if parent:
                        img = parent.find('img')
                    if not img and link.find_parent('td'):
                        img = link.find_parent('td').find('img')
                    
                    if img:
                        img_url = img.get('src') or img.get('data-src') or img.get('data-original') or img.get('data-lazy-src')
                        if img_url:
                            if not img_url.startswith('http'):
                                image_url = urljoin(self.BASE_URL, img_url)
                            else:
                                image_url = img_url
                    
                    products.append({
                        'name': name,
                        'url': url,
                        'availability': availability,
                        'price': price,
                        'image_url': image_url,
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
            
            price_min_text = ""
            price_max_text = ""
            found_prices = []
            
            for i in range(len(cells) - 1, max(0, len(cells) - 5), -1):
                cell_text = cells[i].get_text(strip=True)
                price_match = re.search(r'(\d{1,3}(?:\s+\d{3})+|\d{4,})', cell_text)
                if price_match:
                    potential_price = price_match.group(1).replace(' ', '').replace('\xa0', '')
                    if len(potential_price) >= 4:
                        try:
                            price_val = int(potential_price)
                            if 1000 <= price_val <= 50_000_000:
                                found_prices.append((price_val, cell_text))
                        except ValueError:
                            continue
            
            if found_prices:
                found_prices.sort(key=lambda x: x[0])
                if len(found_prices) >= 2:
                    price_min_text = found_prices[0][1]
                    price_max_text = found_prices[-1][1]
                else:
                    price_min_text = found_prices[0][1]
                    price_max_text = found_prices[0][1]
            
            if not price_min_text and not price_max_text:
                row_text = row.get_text()
                prices = re.findall(r'(\d{1,3}(?:\s+\d{3})+|\d{4,})', row_text)
                if prices:
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
            
            if price_min and price_max:
                price = (price_min + price_max) // 2
            else:
                price = price_min if price_min else price_max
            
            if not price:
                continue
            
            # Парсинг изображения из списка
            image_url = None
            img = row.find('img')
            if not img:
                # Ищем изображение в родительских элементах
                parent_cell = row.find('td')
                if parent_cell:
                    img = parent_cell.find('img')
            
            if img:
                img_url = img.get('src') or img.get('data-src') or img.get('data-original') or img.get('data-lazy-src')
                if img_url:
                    if not img_url.startswith('http'):
                        image_url = urljoin(self.BASE_URL, img_url)
                    else:
                        image_url = img_url
            
            products.append({
                'name': name,
                'url': url,
                'availability': availability,
                'price': price,
                'image_url': image_url,
            })
        
        return products
    
    def parse_product_details(self, soup: BeautifulSoup, url: str) -> Dict:
        """Парсинг детальной страницы товара (та же логика, что и в других парсерах)"""
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
                elif 'Оперативная память' in key or 'RAM' in key:
                    specs['ram'] = self.parse_memory(value)
                elif 'Объем встроенной памяти' in key or 'Встроенная память' in key:
                    specs['storage'] = self.parse_memory(value)
                elif 'Основная камера' in key or 'Камера' in key:
                    specs['camera_main'] = self.parse_camera(value)
                elif 'Модель процессора' in key or 'Процессор' in key:
                    specs['processor'] = value
                elif 'Количество ядер' in key:
                    specs['cores'] = self.parse_cores(value)
                elif 'Разрешение матрицы экрана' in key or 'Разрешение экрана' in key:
                    resolution = self.parse_screen_resolution(value)
                    if resolution:
                        specs['screen_width'] = resolution['width']
                        specs['screen_height'] = resolution['height']
                        specs['screen_pixels'] = resolution['width'] * resolution['height']
                elif 'ОС' in key or 'Операционная система' in key:
                    specs['os_version'] = value
        
        # Стратегия 1: Ищем по ID main_photo (самый надежный способ для NIX.ru)
        img = soup.find('img', id='main_photo')
        
        # Стратегия 2: Ищем по классам product/item/phone
        if not img:
            img = (soup.find('img', {'class': re.compile(r'product|item|phone', re.I)}) or
                   soup.find('img', id=re.compile(r'product|item|phone', re.I)))
        
        # Стратегия 3: Ищем в блоке товара
        if not img:
            product_block = (soup.find('div', {'class': re.compile(r'product|item|detail', re.I)}) or
                           soup.find('div', id=re.compile(r'product|item|detail', re.I)))
            if product_block:
                img = product_block.find('img')
        
        # Стратегия 4: Ищем любое изображение с товарными ключевыми словами в src
        if not img:
            all_imgs = soup.find_all('img')
            for candidate_img in all_imgs:
                src = candidate_img.get('src', '') or candidate_img.get('data-src', '') or candidate_img.get('data-original', '')
                if src and any(keyword in src.lower() for keyword in ['product', 'phone', 'smartphone', 'catalog', 'item', 'autocatalog']):
                    img = candidate_img
                    break
        
        if img:
            img_url = img.get('src') or img.get('data-src') or img.get('data-original')
            if img_url:
                # Очищаем URL от HTML entities
                img_url = img_url.replace('&amp;', '&')
                if not img_url.startswith('http'):
                    img_url = urljoin(self.BASE_URL, img_url)
                specs['image_url'] = img_url
        
        return specs
    
    def parse_all_smartphones(self, max_pages: int = None, use_page_all: bool = True) -> List[Dict]:
        """Парсинг всех смартфонов с использованием Selenium
        
        Args:
            max_pages: Максимальное количество товаров (если use_page_all=True) или страниц (если False)
            use_page_all: Использовать page=all для получения всех товаров сразу (рекомендуется)
        """
        all_products = []
        products = []
        
        print(f"\n{Fore.CYAN}{Style.BRIGHT}{'='*60}{Style.RESET_ALL}")
        print(f"{Fore.CYAN}{Style.BRIGHT}НАЧАЛО ПАРСИНГА С Selenium{Style.RESET_ALL}")
        print(f"{Fore.CYAN}Режим: {'Headless' if self.headless else 'С окном браузера'}{Style.RESET_ALL}")
        if use_page_all:
            print(f"{Fore.CYAN}Используется page=all (все товары за один раз){Style.RESET_ALL}")
        print(f"{Fore.CYAN}{Style.BRIGHT}{'='*60}{Style.RESET_ALL}\n")
        
        try:
            self._init_driver()
            
            if use_page_all:
                list_url = self.build_list_url('all', smartphones_only=True)
                print(f"{Fore.BLUE}Загрузка всех товаров (page=all)...{Style.RESET_ALL}")
                
                self.driver.get(list_url)
                
                try:
                    WebDriverWait(self.driver, self.wait_timeout).until(
                        EC.presence_of_element_located((By.TAG_NAME, "table"))
                    )
                except TimeoutException:
                    print(f"{Fore.YELLOW}Таймаут загрузки страницы{Style.RESET_ALL}")
                    return []
                
                time.sleep(5)
                
                self.driver.execute_script("window.scrollTo(0, document.body.scrollHeight);")
                time.sleep(2)
                self.driver.execute_script("window.scrollTo(0, 0);")
                time.sleep(1)
                
                page_source = self.driver.page_source
                soup = BeautifulSoup(page_source, 'lxml')
                
                products = self.parse_product_list(soup)
                if not products:
                    print(f"{Fore.YELLOW}Товары не найдены{Style.RESET_ALL}")
                    return []
                
                print(f"{Fore.GREEN}Найдено {len(products)} товаров{Style.RESET_ALL}")
                
                if max_pages:
                    products = products[:max_pages]
                    print(f"{Fore.CYAN}Ограничено до {len(products)} товаров (max_pages={max_pages}){Style.RESET_ALL}")
            else:
                current_page = 1
                all_page_products = []
                while True:
                    if max_pages and current_page > max_pages:
                        break
                    
                    list_url = self.build_list_url(current_page, smartphones_only=True)
                    print(f"{Fore.BLUE}Парсинг страницы {current_page}...{Style.RESET_ALL}")
                    
                    self.driver.get(list_url)
                    
                    try:
                        WebDriverWait(self.driver, self.wait_timeout).until(
                            EC.presence_of_element_located((By.TAG_NAME, "table"))
                        )
                    except TimeoutException:
                        print(f"{Fore.YELLOW}Таймаут загрузки страницы {current_page}{Style.RESET_ALL}")
                        break
                    
                    time.sleep(2)
                    page_source = self.driver.page_source
                    soup = BeautifulSoup(page_source, 'lxml')
                    
                    page_products = self.parse_product_list(soup)
                    if not page_products:
                        print(f"{Fore.YELLOW}На странице {current_page} товаров не найдено{Style.RESET_ALL}")
                        break
                    
                    all_page_products.extend(page_products)
                    print(f"{Fore.GREEN}Найдено {len(page_products)} товаров на странице {current_page} (всего {len(all_page_products)}){Style.RESET_ALL}")
                    
                    current_page += 1
                    if len(page_products) < 100:
                        break
                
                products = all_page_products
            
            import requests
            session = requests.Session()
            session.headers.update({
                'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36'
            })
            
            print(f"{Fore.CYAN}Начало парсинга детальных страниц...{Style.RESET_ALL}\n")
            
            for idx, product in enumerate(products, 1):
                try:
                    response = session.get(product['url'], timeout=30)
                    response.raise_for_status()
                    detail_soup = BeautifulSoup(response.content, 'lxml')
                    specs = self.parse_product_details(detail_soup, product['url'])
                    
                    if specs.get('is_smartphone') is False:
                        continue
                    
                    if specs.get('device_type') and 'кнопочный' in specs['device_type'].lower():
                        continue
                    
                    # Объединяем данные: если image_url уже есть в product (из списка), используем его, иначе из детальной страницы
                    if not product.get('image_url') and specs.get('image_url'):
                        product['image_url'] = specs['image_url']
                    
                    product.update(specs)
                    all_products.append(product)
                    
                    item_name = product.get('name', 'Неизвестно')[:60]
                    print(f"  [{idx:3d}/{len(products)}] {Fore.GREEN}OK{Style.RESET_ALL} - {item_name}...")
                    
                    time.sleep(0.5)  # Задержка между запросами
                    
                except Exception as e:
                    logger.error(f"Ошибка при парсинге {product['name']}: {str(e)}")
                    continue
            
            print(f"\n{Fore.CYAN}Обработано {len(all_products)} товаров{Style.RESET_ALL}\n")
        
        finally:
            self._close_driver()
        
        print(f"\n{Fore.CYAN}{Style.BRIGHT}{'='*60}{Style.RESET_ALL}")
        print(f"{Fore.GREEN}{Style.BRIGHT}ПАРСИНГ ЗАВЕРШЕН. Всего получено: {len(all_products)} смартфонов{Style.RESET_ALL}")
        print(f"{Fore.CYAN}{Style.BRIGHT}{'='*60}{Style.RESET_ALL}\n")
        
        return all_products
