FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS builder
LABEL maintainer="Пупов Иван"
ENV ENVIRONMENT=stage
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src
COPY ["BlogAPI.csproj", "./"]
RUN dotnet restore "./BlogAPI.csproj"
COPY . .
WORKDIR /src
RUN dotnet build "./BlogAPI.csproj" -c Release -o /app

FROM build AS final
RUN dotnet publish "BlogAPI.csproj" -c Release -o /app /p:UseAppHost=false

FROM builder AS finish
COPY --from=build /app .
CMD ["dotnet", "/BlogAPI.dll"]


