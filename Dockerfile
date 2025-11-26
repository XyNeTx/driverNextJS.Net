FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /backend

# Copy csproj and restore as distinct layers
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