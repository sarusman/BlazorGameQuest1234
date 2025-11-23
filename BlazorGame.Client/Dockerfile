FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

COPY BlazorGameQuest1234.sln .
COPY AuthenticationServices/AuthenticationServices.csproj AuthenticationServices/
COPY BlazorGame.Client/BlazorGame.Client.csproj BlazorGame.Client/
COPY BlazorGame.GameService/BlazorGame.GameService.csproj BlazorGame.GameService/
COPY BlazorGame.Tests/BlazorGame.Tests.csproj BlazorGame.Tests/
COPY SharedModels/SharedModels.csproj SharedModels/
RUN dotnet restore BlazorGameQuest1234.sln

COPY . .

WORKDIR /source/BlazorGame.Client
RUN dotnet publish -c Release -o /app/publish

FROM nginx:alpine
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY default.conf /etc/nginx/conf.d/default.conf
