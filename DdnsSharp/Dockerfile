#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 54321

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DdnsSharp/DdnsSharp.csproj", "DdnsSharp/"]
COPY ["DdnsSharp.Core/DdnsSharp.Core.csproj", "DdnsSharp.Core/"]
COPY ["DdnsSharp.IServices/DdnsSharp.IServices.csproj", "DdnsSharp.IServices/"]
COPY ["DdnsSharp.Model/DdnsSharp.Model.csproj", "DdnsSharp.Model/"]
COPY ["DdnsSharp.EFCore/DdnsSharp.EFCore.csproj", "DdnsSharp.EFCore/"]
COPY ["DdnsSharp.HostedService/DdnsSharp.HostedService.csproj", "DdnsSharp.HostedService/"]
COPY ["DdnsSharp.Services/DdnsSharp.Services.csproj", "DdnsSharp.Services/"]
COPY ["DdnsSharp.IRepository/DdnsSharp.IRepository.csproj", "DdnsSharp.IRepository/"]
COPY ["DdnsSharp.Repository/DdnsSharp.Repository.csproj", "DdnsSharp.Repository/"]
RUN dotnet restore "DdnsSharp/DdnsSharp.csproj"
COPY . .
WORKDIR "/src/DdnsSharp"
RUN dotnet build "DdnsSharp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DdnsSharp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DdnsSharp.dll"]