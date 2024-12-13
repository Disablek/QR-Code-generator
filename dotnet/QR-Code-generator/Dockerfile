# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Устанавливаем рабочую директорию
WORKDIR /app

# Копируем файл проекта
COPY *.csproj ./

# Восстанавливаем зависимости
RUN dotnet restore

# Копируем остальной код
COPY . ./

# Сборка и публикация проекта
RUN dotnet publish -c Release -o /out

# Этап выполнения
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Устанавливаем рабочую директорию
WORKDIR /app

# Копируем собранное приложение из этапа сборки
COPY --from=build /out ./

# Открываем порт для HTTP (или сокетов)
EXPOSE 5000

# Указываем команду для запуска приложения
ENTRYPOINT ["dotnet", "QR-Code-generator.dll"]
