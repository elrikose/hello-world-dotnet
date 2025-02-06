# hello-world-dotnet

Hello World .NET Core Web Api

This is a sandbox repo for testing out .NET Core services

# Build Service in Container

```sh
cd HelloWord

docker build --platform=linux/amd64 -t hello-world-dotnet:9.0 .
```

# Run Service Standalone

```sh
docker run -p 8080:8080 --name hello hello-world-dotnet:9.0
```

# Run Service with OpenTelemetry

There is a `docker-compose.yaml` that not only allows you to build the `hello-world-net` service, but also stands up:

- OpenTelemetry
- Prometheus
- Grafana
- Node Exporter

To Run

```sh
docker compose build
docker compose up
```
