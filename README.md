# Vitality.NET
[![Travis](https://img.shields.io/travis/joncloud/vitality-net.svg)](https://travis-ci.org/joncloud/vitality-net/)
[![NuGet](https://img.shields.io/nuget/v/Vitality.svg)](https://www.nuget.org/packages/Vitality/)

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

For additional usage see [Tests][].

[Tests]: tests/Vitality.Tests
