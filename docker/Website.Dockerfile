FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
# RUN apt-get update && apt-get install nodejs npm -y
WORKDIR /src
COPY . .
RUN dotnet restore "Website/Website.csproj"
WORKDIR "/src/Website"
RUN dotnet build "Website.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Website.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Website.dll"]

MAINTAINER Tristan Zander <tristannzander@gmail.com>
