# <img src="./img/icon.png" style="width: 30px" /> KutCode.AutoMapper.Extensions

.NET library that allows you:    
- Configure Mappings in the type definition
- Use inheritance of interfaces for "default" mappings, without complex rules

## 📜 Install

`KutCode.AutoMapper.Extensions` is designed with `net7.0`, `net8.0` and higher.

Install `KutCode.AutoMapper.Extensions` using NuGet Package Manager:

```powershell
Install-Package KutCode.AutoMapper.Extensions
```

Or via the .NET CLI:

```shell
dotnet add package KutCode.AutoMapper.Extensions
```

All versions can be found [here](https://www.nuget.org/packages/KutCode.AutoMapper.Extensions/).


## 🚀 Quick Start

Lets declare two types:
```csharp
public class SomeEntity
{
    public string Value { get;set; }
}

public class SomeDto : IMapWith<SomeEntity> // <-- just inherit it
{
    public string Value { get;set; }
}
```
Use DI to configure AutoMapper:
```csharp
global using AutoMapper;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
// just call this to scan All assemblies
builder.Services.AddAllMappings();
// or select assemblies manually
builder.Services.AddMappings(typeof(Program).Assembly, typeof(Domain).Assembly);
```
So, that's all, now you can map with AutoMapper's `IMapper` as usual:
```csharp
SomeDto dto = mapper.Map<SomeEntity>(entity);
```
Notice, that:  
Interface `IMapWith<T>` creates `.ReverseMap()`, whereas, you can also use:  
- `IMapFrom<T>` create default map from `T` to implementing class 
- `IMapTo<T>` create default map from implementing class to `T`

⚠️ Of course, that's not all. You can also override map methods.




