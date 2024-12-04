FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Используем образ .NET SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Устанавливаем рабочую директорию
WORKDIR /app

# Копируем все файлы проекта в контейнер
COPY . .

# Сборка и публикация проекта
RUN dotnet publish -c Release -o /out

# Финальный образ для запуска
FROM mcr.microsoft.com/dotnet/runtime:8.0

WORKDIR /app

# Установка системных библиотек, необходимых для SkiaSharp и Python
RUN apt-get update && apt-get install -y \
    libfontconfig1 \
    libfreetype6 \
    libx11-6 \
    libxext6 \
    libxrender1 \
    libglib2.0-0 \
    libc6 \
    libgcc1 \
    libstdc++6 \
    libasound2 \
    libpulse0 \
    libxcomposite1 \
    libxdamage1 \
    libxfixes3 \
    libxcursor1 \
    libxi6 \
    libxrandr2 \
    libpangocairo-1.0-0 \
    libpango-1.0-0 \
    libcairo2 \
    libxinerama1 \
    libgtk-3-0 \
    python3 \
    python3-pip \
    python3-venv \
    libpython3.11 \
    && apt-get clean && rm -rf /var/lib/apt/lists/*

# Создание виртуального окружения и установка библиотеки requests
RUN python3 -m venv /venv \
    && . /venv/bin/activate \
    && pip install --no-cache-dir requests

# Устанавливаем виртуальное окружение в PATH
ENV PATH="/venv/bin:$PATH"

# Копируем собранные файлы из предыдущего этапа
COPY --from=build /out .

# Команда для запуска приложения
ENTRYPOINT ["dotnet", "QR-Code-generator.dll"]
