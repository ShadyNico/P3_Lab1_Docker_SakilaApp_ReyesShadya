# Etapa 1: compilación de SakilaApp
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copiar el archivo del proyecto y restaurar dependencias
COPY ["SakilaApp.csproj", "./"]
RUN dotnet restore "SakilaApp.csproj"

# Copiar el resto del proyecto
COPY . .

# Publicar la aplicación
RUN dotnet publish "SakilaApp.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# Etapa 2: ejecución de SakilaApp
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

# Copiar el resultado publicado
COPY --from=build /app/publish .

# Configurar ASP.NET Core para escuchar dentro del contenedor
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 8080

ENTRYPOINT ["dotnet", "SakilaApp.dll"]