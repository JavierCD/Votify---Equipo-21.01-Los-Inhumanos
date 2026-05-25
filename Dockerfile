FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Votify.Core/Votify.Core.csproj", "Votify.Core/"]
COPY ["Votify.Services/Votify.Services.csproj", "Votify.Services/"]
COPY ["Votify.Persistence/Votify.Persistence.csproj", "Votify.Persistence/"]
COPY ["Votify.Client/Votify.UI/Votify.UI.csproj", "Votify.Client/Votify.UI/"]
COPY ["Votify.Client/Votify.Web/Votify.Web.csproj", "Votify.Client/Votify.Web/"]

RUN dotnet restore "Votify.Client/Votify.Web/Votify.Web.csproj"

COPY . .

WORKDIR "/src/Votify.Client/Votify.Web"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
EXPOSE ${PORT:-8080}

ENTRYPOINT ["dotnet", "Votify.Web.dll"]
