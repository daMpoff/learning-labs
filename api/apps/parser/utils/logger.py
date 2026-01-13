"""
Красивое логирование с цветами для парсера
"""
import logging
import sys
from datetime import datetime

# Попытка импорта colorama (опционально)
try:
    from colorama import init, Fore, Back, Style
    # Инициализация colorama для Windows
    init(autoreset=True)
    COLORAMA_AVAILABLE = True
except ImportError:
    # Если colorama не установлен, используем пустые строки
    class Fore:
        BLACK = ''
        RED = ''
        GREEN = ''
        YELLOW = ''
        BLUE = ''
        MAGENTA = ''
        CYAN = ''
        WHITE = ''
        RESET = ''
    
    class Back:
        BLACK = ''
        RED = ''
        GREEN = ''
        YELLOW = ''
        BLUE = ''
        MAGENTA = ''
        CYAN = ''
        WHITE = ''
        RESET = ''
    
    class Style:
        DIM = ''
        NORMAL = ''
        BRIGHT = ''
        RESET_ALL = ''
    
    COLORAMA_AVAILABLE = False


class ColoredFormatter(logging.Formatter):
    """Форматтер с цветами для разных уровней логирования"""
    
    COLORS = {
        'DEBUG': Fore.CYAN,
        'INFO': Fore.GREEN,
        'WARNING': Fore.YELLOW,
        'ERROR': Fore.RED,
        'CRITICAL': Fore.RED + Back.WHITE,
    }
    
    def format(self, record):
        # Добавляем цвет
        log_color = self.COLORS.get(record.levelname, '')
        
        # Форматируем время
        record.asctime = datetime.fromtimestamp(record.created).strftime('%H:%M:%S')
        
        # Форматируем сообщение
        if record.levelname == 'INFO' and 'ПАРСИНГ' in record.getMessage():
            # Специальное форматирование для заголовков парсинга
            return f"{Style.BRIGHT}{Fore.CYAN}{'='*60}{Style.RESET_ALL}\n{Style.BRIGHT}{Fore.CYAN}{record.getMessage()}{Style.RESET_ALL}\n{Style.BRIGHT}{Fore.CYAN}{'='*60}{Style.RESET_ALL}"
        elif record.levelname == 'INFO' and any(word in record.getMessage() for word in ['Найдено', 'Обработано', 'Добавлен', 'Обновлен']):
            return f"{log_color}{record.getMessage()}{Style.RESET_ALL}"
        else:
            return f"{log_color}[{record.asctime}] {record.getMessage()}{Style.RESET_ALL}"


def setup_colored_logger(name: str = None, level: int = logging.INFO):
    """Настройка цветного логгера"""
    logger = logging.getLogger(name)
    logger.setLevel(level)
    
    # Удаляем существующие обработчики
    logger.handlers.clear()
    
    # Создаем консольный обработчик
    console_handler = logging.StreamHandler(sys.stdout)
    console_handler.setLevel(level)
    
    # Устанавливаем форматтер
    formatter = ColoredFormatter(
        fmt='%(message)s',
        datefmt='%H:%M:%S'
    )
    console_handler.setFormatter(formatter)
    
    logger.addHandler(console_handler)
    
    return logger


def print_header(text: str, char: str = "=", color: str = Fore.CYAN):
    """Печать красивого заголовка"""
    print(f"\n{Style.BRIGHT}{color}{char*60}{Style.RESET_ALL}")
    print(f"{Style.BRIGHT}{color}{text.center(60)}{Style.RESET_ALL}")
    print(f"{Style.BRIGHT}{color}{char*60}{Style.RESET_ALL}\n")


def print_progress(current: int, total: int, item_name: str = "", bar_length: int = 30, parsed_items: list = None):
    """Печать прогресс-бара"""
    if total == 0:
        return
    
    percent = (current / total) * 100
    filled = int(bar_length * current / total)
    bar = '█' * filled + '░' * (bar_length - filled)
    
    # Очищаем предыдущую строку
    print(f"\r{Fore.CYAN}Прогресс: {Fore.GREEN}[{bar}]{Fore.CYAN} {current}/{total} ({percent:.1f}%){Style.RESET_ALL}", end='', flush=True)
    
    if current == total:
        print()  # Новая строка после завершения


def print_success(message: str):
    """Печать успешного сообщения"""
    print(f"{Fore.GREEN}{message}{Style.RESET_ALL}")


def print_error(message: str):
    """Печать сообщения об ошибке"""
    print(f"{Fore.RED}{message}{Style.RESET_ALL}")


def print_warning(message: str):
    """Печать предупреждения"""
    print(f"{Fore.YELLOW}{message}{Style.RESET_ALL}")


def print_info(message: str):
    """Печать информационного сообщения"""
    print(f"{Fore.BLUE}{message}{Style.RESET_ALL}")


def print_parsed_item(index: int, total: int, item_name: str, elapsed_time: float):
    """Печать информации о спаршенном товаре"""
    item_display = item_name[:60] + "..." if len(item_name) > 60 else item_name
    print(f"  [{index:3d}/{total}] {Fore.CYAN}{item_display}{Style.RESET_ALL} - {Fore.YELLOW}{elapsed_time:.2f}с{Style.RESET_ALL}")

