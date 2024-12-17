# Используем Windows SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0-windowsservercore-ltsc2022 AS build
WORKDIR /app

# Копируем файлы проекта
COPY . .

COPY F:\\python-3.11.0-amd64.exe /app/python-installer.exe

# Устанавливаем Python через MSI
RUN powershell -Command \
    Start-Process python-installer.exe -ArgumentList '/quiet InstallAllUsers=1 PrependPath=1' -NoNewWindow -Wait; \
    Remove-Item python-installer.exe

# Проверяем, что Python установлен
RUN python --version

# Устанавливаем необходимые модули Python
#RUN python -m pip install --upgrade pip && \
 #   pip install requests

# Копируем локальный Python скрипт в контейнер
COPY lok.py /app/python/lok.py

# Восстанавливаем зависимости
RUN dotnet restore "QR-Code-generator/QR-Code-generator.csproj"

# Собираем проект
RUN dotnet publish "QR-Code-generator/QR-Code-generator.csproj" -c Release -o /app

# Создаем папку для загрузок
RUN mkdir C:\app\uploads

# Указываем порт
EXPOSE 5000

# Указываем команду запуска приложения
ENTRYPOINT ["dotnet", "QR-Code-generator.dll"]
