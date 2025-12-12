@echo off
dotnet build
docfx metadata
docfx build
echo Documentation updated!