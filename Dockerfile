FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Install ngrok, curl and jq
RUN apt-get update && apt-get install -y wget unzip curl jq
RUN wget https://bin.equinox.io/c/bNyj1mQVY4c/ngrok-v3-stable-linux-amd64.tgz \
    && tar xvzf ngrok-v3-stable-linux-amd64.tgz -C /usr/local/bin

# Copy startup script
COPY start.sh .
RUN chmod +x start.sh

ENTRYPOINT ["./start.sh"]

# docker build -t monkey-island .
# docker run --env-file .env -p 8080:8080 monkey-island