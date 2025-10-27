FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base
WORKDIR /source

COPY BlazorGameQuest1234.sln .
COPY AuthenticationServices/AuthenticationServices.csproj AuthenticationServices/
COPY BlazorGame.Client/BlazorGame.Client.csproj BlazorGame.Client/
COPY BlazorGame.GameService/BlazorGame.GameService.csproj BlazorGame.GameService/
COPY BlazorGame.Tests/BlazorGame.Tests.csproj BlazorGame.Tests/
COPY SharedModels/SharedModels.csproj SharedModels/
RUN dotnet restore BlazorGameQuest1234.sln

COPY . .

FROM base AS build-gameservice
WORKDIR /source/BlazorGame.GameService
RUN dotnet publish -c Release -o /app/publish

FROM base AS build-client
WORKDIR /source/BlazorGame.Client
RUN dotnet publish -c Release -o /app/publish



FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS gameservice
WORKDIR /app
COPY --from=build-gameservice /app/publish .
ENTRYPOINT ["dotnet", "BlazorGame.GameService.dll"]


FROM nginx:alpine AS client
COPY --from=build-client /app/publish/wwwroot /usr/share/nginx/html