FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /

# Copy csproj files
COPY /backend/*.csproj /backend/
WORKDIR /backend
RUN dotnet restore
RUN dotnet build -c Release

COPY /tests/*.csproj /tests/
WORKDIR /tests
RUN dotnet restore
RUN dotnet test --no-build -c Release

WORKDIR /backend
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /backend
COPY --from=build-env /backend/out .
ENTRYPOINT ["dotnet", "driver_api.dll"]