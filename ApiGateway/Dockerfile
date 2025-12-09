FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source


COPY BlazorGameQuest1234.sln .
COPY ApiGateway/ApiGateway.csproj ApiGateway/
COPY AuthenticationServices/AuthenticationServices.csproj AuthenticationServices/
COPY BlazorGame.GameService/BlazorGame.GameService.csproj BlazorGame.GameService/
COPY SharedModels/SharedModels.csproj SharedModels/
COPY BlazorGame.Client/BlazorGame.Client.csproj BlazorGame.Client/
COPY BlazorGame.Tests/BlazorGame.Tests.csproj BlazorGame.Tests/

RUN dotnet restore BlazorGameQuest1234.sln

COPY . .

WORKDIR /source/ApiGateway
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/out ./

ENV ASPNETCORE_URLS=http://+:5001
EXPOSE 5001

ENTRYPOINT ["dotnet", "ApiGateway.dll"]
