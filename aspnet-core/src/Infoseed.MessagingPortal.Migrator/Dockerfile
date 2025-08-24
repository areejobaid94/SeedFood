#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Infoseed.MessagingPortal.Migrator/Infoseed.MessagingPortal.Migrator.csproj", "src/Infoseed.MessagingPortal.Migrator/"]
COPY ["src/Infoseed.MessagingPortal.EntityFrameworkCore/Infoseed.MessagingPortal.EntityFrameworkCore.csproj", "src/Infoseed.MessagingPortal.EntityFrameworkCore/"]
COPY ["src/Infoseed.MessagingPortal.Core/Infoseed.MessagingPortal.Core.csproj", "src/Infoseed.MessagingPortal.Core/"]
COPY ["src/Infoseed.MessagingPortal.Core.Shared/Infoseed.MessagingPortal.Core.Shared.csproj", "src/Infoseed.MessagingPortal.Core.Shared/"]
RUN dotnet restore "src/Infoseed.MessagingPortal.Migrator/Infoseed.MessagingPortal.Migrator.csproj"
COPY . .
WORKDIR "/src/src/Infoseed.MessagingPortal.Migrator"
RUN dotnet build "Infoseed.MessagingPortal.Migrator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Infoseed.MessagingPortal.Migrator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Infoseed.MessagingPortal.Migrator.dll"]
