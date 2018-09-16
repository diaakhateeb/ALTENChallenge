FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["VehicleStatusLiveMonitor/VehicleStatusLiveMonitor.csproj", "VehicleStatusLiveMonitor/"]
COPY ["DataDomainService.Patterns/DataDomainService.Patterns.csproj", "DataDomainService.Patterns/"]
COPY ["DataDomainService.Vehicle/DataDomainService.Vehicle.csproj", "DataDomainService.Vehicle/"]
COPY ["DataDomainService.ConnectionStatus/DataDomainService.ConnectionStatus.csproj", "DataDomainService.ConnectionStatus/"]
COPY ["RabbitMQEventBus/RabbitMQEventBus.csproj", "RabbitMQEventBus/"]
COPY ["DataDomainService.Models/DataDomainService.Models.csproj", "DataDomainService.Models/"]
COPY ["DataDomainService.GenericsContext/DataDomainService.GenericsContext.csproj", "DataDomainService.GenericsContext/"]
COPY ["Logging/Logging.csproj", "Logging/"]
COPY ["DataDomainService.Customer/DataDomainService.Customer.csproj", "DataDomainService.Customer/"]
RUN dotnet restore "VehicleStatusLiveMonitor/VehicleStatusLiveMonitor.csproj"
COPY . .
WORKDIR "/src/VehicleStatusLiveMonitor"
RUN dotnet build "VehicleStatusLiveMonitor.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "VehicleStatusLiveMonitor.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "VehicleStatusLiveMonitor.dll"]