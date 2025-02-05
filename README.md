# hello-world-dotnet

Hello World .NET Core Web Api

This is a sandbox repo for testing out .NET Core services

# Build

```sh
cd HelloWord

docker build --platform=linux/amd64 -t hello-world-dotnet:9.0 .
```

# Run

```sh
docker run -p 8080:8080 --name hello hello-world-dotnet:9.0
```
