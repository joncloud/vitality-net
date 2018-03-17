# Vitality.NET
[![Travis](https://img.shields.io/travis/joncloud/vitality-net.svg)](https://travis-ci.org/joncloud/vitality-net/)
[![NuGet](https://img.shields.io/nuget/v/Vitality.svg)](https://www.nuget.org/packages/Vitality/)

<img src="https://raw.githubusercontent.com/joncloud/vitality-net/master/nuget.png" alt="vitality.net" />

## Description
Vitality.NET provides component status evaluation to integrate with monitoring services.

## Licensing
Released under the MIT License.  See the [LICENSE][] file for further details.

[license]: LICENSE.md

## Installation
In the Package Manager Console execute

```powershell
Install-Package Vitality
```

Or update `*.csproj` to include a dependency on

```xml
<ItemGroup>
  <PackageReference Include="Vitality" Version="0.1.0-*" />
</ItemGroup>
```

## Usage
Sample authorization for details:
```csharp
class Startup {
  public void ConfigureServices(IServiceCollection services) => 
    services.AddVitality(options => options.AuthorizeDetails = ctx => ctx.User.IsInRole("Admin"));
}
```

Sample integration with Sqlite Database:
```csharp
class Startup {
  // ...
  
  public void ConfigureServices(IServiceCollection services) => 
    services.AddVitality(options => options.AddDbConnectionEvaluator("SqliteDatabase", () => new SqliteConnection(), "Data Source=:memory:;"));
    
  public void Configure(IApplicationBuilder app, IHostingEnvironment env) =>
    app.UseVitality().UseMvc();
}
```

Sample output for `/vitality` results in
```json
{
  "sqliteDatabase": "Up"
}
```

Test results can be cached by using the appropriate overload:
```csharp
services.AddVitality(options => options.AddDbConnectionEvaluator("SqliteDatabase", () => new SqliteConnection(), "Data Source=:memory:;"), TimeSpan.FromMinutes(5));
```

Test an Entity Framework DbContext (include Vitality.EntityFrameworkCore):
```csharp
// Simply make sure the database connection is correct.
services.AddVitality(options => options.AddDbContextEvaluator<ApplicationDbContext>("ApplicationDbContext"));

// Check something on the context for verification.
services.AddVitality(options => options.AddDbContextEvaluator<ApplicationDbContext>("ApplicationDbContext", ctx => ctx.CriticalTableMustHaveRows.AnyAsync()));
```

For additional usage see [Tests][].

[Tests]: tests/Vitality.Tests
