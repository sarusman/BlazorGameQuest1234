# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["BlazorGame.Client/BlazorGame.Client.csproj", "BlazorGame.Client/"]
COPY ["BlazorGame.Core/BlazorGame.Core.csproj", "BlazorGame.Core/"]
COPY ["SharedModels/SharedModels.csproj", "SharedModels/"]

# Restore dependencies
RUN dotnet restore "BlazorGame.Client/BlazorGame.Client.csproj"

# Copy remaining source code
COPY . .

# Publish the WebAssembly app
RUN dotnet publish "BlazorGame.Client/BlazorGame.Client.csproj" -c Release -o /app/publish

# Runtime stage - use nginx to serve static files
FROM nginx:stable-alpine

# Remove default config
RUN rm -rf /etc/nginx/conf.d/default.conf

# Copy published Blazor WebAssembly files to nginx root
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

# Fix permissions - very permissive for now
RUN chmod -R 777 /usr/share/nginx/html && \
    find /usr/share/nginx/html -type f -exec chmod 644 {} \; && \
    find /usr/share/nginx/html -type d -exec chmod 755 {} \;

# Copy custom nginx config template
COPY default.conf.template /etc/nginx/templates/default.conf.template

EXPOSE 5050