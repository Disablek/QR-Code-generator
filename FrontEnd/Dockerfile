# Используем базовый образ .NET SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Устанавливаем рабочую директорию
WORKDIR /app

# Копируем файлы проекта в контейнер
COPY . .

# Сборка проекта
RUN dotnet publish -c Release -r win-x64 --self-contained true -o /out

# Финальный образ
FROM mcr.microsoft.com/dotnet/runtime:8.0

# Устанавливаем рабочую директорию
WORKDIR /app

# Копируем собранное приложение из предыдущего этапа
COPY --from=build /out .

# Устанавливаем точку входа для запуска приложения
ENTRYPOINT ["QR-Code-generator.exe"]
