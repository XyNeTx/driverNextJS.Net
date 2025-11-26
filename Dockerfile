FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-

RUN docker pull node:24-alpine

RUN docker run -it --rm --entrypoint sh node:24-alpine
RUN npm -v
RUN node -v

WORKDIR /

COPY *.sln /
COPY /backend /backend
COPY /tests /tests
COPY /frontend /frontend

RUN dotnet restore
RUN dotnet build -c Release
RUN dotnet test -c Release

WORKDIR /backend
RUN dotnet publish -c Release -o out

WORKDIR /frontend
RUN npm install
RUN npm run build

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /backend
COPY --from=build-env /backend/out .
EXPOSE 5272
ENTRYPOINT ["dotnet", "driver_api.dll"]