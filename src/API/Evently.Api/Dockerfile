FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
RUN apt-get update \
    && apt-get install -y krb5-user \
    && rm -rf /var/lib/apt/lists/*
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
ENV HUSKY=0
WORKDIR /src
COPY ["Directory.Build.props", "Directory.Packages.props", "./"]
COPY ["src/API/Evently.Api/Evently.Api.csproj", "src/API/Evently.Api/"]

COPY ["src/Common/Evently.Common.Application/Evently.Common.Application.csproj", "src/Common/Evently.Common.Application/"]
COPY ["src/Common/Evently.Common.Domain/Evently.Common.Domain.csproj", "src/Common/Evently.Common.Domain/"]
COPY ["src/Common/Evently.Common.Infrastructure/Evently.Common.Infrastructure.csproj", "src/Common/Evently.Common.Infrastructure/"]
COPY ["src/Common/Evently.Common.Presentation/Evently.Common.Presentation.csproj", "src/Common/Evently.Common.Presentation/"]

COPY ["src/Modules/Events/Evently.Modules.Events.Infrastructure/Evently.Modules.Events.Infrastructure.csproj", "src/Modules/Events/Evently.Modules.Events.Infrastructure/"]
COPY ["src/Modules/Events/Evently.Modules.Events.Application/Evently.Modules.Events.Application.csproj", "src/Modules/Events/Evently.Modules.Events.Application/"]
COPY ["src/Modules/Events/Evently.Modules.Events.Domain/Evently.Modules.Events.Domain.csproj", "src/Modules/Events/Evently.Modules.Events.Domain/"]
COPY ["src/Modules/Events/Evently.Modules.Events.Presentation/Evently.Modules.Events.Presentation.csproj", "src/Modules/Events/Evently.Modules.Events.Presentation/"]

RUN dotnet restore "./src/API/Evently.Api/Evently.Api.csproj"
COPY . .
WORKDIR "/src/src/API/Evently.Api"
RUN dotnet build "./Evently.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Evently.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Evently.Api.dll"]
