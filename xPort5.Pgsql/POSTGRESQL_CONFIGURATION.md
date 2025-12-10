# PostgreSQL Configuration Guide for xPort5

This guide explains how to configure xPort5 projects to use PostgreSQL instead of MS SQL Server.

## Prerequisites

1. PostgreSQL 12+ installed and running
2. Database schema migrated (see schema migration scripts)
3. Data migrated (see Python migration tool)

## NuGet Package Installation

### For xPort5.EF6 Project

Install the Npgsql provider for Entity Framework 6:

```powershell
Install-Package Npgsql.EntityFramework
```

Or via Package Manager Console:
```
Install-Package Npgsql.EntityFramework -Version 6.4.0
```

## Connection String Configuration

### MS SQL Server Connection String (Current)

```xml
<add name="xPort5Entities" 
     connectionString="metadata=res://*/xPort5Model.csdl|res://*/xPort5Model.ssdl|res://*/xPort5Model.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=192.168.12.144;initial catalog=xPort3_Newish;persist security info=True;user id=sa;password=nx-9602;MultipleActiveResultSets=True;App=EntityFramework&quot;"
     providerName="System.Data.EntityClient" />
```

### PostgreSQL Connection String

```xml
<add name="xPort5Entities" 
     connectionString="metadata=res://*/xPort5Model.csdl|res://*/xPort5Model.ssdl|res://*/xPort5Model.msl;provider=Npgsql;provider connection string=&quot;Host=localhost;Port=5432;Database=xport5;Username=postgres;Password=your_password;App=EntityFramework&quot;"
     providerName="System.Data.EntityClient" />
```

**Note**: For Database-First EF6 with PostgreSQL, you may need to regenerate the EDMX model from the PostgreSQL database. See "EDMX Regeneration" section below.

## App.config / Web.config Updates

### xPort5.EF6/App.config

Add Npgsql provider to `<entityFramework><providers>` section:

```xml
<entityFramework>
  <defaultConnectionFactory type="System.Data.Entity.Infrastructure.LocalDbConnectionFactory, EntityFramework">
    <parameters>
      <parameter value="mssqllocaldb" />
    </parameters>
  </defaultConnectionFactory>
  <providers>
    <provider invariantName="System.Data.SqlClient" type="System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer" />
    <provider invariantName="Npgsql" type="Npgsql.NpgsqlServices, EntityFramework6.Npgsql" />
  </providers>
</entityFramework>
```

### xPort5/Web.config

Same provider configuration as above, plus update connection strings.

### xPort5.Bot/Web.config

Same provider configuration as above, plus update connection strings.

## EDMX Regeneration (Database-First)

For Database-First EF6, the EDMX model files (`.csdl`, `.ssdl`, `.msl`) are provider-specific. To use PostgreSQL:

### Option 1: Regenerate from PostgreSQL Database

1. Open Visual Studio
2. Right-click on `xPort5Model.edmx` → "Update Model from Database..."
3. Select PostgreSQL connection
4. Update the model

### Option 2: Create Separate EDMX for PostgreSQL

1. Create a copy of `xPort5Model.edmx` → `xPort5Model_PostgreSQL.edmx`
2. Update from PostgreSQL database
3. Use conditional compilation or configuration to select the appropriate model

### Option 3: Use Code-First Approach (Future)

Migrate from Database-First to Code-First, which is provider-agnostic.

## xPort5.Common Configuration

Update `xPort5.Common/Config.cs` to handle PostgreSQL connection strings:

```csharp
public static string ConnectionString
{
    get
    {
        var connStr = WebConfigurationManager.ConnectionStrings["SysDb"].ConnectionString;
        // PostgreSQL connection strings use different format
        // Format: Host=host;Port=port;Database=db;Username=user;Password=pass
        return connStr;
    }
}
```

## Testing

1. Update connection strings to point to PostgreSQL
2. Run application
3. Test CRUD operations
4. Verify data integrity
5. Test views and complex queries

## Switching Between Databases

To switch between MS SQL Server and PostgreSQL:

1. Update connection strings in `Web.config` / `App.config`
2. Ensure appropriate provider is configured
3. Restart application

## Troubleshooting

### Error: "The provider did not return a ProviderManifestToken string"

- Ensure Npgsql provider is installed
- Check that provider is registered in `<providers>` section
- Verify connection string uses correct provider name (`Npgsql`)

### Error: "Unable to determine the provider name"

- Check that `providerName="System.Data.EntityClient"` is set
- Verify metadata paths in connection string are correct

### Error: "Schema specified is not valid"

- EDMX model may need regeneration from PostgreSQL database
- Check that `.csdl`, `.ssdl`, `.msl` files match PostgreSQL schema

## References

- [Npgsql Entity Framework 6 Documentation](https://www.npgsql.org/ef6/)
- [Entity Framework 6 Provider Model](https://docs.microsoft.com/en-us/ef/ef6/fundamentals/providers/)

