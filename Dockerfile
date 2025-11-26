FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

RUN apt-get update \
    && curl -fsSL https://deb.nodesource.com/setup_22.x | bash - \
    && apt-get install -y nodejs \
    && node -v \
    && npm -v

WORKDIR /
COPY *.sln /
COPY /backend /backend
COPY /tests /tests
COPY /frontend /frontend

# RUN dotnet restore \
#     && dotnet build -c Release \
#     && dotnet test -c Release


# Restore ONLY backend, not frontend .esproj
RUN dotnet restore backend/driver_api.csproj

RUN dotnet build backend/driver_api.csproj -c Release
RUN dotnet test tests/driver_tests.csproj -c Release

RUN dotnet publish backend/driver_api.csproj -c Release -o /out

WORKDIR /backend
RUN dotnet publish -c Release -o out

WORKDIR /frontend
RUN npm install \
    && npm run build

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
# ENV ASPNETCORE_URLS=http://+:5272
# ENV ASPNETCORE_ENVIRONMENT=Development
WORKDIR /backend
COPY --from=build-env /backend/out .
EXPOSE 5272
ENTRYPOINT ["dotnet", "driver_api.dll"]