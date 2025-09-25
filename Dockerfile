# Use the official ASP.NET Core runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Set environment variables for better performance
ENV ASPNETCORE_URLS=http://+:8080;https://+:8081
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1

# Create non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files in optimal order for layer caching
COPY ["AuraDecor.APIs/AuraDecor.APIs.csproj", "AuraDecor.APIs/"]
COPY ["AuraDecor.Core/AuraDecor.Core.csproj", "AuraDecor.Core/"]
COPY ["AuraDecor.Repository/AuraDecor.Repository.csproj", "AuraDecor.Repository/"]
COPY ["AuraDecor.Services/AuraDecor.Services.csproj", "AuraDecor.Services/"]

# Restore packages (cached layer if project files haven't changed)
RUN dotnet restore "AuraDecor.APIs/AuraDecor.APIs.csproj"

# Copy source code
COPY . .
WORKDIR "/src/AuraDecor.APIs"

# Build with optimizations
RUN dotnet build "AuraDecor.APIs.csproj" -c Release -o /app/build \
    --no-restore

FROM build AS publish
RUN dotnet publish "AuraDecor.APIs.csproj" -c Release -o /app/publish \
    --no-restore \
    /p:UseAppHost=false \
    /p:PublishTrimmed=false \
    /p:PublishSingleFile=false

FROM base AS final
WORKDIR /app

# Copy published app
COPY --from=publish --chown=appuser:appuser /app/publish .

# Create directories with proper permissions
RUN mkdir -p /app/Uploads /app/logs && \
    chown -R appuser:appuser /app && \
    chmod 755 /app/Uploads /app/logs

# Copy seed data with proper permissions  
COPY --chown=appuser:appuser ["AuraDecor.Repository/Data/DataSeed/", "/app/AuraDecor.Repository/Data/DataSeed/"]

# Switch to non-root user
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "AuraDecor.APIs.dll"]