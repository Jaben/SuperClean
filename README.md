# SuperClean
Command line tool that recursively cleans `bin` and `obj` directories of build output (`*.dll`, `*.pdb`, `*.exe`, `*.cache`).

[![Build](https://github.com/Jaben/SuperClean/actions/workflows/build.yml/badge.svg)](https://github.com/Jaben/SuperClean/actions/workflows/build.yml) [![NuGet](https://img.shields.io/nuget/v/SuperClean.svg)](https://www.nuget.org/packages/SuperClean/)

## Usage

Run `SuperClean.exe` (or `superclean`) from the root of a solution — it finds all nested `bin` and `obj` directories and deletes the build output inside them, skipping `.git`, `.vs`, `node_modules`, `packages`, and similar directories.

## Install

As a dotnet global tool:

```
dotnet tool install -g SuperClean
superclean
```

Or download the standalone `SuperClean.exe` from [Releases](https://github.com/Jaben/SuperClean/releases) — no .NET runtime required.

## Build

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

```
dotnet build src/SuperClean.csproj -c Release
```

Publish a self-contained single-file exe (trimmed and compressed, ~10 MB):

```
dotnet publish src/SuperClean.csproj -c Release -r win-x64
```
