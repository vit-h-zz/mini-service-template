# Local Template Setup
There are two options to install the template: from local files and via nuget package.

## Local-Files Install (a)
1. Clone repo
2. Get shell at repo root directory
3. Run: `dotnet new --install ./Mini-Service`

## Nuget-Based Install (b)
1. Download the nuget.exe and save for example at: `C:\W\nuget.exe`
2. Pack the template to NuGet package by the instructions provided in the .nuspec file
`C:\W\nuget.exe pack C:\W\Service-Template\Mini-Service\MiniService.Template.CSharp.nuspec -OutputDirectory C:\Temp -NoDefaultExcludes`
3. Install the template from the NuGet package to the dotnet to check it works
`dotnet new --install C:\Temp\MiniService.Template.CSharp.0.0.1.nupkg`
4. See "Using Template"

## Uninstall
dotnet new --uninstall MiniService.Template.CSharp
or when installed from repo (no need to reinstall when changed files)
dotnet new --uninstall C:\W\Service-Template\Mini-Service

# Using Template
1. Clone the repo where you want the template
2. Ex Command: `dotnet new mst -o ./Repos/CoinLedger/My-Service --basename MyService`
3. Options:
- Output (required): `-o ./path/to/repo`
- Base Name (required): `--basename {ServiceName}`
- gRPC: `--add-grpc {true/false}` [default: true]


# Helpful Links:
- [How to create a template in .NET for a C# application and what NuGet is for](https://itnext.io/how-to-create-a-template-in-net-for-a-c-application-and-what-nuget-is-for-e5d4fc03c487)
