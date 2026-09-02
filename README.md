# Edi.PasswordGenerator

[![.NET Build and Pack](https://github.com/EdiWang/Edi.PasswordGenerator/actions/workflows/dotnet.yml/badge.svg)](https://github.com/EdiWang/Edi.PasswordGenerator/actions/workflows/dotnet.yml)

Generate Chromium-style passwords with cryptographically secure randomness.

## Install from NuGet

```powershell
dotnet add package Edi.PasswordGenerator
```

```powershell
NuGet\Install-Package Edi.PasswordGenerator
```

```xml
<PackageReference Include="Edi.PasswordGenerator" Version="3.0.0" />
```

## Usage

### .NET

```csharp
var generator = new PasswordGenerator();

// Uses the Chromium-style defaults: 15 characters, letters and numbers,
// with ambiguous characters excluded.
var password = generator.GeneratePassword();

// Customize the character classes and length.
var customPassword = generator.GeneratePassword(new ChromiumPasswordOptions
{
    Length = 24,
    UseUppercase = true,
    UseLowercase = true,
    UseNumbers = true,
    UseSymbols = true,
    ExcludeAmbiguous = true
});
```

`DefaultPasswordGenerator` remains available as a compatibility name for
`PasswordGenerator`. The pre-3.0.0 algorithm is available explicitly through
`LegacyPasswordGenerator` and `PasswordRule`.

### ASP.NET Core

Register `IPasswordGenerator` in the DI container.

```csharp
services.AddTransient<IPasswordGenerator, PasswordGenerator>();
```

```csharp
[HttpGet("password/generate")]
[ProducesResponseType(StatusCodes.Status200OK)]
public IActionResult GeneratePassword([FromServices] IPasswordGenerator passwordGenerator)
{
    var password = passwordGenerator.GeneratePassword();
    return Ok(new
    {
        ServerTimeUtc = DateTime.UtcNow,
        Password = password
    });
}
```
