FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY YellowBlossom.sln ./
COPY YellowBlossom.Presentation/Presentation.Application.csproj src/YellowBlossom.Presentation/
COPY YellowBlossom.Application/YellowBlossom.Application.csproj src/YellowBlossom.Application/
COPY YellowBlossom.Domain/YellowBlossom.Domain.csproj src/YellowBlossom.Domain/
COPY YellowBlossom.Infrastructure/YellowBlossom.Infrastructure.csproj src/YellowBlossom.Infrastructure/

RUN dotnet restore "YellowBlossom.sln"

COPY . .

# Build the application
WORKDIR /src/YellowBlossom.Presentation
RUN dotnet build "YellowBlossom.Presentation.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "YellowBlossom.Presentation.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update && \
    apt-get install -y --no-install-recommends icu-libs && \
    rm -rf /var/lib/apt/lists/*

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "YellowBlossom.Presentation.dll"]
