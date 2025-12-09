FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

COPY BlazorGameQuest1234.sln .
COPY AuthenticationServices/AuthenticationServices.csproj AuthenticationServices/
COPY BlazorGame.GameService/BlazorGame.GameService.csproj BlazorGame.GameService/
COPY BlazorGame.Client/BlazorGame.Client.csproj BlazorGame.Client/
COPY BlazorGame.Tests/BlazorGame.Tests.csproj BlazorGame.Tests/
COPY SharedModels/SharedModels.csproj SharedModels/
COPY ApiGateway/ApiGateway.csproj ApiGateway/

RUN dotnet restore BlazorGameQuest1234.sln

COPY . .

WORKDIR /source/AuthenticationServices
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5052
EXPOSE 5052

ENTRYPOINT ["dotnet", "AuthenticationServices.dll"]
