# Base Image
FROM mcr.microsoft.com/dotnet/sdk:8.0 As build-env
# FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0.101 As build-env
# FROM mcr.microsoft.com/dotnet/runtime:7.0-bullseye-slim-amd64 As build-env
WORKDIR /app
EXPOSE 8888
EXPOSE 443

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

# Final Image
FROM mcr.microsoft.com/dotnet/sdk:8.0
# FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0.101
# FROM mcr.microsoft.com/dotnet/runtime:7.0-bullseye-slim-amd64
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "DiceApp.dll" ]