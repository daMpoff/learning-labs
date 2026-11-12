# MyMarketplaceProject

Учебный API маркетплейса на ASP.NET Core 8 и MySQL.

API считывает настройки из переменных окружения.

- `ConnectionStrings__DefaultConnection` — строка подключения к вашей базе MySQL.
- `JwtSettings__SecretKey` — ключ подписи JWT длиной не менее 32 байт.
- `Smtp__Username` и `Smtp__Password` — учётные данные для отправки кода через `smtp.yandex.ru` при регистрации.

Запустите API командой `dotnet run` из папки проекта.
