# ---- build ---- 
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build 
WORKDIR /src 
# 1. Копируем nuget.config и локальные пакеты ДО restore 
# Это нужно, чтобы dotnet restore нашёл пакеты Менеджера и Руководителя 
COPY nuget.config . 
COPY packages/ packages/ 
# 2. Копируем .csproj файлы обоих проектов для кэширования restore 
COPY StudentApi.Contracts/StudentApi.Contracts.csproj StudentApi.Contracts/ 
COPY StudentApi/StudentApi.csproj StudentApi/ 
# 3. Восстанавливаем зависимости 
RUN dotnet restore StudentApi/StudentApi.csproj 
# 4. Копируем весь исходный код обоих проектов 
COPY StudentApi.Contracts/ StudentApi.Contracts/ 
COPY StudentApi/ StudentApi/ 
# 5. Публикуем основной проект 
RUN dotnet publish StudentApi/StudentApi.csproj \
    -c Release -o /app/publish --no-restore 
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