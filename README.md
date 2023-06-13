# Edi.PasswordGenerator

[![.NET Build and Pack](https://github.com/EdiWang/Edi.PasswordGenerator/actions/workflows/dotnet.yml/badge.svg)](https://github.com/EdiWang/Edi.PasswordGenerator/actions/workflows/dotnet.yml)

Generate secure password, but I am not sure, so use it on your own risk.

## Install from NuGet

```powershell
dotnet add package Edi.PasswordGenerator
```

```powershell
NuGet\Install-Package Edi.PasswordGenerator
```

```xml
<PackageReference Include="Edi.PasswordGenerator" Version="1.0.0" />
```

## Usage

### .NET

```csharp
var gen = new DefaultPasswordGenerator();

// Using classic ASP.NET MVC membership method
var p1 = gen.GeneratePassword(new(10, 3));
// example: WSI:R=6s(C

// Quickly get a password
var p2 = gen.GeneratePassword();
// example: ou45V8La%X

```

### ASP.NET Core

```csharp
services.AddTransient<IPasswordGenerator, DefaultPasswordGenerator>();
```

```csharp
[HttpGet("password/generate")]
[ProducesResponseType(StatusCodes.Status200OK)]
public IActionResult GeneratePassword([FromServices] IPasswordGenerator passwordGenerator)
{
    var password = passwordGenerator.GeneratePassword(new(10, 3));
    return Ok(new
    {
        ServerTimeUtc = DateTime.UtcNow,
        Password = password
    });
}
```

