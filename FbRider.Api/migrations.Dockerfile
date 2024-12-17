# Use the SDK image to perform the migration
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy only the necessary project files (not the entire source code)
COPY ["FbRider.Api/FbRider.Api.csproj", "FbRider.Api/"]
RUN dotnet restore "./FbRider.Api/FbRider.Api.csproj"

# Install Entity Framework Core CLI tools
RUN dotnet tool install --global dotnet-ef

# Ensure the .NET tools are in the PATH
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy the wait-for-it.sh script into the container
COPY wait-for-it.sh /wait-for-it.sh
RUN chmod +x /wait-for-it.sh

# Copy the rest of the application code
COPY . .

# Set the working directory
WORKDIR "/src/FbRider.Api"

# Apply the migrations
ENTRYPOINT ["dotnet", "ef", "database", "update", "--no-build"]
