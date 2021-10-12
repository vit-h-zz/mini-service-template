# EF Core Migrations via command line and dotnet tool
No 'Package Manager Console' in VS for Mac

### Instruction:
1. Install EF Tools: "dotnet tool install --global dotnet-ef" (update: "dotnet tool update --global dotnet-ef")

2. Open Terminal at the MiniService.Data root directory

3. Create Migration: "dotnet ef migrations add {NEW_MIGRATION_NAME}"
    * [Optional] '-p MiniService.Data' use it if run not from the MiniService.Data project root
    * [Optional] '-c AppContext' if there is more than one context in the project
        
4. Execute migrations: "dotnet ef database update"
    * [Optional] '-p MiniService.Data'
    * [Optional] '-c AppContext'
        
5. Generate SQL: "dotnet ef migrations script {CURRENT_MIGRATION_NAME or 0 if new DB} {NEW_MIGRATION_NAME} -o ./Migrations/Scripts/{FILENAME}.sql"
    * [Optional] -c AppContext
    * Example: "dotnet ef migrations script 0 Init -o ./Migrations/Scripts/Init.sql"

### More commands:
- dotnet ef database drop
- dotnet ef migrations list
- dotnet ef migrations remove

### More info:
- https://www.entityframeworktutorial.net/efcore/cli-commands-for-ef-core-migration.aspx
