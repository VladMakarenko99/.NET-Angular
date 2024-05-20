FROM mcr.microsoft.com/dotnet/sdk:7.0.100 AS build-env
WORKDIR /app

# Copy everything
COPY *.csproj ./
RUN dotnet restore 
# Build and publish a release
COPY . ./
RUN dotnet publish -c Release -o out

ARG TARGETOS TARGETARCH
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0.0
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "API.dll"]