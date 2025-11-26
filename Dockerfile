FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

RUN curl -fsSL https://deb.nodesource.com/setup_22.x | bash - \
    && apt-get install -y nodejs \
    && node -v && npm -v

WORKDIR /
COPY *.sln /
COPY /backend /backend
COPY /tests /tests
COPY /frontend /frontend

RUN dotnet restore \
    && dotnet build -c Release \
    && dotnet test -c Release

WORKDIR /backend
RUN dotnet publish -c Release -o out

WORKDIR /frontend
RUN npm install \
    && npm run build

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /backend
COPY --from=build-env /backend/out .
EXPOSE 5272
ENTRYPOINT ["dotnet", "driver_api.dll"]