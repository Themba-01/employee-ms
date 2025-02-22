# Use an official .NET SDK image for building the server
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-server
WORKDIR /app

# Copy server project files and restore dependencies
COPY Server/Server.csproj ./Server/
COPY BaseLibrary/BaseLibrary.csproj ./BaseLibrary/
COPY ServerLibrary/ServerLibrary.csproj ./ServerLibrary/
WORKDIR /app/Server
RUN dotnet restore

# Copy everything else and build
COPY Server/. ./
COPY ../BaseLibrary/. ../BaseLibrary/
COPY ../ServerLibrary/. ../ServerLibrary/
COPY Server/appsettings.json ./appsettings.json  

# Log before publishing
RUN echo "Files before publishing:" && ls -lR

# Ensure packages are restored for any additional references in source files
RUN dotnet restore

# Clean before publishing to ensure a fresh build
RUN dotnet clean

# Publish with verbose logging to see exactly what's happening
RUN dotnet publish -c Release -o out --no-restore --verbosity detailed

# Log after publishing
RUN echo "Files after publishing:" && ls -lR

# Check if Server.dll exists
RUN if [ ! -f "out/Server.dll" ]; then \
        echo "Server.dll is missing!"; \
        exit 1; \
    else \
        echo "Server.dll exists."; \
    fi

# Ensure out directory exists
RUN if [ ! -d "out" ]; then \
        echo "out directory does not exist!"; \
        exit 1; \
    else \
        echo "out directory exists."; \
    fi

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime-server
WORKDIR /app
COPY --from=build-server /app/Server/out .  
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "Server.dll"]