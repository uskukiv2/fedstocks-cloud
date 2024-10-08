#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/gateways/web/fedstocks.cloud.web.api/fedstocks.cloud.web.api.csproj", "src/gateways/web/fedstocks.cloud.web.api/"]
RUN dotnet restore "src/gateways/web/fedstocks.cloud.web.api/fedstocks.cloud.web.api.csproj"
COPY . .
WORKDIR "/src/src/gateways/web/fedstocks.cloud.web.api"
RUN dotnet build "fedstocks.cloud.web.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "fedstocks.cloud.web.api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "fedstocks.cloud.web.api.dll"]