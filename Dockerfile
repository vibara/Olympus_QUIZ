# Use the official .NET runtime image for Linux
FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS base

# Set the working directory inside the container
WORKDIR /app

# Copy the published output of the Console App
COPY bin/Release/net6.0/ .

# Specify the entry point for the container
ENTRYPOINT ["dotnet", "olympus_quiz.dll"]