# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base


# Set the timezone to Caracas/La Paz and configure locales
RUN apk add --no-cache tzdata icu-libs \
    && cp /usr/share/zoneinfo/America/Caracas /etc/localtime \
    && echo "America/Caracas" > /etc/timezone

# Set environment variables for .NET globalization
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    TZ=America/Caracas \
    LANG=es_BO.UTF-8 \
    LC_ALL=es_BO.UTF-8


WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY /Src /Src

RUN dotnet restore "/Src/External/Presentation/Web.ApinetCore/Web.ApinetCore.csproj"
COPY . .
WORKDIR "/src/Src/External/Presentation/Web.ApinetCore"
RUN dotnet build "./Web.ApinetCore.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Web.ApinetCore.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.ApinetCore.dll"]

# docker build --no-cache -t apipagos:latest .
# docker run -d -p 9013:8080 --network bridge -v /home/managerdevop/docker/app/pagos/uploads:/app/uploads apipagos:latest
