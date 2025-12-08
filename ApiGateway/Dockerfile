# Use the official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ApiGateway.csproj ./ApiGateway/
RUN dotnet restore ./ApiGateway

# Copy the remaining source code
COPY . ./ApiGateway/
RUN dotnet publish ./ApiGateway -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

ENV ASPNETCORE_URLS=http://+:5001
EXPOSE 5001

ENTRYPOINT ["dotnet", "ApiGateway.dll"]
