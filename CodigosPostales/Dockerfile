#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#ARG BUILD_CONFIGURATION=Release
ARG BUILD_CONFIGURATION=runtime
WORKDIR /src
COPY ["CodigosPostales/CodigosPostales_net.csproj", "CodigosPostales/"]
RUN dotnet restore "./CodigosPostales/CodigosPostales_net.csproj"
COPY . .
WORKDIR "/src/CodigosPostales"
RUN dotnet build "./CodigosPostales_net.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
ARG BUILD_CONFIGURATION=runtime
RUN dotnet publish "./CodigosPostales_net.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CodigosPostales_net.dll"]


#para ejecurar este archivo: docker build --tag codigospostales