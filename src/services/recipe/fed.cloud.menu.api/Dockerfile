FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/services/recipe/fed.cloud.recipe.api/fed.cloud.recipe.api.csproj", "src/services/recipe/fed.cloud.recipe.api/"]
RUN dotnet restore "src/services/recipe/fed.cloud.recipe.api/fed.cloud.recipe.api.csproj"
COPY . .
WORKDIR "/src/src/services/recipe/fed.cloud.recipe.api"
RUN dotnet build "fed.cloud.recipe.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "fed.cloud.recipe.api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "fed.cloud.recipe.api.dll"]
