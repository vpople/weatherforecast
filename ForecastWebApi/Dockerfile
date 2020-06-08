#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ForecastWebApi/ForecastWebApi.csproj", "ForecastWebApi/"]
RUN dotnet restore "ForecastWebApi/ForecastWebApi.csproj"
COPY . .
WORKDIR "/src/ForecastWebApi"
RUN dotnet build "ForecastWebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ForecastWebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ForecastWebApi.dll"]
