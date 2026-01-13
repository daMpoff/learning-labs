# Работа с изображениями смартфонов

## Хранение изображений

Изображения **не скачиваются** на сервер, а хранятся только **URL ссылки** на оригинальные изображения с сайта NIX.ru.

### Преимущества:
- ✅ Не занимает место на диске
- ✅ Всегда актуальные изображения
- ✅ Быстрая работа (не нужно скачивать)
- ✅ Простота реализации

### Недостатки:
- ⚠️ Если сайт NIX.ru изменит URL, изображения могут перестать работать
- ⚠️ Требуется интернет для отображения

## Структура данных

### В базе данных:
```sql
image_url TEXT  -- URL изображения, например: https://www.nix.ru/images/products/12345.jpg
```

### В API ответе:
```json
{
  "id": 1,
  "name": "Samsung Galaxy S23",
  "image_url": "https://www.nix.ru/images/products/samsung-s23.jpg",
  ...
}
```

## Логика поиска изображений

Парсер использует несколько стратегий для поиска изображений:

1. **По классам и ID** - ищет `img` с классами `product`, `item`, `phone`
2. **В блоке товара** - ищет изображение внутри блока с классом `product` или `detail`
3. **По ключевым словам в URL** - ищет изображения с `product`, `phone`, `smartphone` в пути
4. **Первое большое изображение** - берет первое изображение, которое не является иконкой/логотипом

## Использование в Frontend

### Vue.js пример:

```vue
<template>
  <div v-for="phone in smartphones" :key="phone.id">
    <img 
      v-if="phone.image_url" 
      :src="phone.image_url" 
      :alt="phone.name"
      @error="handleImageError"
    />
    <div v-else class="no-image">Нет изображения</div>
  </div>
</template>

<script>
export default {
  methods: {
    handleImageError(event) {
      // Если изображение не загрузилось, показываем placeholder
      event.target.src = '/placeholder-phone.png';
    }
  }
}
</script>
```

### Проверка наличия изображения:

```javascript
// Проверка наличия изображения
const hasImage = smartphone.image_url && smartphone.image_url.length > 0;

// Отображение с fallback
const imageUrl = smartphone.image_url || '/default-phone.png';
```

## Статистика изображений

Можно проверить, сколько смартфонов имеют изображения:

```sql
-- Количество смартфонов с изображениями
SELECT COUNT(*) FROM smartphones WHERE image_url IS NOT NULL AND image_url != '';

-- Количество без изображений
SELECT COUNT(*) FROM smartphones WHERE image_url IS NULL OR image_url = '';

-- Процент покрытия
SELECT 
  COUNT(*) as total,
  SUM(CASE WHEN image_url IS NOT NULL AND image_url != '' THEN 1 ELSE 0 END) as with_images,
  ROUND(100.0 * SUM(CASE WHEN image_url IS NOT NULL AND image_url != '' THEN 1 ELSE 0 END) / COUNT(*), 2) as percentage
FROM smartphones;
```

## API Endpoints

### Получить смартфон с изображением:
```bash
GET /api/parser/smartphones/{id}
```

Ответ:
```json
{
  "id": 1,
  "name": "Samsung Galaxy S23",
  "image_url": "https://www.nix.ru/images/products/samsung-s23.jpg",
  ...
}
```

### Фильтр по наличию изображений:
Можно добавить в будущем:
```bash
GET /api/parser/smartphones?has_image=true
```

## Рекомендации

1. **Валидация URL** - проверяйте, что URL начинается с `http://` или `https://`
2. **Обработка ошибок** - на frontend обрабатывайте случаи, когда изображение не загружается
3. **Placeholder** - используйте placeholder изображение для товаров без фото
4. **Lazy loading** - загружайте изображения по требованию для лучшей производительности

## Альтернатива: Локальное хранение

Если в будущем понадобится скачивать изображения локально:

1. Создать папку `static/images/smartphones/`
2. Скачивать изображения при парсинге
3. Сохранять путь к локальному файлу в БД
4. Отдавать через FastAPI static files

Но для курсовой работы **хранение URL достаточно** и проще в реализации.

