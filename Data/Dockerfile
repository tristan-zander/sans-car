FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Data.csproj", "Data/"]
RUN dotnet restore "Data/Data.csproj"
COPY . .
WORKDIR "/src/Data"
RUN dotnet build "Data.csproj" -c Release -o /app/build
