#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Infoseed.MessagingPortal.Web.Public/Infoseed.MessagingPortal.Web.Public.csproj", "src/Infoseed.MessagingPortal.Web.Public/"]
COPY ["src/Infoseed.MessagingPortal.Web.Core/Infoseed.MessagingPortal.Web.Core.csproj", "src/Infoseed.MessagingPortal.Web.Core/"]
COPY ["src/Infoseed.MessagingPortal.Application/Infoseed.MessagingPortal.Application.csproj", "src/Infoseed.MessagingPortal.Application/"]
COPY ["src/Infoseed.MessagingPortal.Application.Shared/Infoseed.MessagingPortal.Application.Shared.csproj", "src/Infoseed.MessagingPortal.Application.Shared/"]
COPY ["src/Infoseed.MessagingPortal.Core.Shared/Infoseed.MessagingPortal.Core.Shared.csproj", "src/Infoseed.MessagingPortal.Core.Shared/"]
COPY ["src/Infoseed.MessagingPortal.Core/Infoseed.MessagingPortal.Core.csproj", "src/Infoseed.MessagingPortal.Core/"]
COPY ["src/Infoseed.MessagingPortal.EntityFrameworkCore/Infoseed.MessagingPortal.EntityFrameworkCore.csproj", "src/Infoseed.MessagingPortal.EntityFrameworkCore/"]
COPY ["src/Infoseed.MessagingPortal.GraphQL/Infoseed.MessagingPortal.GraphQL.csproj", "src/Infoseed.MessagingPortal.GraphQL/"]
RUN dotnet restore "src/Infoseed.MessagingPortal.Web.Public/Infoseed.MessagingPortal.Web.Public.csproj"
COPY . .
WORKDIR "/src/src/Infoseed.MessagingPortal.Web.Public"
RUN dotnet build "Infoseed.MessagingPortal.Web.Public.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Infoseed.MessagingPortal.Web.Public.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Infoseed.MessagingPortal.Web.Public.dll"]
