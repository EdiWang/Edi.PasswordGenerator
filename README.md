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
<PackageReference Include="Edi.PasswordGenerator" Version="2.0.0" />
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

Register `IPasswordGenerator` in DI container.

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

## 免责申明

此项目及其配套组件均为免费开源的产品，仅用于学习交流，并且不直接向中国提供服务，中国访客请于下载后立即删除。

任何中国境内的组织及个人不得使用此项目及其配套组件构建任何形式的面向中国境内访客的网站或服务。

不可用于任何违反中华人民共和国(含台湾省)或使用者所在地区法律法规的用途。

因为作者即本人仅完成代码的开发和开源活动(开源即任何人都可以下载使用)，从未参与访客的任何运营和盈利活动。

且不知晓访客后续将程序源代码用于何种用途，故访客使用过程中所带来的任何法律责任即由访客自己承担。

[《开源软件有漏洞，作者需要负责吗？是的！》](https://go.edi.wang/aka/os251)
