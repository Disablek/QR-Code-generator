# Базовый образ для ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файл проекта и восстанавливаем зависимости
COPY ["QR-Code-generator.csproj", "./"]
RUN dotnet restore QR-Code-generator.csproj

# Копируем исходные файлы и компилируем проект
COPY . . 
RUN dotnet publish QR-Code-generator.csproj -c Release -o /out

# Финальный этап
FROM base AS final
WORKDIR /app
COPY --from=build /out . 
ENTRYPOINT ["dotnet", "QR-Code-generator.dll"]
