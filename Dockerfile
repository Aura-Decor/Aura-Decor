FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AuraDecor.APIs/AuraDecor.APIs.csproj", "AuraDecor.APIs/"]
COPY ["AuraDecor.Core/AuraDecor.Core.csproj", "AuraDecor.Core/"]
COPY ["AuraDecor.Repository/AuraDecor.Repository.csproj", "AuraDecor.Repository/"]
COPY ["AuraDecor.Services/AuraDecor.Services.csproj", "AuraDecor.Services/"]
RUN dotnet restore "AuraDecor.APIs/AuraDecor.APIs.csproj"
COPY . .
WORKDIR "/src/AuraDecor.APIs"
RUN dotnet build "AuraDecor.APIs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuraDecor.APIs.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create directory for uploaded files
RUN mkdir -p /app/Uploads && chmod 777 /app/Uploads

# Copy seed data
COPY ["AuraDecor.Repository/Data/DataSeed/", "/app/AuraDecor.Repository/Data/DataSeed/"]

ENTRYPOINT ["dotnet", "AuraDecor.APIs.dll"]