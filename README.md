# Features list
1. ASP.NET 5
2. Packageable .NET template
3. Clean architecture*
4. WebApi with OpenApi and Swagger UI
5. RabbitMq via MassTransit lib
6. Grpc setup
7. Logging with Serilog
8. EF core code first
9. EF on SaveChanges interceptor
10. MediatR (for layers separation and atomic logic split)
11. Cashing with Redis via IDistributedCache
12. Automapping with Mapster (fastest and comfortable mapper in .net)
13. Fluent Validation as MediatR pipeline step
14. Open Telemetry, Tracing with Jaeger
15. Exceptions handling with Fluent Results - wraps results and errors, so only unhandled exceptions means a problem
16. HealthChecks for Readiness and Liveness probes
17. Tests with WebAssembly, xUnit, FluentAssertions, Moq and AutoBogus with providing dependencies as test params
18. Packagable base libs with microservice settings (to update all microservices by pkg update)
19. Packagable OpenApi.json communication contracts as NuGet (for easy microservices connect)
20. Packagable RabbitMq communication contracts as NuGet (for easy microservices connect)
21. Packagable Grpc proto files as NuGet (for easy microservices connect)
22. Request context passing to subrequests
23. Multi-tenancy support
24. Template dummy logic with examples for all layers
25. Docker file and ignore file
26. git ignore
27. NuGet config
28. ReadMe for microservice
29. ReadMe for template usage 
30. Editor config
31. Local NuGet feed for effective development

TODO features:
1. Metrics with Prometeus
2. Authentication and Authorization
3. Upgrade cashing using CacheTower library
4. Add open telemetry traceId and spanId to logs for corellation
5. Add logs viewer to docker compose
6. Enable logs output to the test run output
7. Static analyzer
8. Automatic NuGet template packaging and publish on release
9. Monitoring, definition of alerts for Devops, action item for support
10. GitHub CI/CD workflows for microservices
11. Dev data seeding

# How to use Template
There are two options to install the template: from local files and via nuget package.

## Local-Files Install (a)
1. Clone repo
2. Get shell at repo root directory
3. Run: `dotnet new --install ./Template-Service`

## Nuget-Based Install (b)
1. Download the nuget.exe and save for example at: `C:\W\nuget.exe`
2. Pack the template to NuGet package by the instructions provided in the .nuspec file
`C:\W\nuget.exe pack .\Template-Service\Template-Service.Template.CSharp.nuspec -OutputDirectory C:\Temp -NoDefaultExcludes`
3. Install the template from the NuGet package to the dotnet to check it works
`dotnet new --install C:\Temp\MiniService.Template.CSharp.VH.0.0.1.nupkg`
4. See "Using Template"

## Uninstall
`dotnet new --uninstall MiniService.Template.CSharp.VH`

or when installed from repo (no need to reinstall when changed files)

`dotnet new --uninstall C:\W\Service-Template\Template-Service`

# Using Template
1. Clone the repo where you want the template
2. Ex Command: `dotnet new mst.vh -o ../My-Service --basename MyService`
3. Options:
- Output (required): `-o ./path/to/repo`
- Base Name (required): `--basename {ServiceName}`
- gRPC: `--add-grpc {true/false}` [default: true]


# Helpful Links:
- [How to create a template in .NET for a C# application and what NuGet is for](https://itnext.io/how-to-create-a-template-in-net-for-a-c-application-and-what-nuget-is-for-e5d4fc03c487)
