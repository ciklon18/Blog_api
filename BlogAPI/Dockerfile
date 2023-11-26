FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS builder
LABEL maintainer="Пупов Иван"
ENV ENVIRONMENT=stage
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /app
COPY ["BlogAPI.csproj", "./"]
RUN dotnet restore "./BlogAPI.csproj"


FROM build AS final
RUN dotnet publish "BlogAPI.csproj" -c Release -o /app


COPY --from=build /app .
CMD ["dotnet", "/BlogAPI.dll"]
