# Use the official .NET runtime image for Linux
FROM mcr.microsoft.com/dotnet/runtime:6.0 

# Set the working directory inside the container
#WORKDIR /app

# Copy the published output of the Console App
COPY bin/Release/net6.0/publish .

# Specify the entry point for the container
ENTRYPOINT ["dotnet", "Olympus_QUIZ.dll"]