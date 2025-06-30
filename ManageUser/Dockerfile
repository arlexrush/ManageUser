# Usar la imagen oficial del SDK de .NET como entorno de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copiar el archivo de solución y los archivos de proyecto primero para aprovechar el almacenamiento en caché de capas de Docker
COPY ManageUser.sln .
COPY ManageUser/ManageUser.csproj ./ManageUser/
COPY ManageUser.Application/ManageUser.Application.csproj ./ManageUser.Application/
COPY ManageUser.Domain/ManageUser.Domain.csproj ./ManageUser.Domain/
COPY ManageUser.Infrastructure/ManageUser.Infrastructure.csproj ./ManageUser.Infrastructure/
# Copiar proyectos de prueba si es necesario para la restauración (generalmente no es necesario para publicar)
# COPY Application.Tests/Application.Tests.csproj ./Application.Tests/
# COPY Domain.Tests/Domain.Tests.csproj ./Domain.Tests/

# Restaurar dependencias para toda la solución
RUN dotnet restore ManageUser.sln

# Copiar el resto del código fuente
COPY . .

# Establecer el directorio de trabajo en el proyecto específico que se está publicando
WORKDIR /source/ManageUser

# Publicar la aplicación
RUN dotnet publish ManageUser.csproj -c Release -o /app/publish --no-restore

# Usar la imagen oficial del runtime de ASP.NET Core para la etapa final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Exponer el puerto en el que escucha la aplicación dentro del contenedor
# docker-compose mapea el puerto 5000 del host al 5000 del contenedor.
# Asegúrate de que tu aplicación ASP.NET Core escuche en el puerto 5000.
# Puedes configurar esto en Program.cs o mediante la variable de entorno ASPNETCORE_URLS en docker-compose.yml
EXPOSE 5000

# Definir el punto de entrada para el contenedor
ENTRYPOINT ["dotnet", "ManageUser.dll"]
