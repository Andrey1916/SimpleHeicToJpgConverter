FROM mcr.microsoft.com/dotnet/sdk:5.0 as builder
WORKDIR /app

COPY SimpleHeicToJpgConverter/*.csproj ./SimpleHeicToJpgConverter/
RUN dotnet restore /app/SimpleHeicToJpgConverter/SimpleHeicToJpgConverter.csproj

COPY SimpleHeicToJpgConverter ./SimpleHeicToJpgConverter/
RUN dotnet publish /app/SimpleHeicToJpgConverter/SimpleHeicToJpgConverter.csproj -c Release -o publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:5.0 as runner
WORKDIR /app
COPY --from=builder /app/publish .
ENTRYPOINT [ "dotnet", "SimpleHeicToJpgConverter.dll"]