# Use an official .NET SDK image for building the client
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-client
WORKDIR /app

# Copy client project files and restore dependencies
COPY Client/Client.csproj ./Client/
COPY ../BaseLibrary/BaseLibrary.csproj ../BaseLibrary/
COPY ../ClientLibrary/ClientLibrary.csproj ../ClientLibrary/
WORKDIR /app/Client

# Restore with retries for initial dependencies
RUN dotnet restore Client.csproj --no-cache || \
    (echo "Retry 1/3" && sleep 10 && dotnet restore Client.csproj --no-cache) || \
    (echo "Retry 2/3" && sleep 10 && dotnet restore Client.csproj --no-cache) || \
    (echo "Retry 3/3" && sleep 10 && dotnet restore Client.csproj --no-cache)

# Copy only Client-specific files and necessary libs
COPY Client/ ./
COPY ../BaseLibrary/ ../BaseLibrary/
COPY ../ClientLibrary/ ../ClientLibrary/

# Log before publishing
RUN echo "Files before publishing:" && ls -l || dir

# Restore again, targeting Client.csproj explicitly
RUN dotnet restore Client.csproj

# Clean before publishing
RUN dotnet clean Client.csproj

# Publish with verbose logging
RUN dotnet publish Client.csproj -c Release -o out --no-restore --verbosity detailed

# Log after publishing
RUN echo "Files after publishing:" && ls -l out || dir out

# List contents of out directory
RUN if [ -d "out" ]; then ls -l out || dir out; else echo "out directory not found"; fi

# Check if wwwroot exists (Blazor WASM output)
RUN if [ ! -d "out/wwwroot" ]; then echo "wwwroot directory is missing in out!" && exit 1; else echo "wwwroot directory exists in out."; fi

# Build runtime image using Nginx to serve static files
FROM nginx:alpine AS runtime-client
WORKDIR /usr/share/nginx/html
COPY --from=build-client /app/Client/out/wwwroot .
COPY Client/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
ENTRYPOINT ["nginx", "-g", "daemon off;"]