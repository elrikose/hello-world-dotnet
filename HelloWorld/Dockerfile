FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
EXPOSE 8080
EXPOSE 8081
ENV ASPNETCORE_URLS=http://*:8080/
RUN addgroup --group softdocs --gid 1000 && \
    adduser --uid 1000 --gid 1000 softdocs
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["HelloWorld.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Development
RUN chown -R softdocs:softdocs /app
USER softdocs:softdocs
ENTRYPOINT ["dotnet", "HelloWorld.dll"]
