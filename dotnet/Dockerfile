# Устанавливаем базовые образы
FROM mcr.microsoft.com/dotnet/aspnet:8.0-windowsservercore-ltsc2022 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0-windowsservercore-ltsc2022 AS build
WORKDIR /src

# Правильный путь к .csproj
COPY ["QR-Code-generator/QR-Code-generator.csproj", "QR-Code-generator/"]
RUN dotnet restore "QR-Code-generator/QR-Code-generator.csproj"

COPY . .
WORKDIR "/src/QR-Code-generator"
RUN dotnet publish "QR-Code-generator.csproj" -c Release -o /app

# Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "QR-Code-generator.dll"]
