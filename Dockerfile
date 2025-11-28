FROM node:22 AS frontend
WORKDIR /frontend
COPY frontend/package*.json ./
RUN npm install
COPY frontend .
RUN npm run build

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