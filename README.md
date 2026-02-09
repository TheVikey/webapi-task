# WebAPI Task

## Описание
Web API приложение для обработки CSV файлов с измерениями.

## Установка и запуск

1. Установите PostgreSQL и создайте базу данных
2. Обновите строку подключения в `appsettings.json`:
json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=WebApiTaskDb;Username=postgres;Password=ваш_пароль"
}

Примените миграции:
dotnet ef migrations add InitialCreate
dotnet ef database update

Запустите приложение:
dotnet run
