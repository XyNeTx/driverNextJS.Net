# FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

# RUN apt-get update \
#     && curl -fsSL https://deb.nodesource.com/setup_22.x | bash - \
#     && apt-get install -y nodejs \
#     && node -v \
#     && npm -v

# WORKDIR /
# COPY *.sln /
# COPY /backend /backend
# COPY /tests /tests
# COPY /frontend /frontend

# # Restore ONLY backend, not frontend .esproj
# RUN dotnet restore backend/driver_api.csproj

# RUN dotnet build backend/driver_api.csproj -c Release
# RUN dotnet test tests/driver_tests.csproj -c Release

# WORKDIR /backend
# RUN dotnet publish -c Release -o out

# WORKDIR /frontend
# RUN npm install \
#     && npm run build

# WORKDIR /
# RUN mkdir -p /backend/out/wwwroot
# RUN cp -r /frontend/out/* /backend/out/wwwroot/

# # Build runtime image
# FROM mcr.microsoft.com/dotnet/aspnet:7.0
# ENV ASPNETCORE_URLS=http://+:5272
# ENV ASPNETCORE_ENVIRONMENT=Development

# WORKDIR /backend
# COPY --from=build-env /backend/out .
# EXPOSE 5272
# ENTRYPOINT ["dotnet", "driver_api.dll"]

# Build frontend development without .env file
FROM node:22 AS frontend
WORKDIR /frontend
COPY frontend .
ENV NODE_ENV=development
RUN npm install \
    && npm run build

# Build backend development without .env file
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS backend
WORKDIR /backend
COPY backend .
RUN dotnet publish -c Release -o out

# Copy Next.js generated files into ASP.NET wwwroot
RUN mkdir -p /backend/out/wwwroot
COPY --from=frontend /frontend/out/ /backend/out/wwwroot/

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=backend /backend/out .
ENTRYPOINT ["dotnet", "driver_api.dll"]