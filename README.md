# Life.NET
[![Travis](https://img.shields.io/travis/joncloud/life-net.svg)](https://travis-ci.org/joncloud/life-net/)
[![NuGet](https://img.shields.io/nuget/v/Life.svg)](https://www.nuget.org/packages/Life/)

## Description
Life.NET provides component status evaluation to integrate with monitoring services.

## Licensing
Released under the MIT License.  See the [LICENSE][] file for further details.

[license]: LICENSE.md

## Installation
In the Package Manager Console execute

```powershell
Install-Package Life
```

Or update `*.csproj` to include a dependency on

```xml
<ItemGroup>
  <PackageReference Include="Life" Version="0.1.0-*" />
</ItemGroup>
```

## Usage
Sample authorization for details:
```csharp
class Startup {
  public void ConfigureServices(IServiceCollection services) => 
    services.AddLife(options => options.AuthorizeDetails = ctx => ctx.User.IsInRole("Admin"));
}
```

Sample integration with Sqlite Database:
```csharp
class Startup {
  // ...
  
  public void ConfigureServices(IServiceCollection services) => 
    services.AddLife(options => options.AddDbConnectionEvaluator("SqliteDatabase", () => new SqliteConnection(), "Data Source=:memory:;"));
    
  public void Configure(IApplicationBuilder app, IHostingEnvironment env) =>
    app.UseLife().UseMvc();
}
```

Sample output for `/life` results in
```json
{
  "sqliteDatabase": "Up"
}
```

For additional usage see [Tests][].

[Tests]: tests/Life.Tests
