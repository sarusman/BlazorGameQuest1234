# Étape 1: Utiliser le SDK .NET 9
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base
WORKDIR /source

# Copier les fichiers projet et restaurer les paquets
COPY BlazorGameQuest1234.sln .
COPY AuthenticationServices/AuthenticationServices.csproj AuthenticationServices/
COPY BlazorGame.Client/BlazorGame.Client.csproj BlazorGame.Client/
COPY BlazorGame.GameService/BlazorGame.GameService.csproj BlazorGame.GameService/
COPY BlazorGame.Tests/BlazorGame.Tests.csproj BlazorGame.Tests/
COPY SharedModels/SharedModels.csproj SharedModels/
RUN dotnet restore BlazorGameQuest1234.sln

# Copier le reste du code source
COPY . .

# Étape 2: Construire le GameService
FROM base AS build-gameservice
WORKDIR /source/BlazorGame.GameService
RUN dotnet publish -c Release -o /app/publish

# Étape 3: Construire le Client
FROM base AS build-client
WORKDIR /source/BlazorGame.Client
RUN dotnet publish -c Release -o /app/publish


# --- Images Finales ---

# Image finale pour GameService avec le runtime .NET 9
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS gameservice
WORKDIR /app
COPY --from=build-gameservice /app/publish .
ENTRYPOINT ["dotnet", "BlazorGame.GameService.dll"]

# Image finale pour le Client (servi par Nginx)
FROM nginx:alpine AS client
COPY --from=build-client /app/publish/wwwroot /usr/share/nginx/html