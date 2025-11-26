FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /

# Copy csproj files
COPY /backend/*.csproj /backend/
COPY /tests/*.csproj /tests/
COPY /frontend/*.esproj /frontend/

# Build frontend
WORKDIR /frontend
RUN npm install
RUN npm run build

WORKDIR /

# Copy solution files and restore as distinct layers
COPY *.sln .
RUN dotnet restore

# Copy and build the rest of the application
COPY . .
RUN dotnet build -c Release
RUN dotnet test --no-build -c Release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /backend
COPY --from=build-env /backend/out .
ENTRYPOINT ["dotnet", "driver_api.dll"]