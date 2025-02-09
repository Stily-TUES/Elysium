@echo off
echo Setting up Elysium...

dotnet --version >nul 2>&1
IF %ERRORLEVEL% NEQ 0 (
    echo .NET SDK is not installed. Please install it from https://dotnet.microsoft.com/en-us/download
    exit /b
)

cd Editor
echo Restoring dependencies...
dotnet restore

echo Building project...
dotnet build --configuration Release

echo Starting Editor...
cd bin\Release\net8.0-windows
start Editor.exe

echo Setup complete!