# ---- build ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Сначала копируем только csproj — кэшируем restore
COPY StudentApi.csproj .
RUN dotnet restore

# Потом весь код
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# ---- runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Папка для загрузок документов
RUN mkdir -p /app/uploads/documents

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5003
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 5003

ENTRYPOINT ["dotnet", "StudentApi.dll"]