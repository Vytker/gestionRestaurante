# Consulte https://aka.ms/customizecontainer para aprender a personalizar su contenedor de depuración y cómo Visual Studio usa este Dockerfile para compilar sus imágenes para una depuración más rápida.

# Esta fase se usa cuando se ejecuta desde VS en modo rápido (valor predeterminado para la configuración de depuración)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Esta fase se usa para compilar el proyecto de servicio
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Servicios/Turnos/Turnos.Api/Turnos.Api.csproj", "Servicios/Turnos/Turnos.Api/"]
COPY ["Servicios/Turnos/Turnos.Application/Turnos.Application.csproj", "Servicios/Turnos/Turnos.Application/"]
COPY ["Servicios/Turnos/Turnos.Infrastructure/Turnos.Infrastructure.csproj", "Servicios/Turnos/Turnos.Infrastructure/"]
COPY ["Servicios/Turnos/Turnos.Domain/Turnos.Domain.csproj", "Servicios/Turnos/Turnos.Domain/"]
RUN dotnet restore "./Servicios/Turnos/Turnos.Api/Turnos.Api.csproj"
COPY . .
WORKDIR "/src/Servicios/Turnos/Turnos.Api"
RUN dotnet build "./Turnos.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase se usa para publicar el proyecto de servicio que se copiará en la fase final.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Turnos.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase se usa en producción o cuando se ejecuta desde VS en modo normal (valor predeterminado cuando no se usa la configuración de depuración)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Turnos.Api.dll"]