# Use an official .NET SDK image for building the client
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-client
WORKDIR /app

# Copy client project files and restore dependencies
COPY Client/Client.csproj ./Client/
COPY BaseLibrary/BaseLibrary.csproj ./BaseLibrary/
COPY ClientLibrary/ClientLibrary.csproj ./ClientLibrary/
WORKDIR /app/Client
RUN dotnet restore

# Copy everything else and build
COPY Client/. ./
COPY ../BaseLibrary/. ../BaseLibrary/
COPY ../ClientLibrary/. ../ClientLibrary/

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

# List contents of out directory explicitly
RUN ls -la out || echo "out directory not found"

# Check if wwwroot exists (Blazor WASM output)
RUN if [ ! -d "out/wwwroot" ]; then \
        echo "wwwroot directory is missing in out!"; \
        exit 1; \
    else \
        echo "wwwroot directory exists in out."; \
    fi

# Build runtime image using Nginx to serve static files
FROM nginx:alpine AS runtime-client
WORKDIR /usr/share/nginx/html
COPY --from=build-client /app/Client/out/wwwroot .
COPY Client/nginx.conf /etc/nginx/nginx.conf  
EXPOSE 80
ENTRYPOINT ["nginx", "-g", "daemon off;"]