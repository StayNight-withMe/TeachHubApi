FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["testApi/testApi.csproj", "testApi/"]
COPY ["Infrastructure/infrastructure.csproj", "Infrastructure/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Logger/Logger.csproj", "Logger/"]

RUN dotnet restore "testApi/testApi.csproj"

COPY . .
WORKDIR "/src/testApi"
RUN dotnet build "testApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "testApi.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .


RUN mkdir -p BloomFilter && chmod 777 BloomFilter

# 6. Указываем, какую команду выполнить при старте контейнера
ENTRYPOINT ["dotnet", "testApi.dll"]