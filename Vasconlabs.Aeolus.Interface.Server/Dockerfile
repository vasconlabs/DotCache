FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Vasconlabs.Aeolus.Interface.Server/Vasconlabs.Aeolus.Interface.Server.csproj", "Vasconlabs.Aeolus.Interface.Server/"]
RUN dotnet restore "Vasconlabs.Aeolus.Interface.Server/Vasconlabs.Aeolus.Interface.Server.csproj"
COPY . .
WORKDIR "/src/Vasconlabs.Aeolus.Interface.Server"
RUN dotnet build "./Vasconlabs.Aeolus.Interface.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Vasconlabs.Aeolus.Interface.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vasconlabs.Aeolus.Interface.Server.dll"]
