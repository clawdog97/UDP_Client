#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Tutorial2/Tutorial2.csproj", "Tutorial2/"]
RUN dotnet restore "Tutorial2/Tutorial2.csproj"
COPY . .
WORKDIR "/src/Tutorial2"
RUN dotnet build "Tutorial2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tutorial2.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tutorial2.dll"]