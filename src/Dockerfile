# CURL inside container
FROM alpine
RUN apk add --no-cache curl

FROM ubuntu:16.04

# Install prerequisites
RUN apt-get update && apt-get install -y \
curl
CMD /bin/bash

# Use the official image as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use SDK image to build the solution
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TicketMapper.WebApi/TicketMapper.WebApi.csproj", "TicketMapper.WebApi/"]
COPY ["TicketMapper.Application/TicketMapper.Application.csproj", "TicketMapper.Application/"]
COPY ["TicketMapper.Domain/TicketMapper.Domain.csproj", "TicketMapper.Domain/"]
COPY ["TicketMapper.Infrastructure.DocumentService/TicketMapper.Infrastructure.DocumentService.csproj", "TicketMapper.Infrastructure.DocumentService/"]
COPY ["TicketMapper.Infrastructure.Persistence/TicketMapper.Infrastructure.Persistence.csproj", "TicketMapper.Infrastructure.Persistence/"]
# Repeat the COPY command for other projects that your WebApi depends on
RUN dotnet restore "TicketMapper.WebApi/TicketMapper.WebApi.csproj"
COPY . .
WORKDIR "TicketMapper.WebApi"
RUN dotnet build "TicketMapper.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TicketMapper.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TicketMapper.WebApi.dll"]
