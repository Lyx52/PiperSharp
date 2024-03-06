FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PiperSharp/PiperSharp.csproj", "PiperSharp/"]
RUN dotnet restore "PiperSharp/PiperSharp.csproj"
COPY . .
WORKDIR "/src/PiperSharp"
RUN dotnet build "PiperSharp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PiperSharp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PiperSharp.dll"]
