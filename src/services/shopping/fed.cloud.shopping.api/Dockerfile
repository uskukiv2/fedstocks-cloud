#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/services/shopping/fed.cloud.shopping.api/fed.cloud.shopping.api.csproj", "src/services/shopping/fed.cloud.shopping.api/"]
RUN dotnet restore "src/services/shopping/fed.cloud.shopping.api/fed.cloud.shopping.api.csproj"
COPY . .
WORKDIR "/src/src/services/shopping/fed.cloud.shopping.api"
RUN dotnet build "fed.cloud.shopping.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "fed.cloud.shopping.api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "fed.cloud.shopping.api.dll"]