FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
COPY . /publish
WORKDIR /publish
EXPOSE 5001

#指定容器运行端口
ENV ASPNETCORE_URLS=http://+:5001
#EXPOSE 443#

##FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
##WORKDIR /src
##COPY ["backend/FuYang.Api/FuYang.Api.csproj", "backend/FuYang.Api/"]
##COPY ["backend/FuYang.Infrastructure/FuYang.Infrastructure.csproj", "backend/FuYang.Infrastructure/"]
##COPY ["backend/FuYang.Core/FuYang.Core.csproj", "backend/FuYang.Core/"]
##RUN dotnet restore "backend/FuYang.Api/FuYang.Api.csproj"
##COPY . .
##WORKDIR "/src/backend/FuYang.Api"
##RUN dotnet build "FuYang.Api.csproj" -c Release -o /app/build
##
##FROM build AS publish
##RUN dotnet publish "FuYang.Api.csproj" -c Release -o /app/publish
##
##FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FuYang.Api.dll"]