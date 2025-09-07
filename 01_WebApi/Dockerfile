# Comments with build/push commands
# docker build -f 01_WebApi/Dockerfile -t vinicius1183/visionary-analyticts-webapi:v1.0.0 .
# docker push vinicius1183/visionary-analyticts-webapi:v1.0.0

# Base image for the final runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files to restore dependencies
COPY ["VisionaryAnalytics.sln", "."]
COPY ["01_WebApi/01_WebApi.csproj", "01_WebApi/"]
COPY ["02_QueueConsumer/02_QueueConsumer.csproj", "02_QueueConsumer/"]
COPY ["03_Application/03_Application.csproj", "03_Application/"]
COPY ["04_Domain/04_Domain.csproj", "04_Domain/"]
COPY ["05_Infraestructure/05_Infraestructure.csproj", "05_Infraestructure/"]
COPY ["06_SharedKernel/06_SharedKernel.csproj", "06_SharedKernel/"]
COPY ["07_CrossCutting/07_CrossCutting.csproj", "07_CrossCutting/"]
COPY ["08_UnitTest/08_UnitTest.csproj", "08_UnitTest/"]
COPY ["09_IntegrationTest/09_IntegrationTest.csproj", "09_IntegrationTest/"]

# The .dcproj is a Visual Studio-specific file.
# We remove it from the .sln file inside the container before restoring
# so the dotnet CLI doesn't try (and fail) to process it.
RUN dotnet sln VisionaryAnalytics.sln remove docker-compose.dcproj

# Restore dependencies for the entire solution
RUN dotnet restore "VisionaryAnalytics.sln" 

# Copy the rest of the application's source code
COPY . .

# Build the project
WORKDIR "/src/01_WebApi"
RUN dotnet build "01_WebApi.csproj" -c $BUILD_CONFIGURATION --no-restore

# Publish the project
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "01_WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore --no-build

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# This service does not use FFMpeg, so remove the entire Assets folder to save space
RUN rm -rf Assets

ENTRYPOINT ["dotnet", "01_WebApi.dll"]