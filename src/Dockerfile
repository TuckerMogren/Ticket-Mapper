# Use the official image as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use SDK image to build the solution
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TicketMapper.WebApi/TicketMapper.WebApi.csproj", "TicketMapper.WebApi/"]
COPY ["TicketMapper.Application/TicketMapper.Application.csproj", "TicketMapper.Application/"]
COPY ["TicketMapper.Domain/TicketMapper.Domain.csproj", "TicketMapper.Domain/"]
COPY ["TicketMapper.Infrastructure.Document/TicketMapper.Infrastructure.Document.csproj", "TicketMapper.Infrastructure.Document/"]
COPY ["TicketMapper.Infrastructure.FileIO/TicketMapper.Infrastructure.FileIO.csproj", "TicketMapper.Infrastructure.FileIO/"]
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
