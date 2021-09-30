// --- EF Core Migrations --- (No 'Package Manager Console' in VS for Mac)

1. Install EF Tools: "dotnet tool install --global dotnet-ef"
2. Open Terminal at the MiniService.Data root directory
3. Create Migration:
    - Command: "dotnet ef migrations add {NEW_MIGRATION_NAME}"
      [Optional] '-p MiniService.Data' use it if run not from the MiniService.Data project root
      [Optional] '-c MiniServiceDbContext' if there is more than one context in the project
4. Generate SQL:
    - Command: "dotnet ef migrations script {CURRENT_MIGRATION_NAME} {NEW_MIGRATION_NAME} -p MiniService.Data -o
      ./Migrations/Scripts/{FILENAME}.sql"
      [Optional] -c MiniServiceDbContext if there is more than one context in the project