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

WORKDIR /source/BlazorGame.GameService
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BlazorGame.GameService.dll"]
