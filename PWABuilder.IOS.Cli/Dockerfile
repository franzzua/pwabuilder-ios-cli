FROM  mcr.microsoft.com/dotnet/runtime:8.0

COPY bin/publish/ /app/

WORKDIR /app

ENTRYPOINT ["./PWABuilder.IOS.Cli"]
