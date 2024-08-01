# <img src="./img/icon.png" style="width: 30px" /> KutCode.AutoMapper.Extensions

.NET library that allows you:    
- Configure Mappings in the type definition
- Use inheritance of interfaces for "default" mappings, without complex rules

## 📜 Installation

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
### Basic example
Let's declare two types:
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
SomeDto dto = mapper.Map<SomeDto>(entity);
```
----
### `IMapFrom<T>` and `IMapTo<T>`
⚠️ Whereas, you can also use those interfaces, which just calls `CreateMap()` if not overrided:
- `IMapFrom<T>` create map from `T` to implementing class
- `IMapTo<T>` create map from implementing class to `T`

----
### Override default mapping
If you just inherite interface `IMapWith<T>` - that will created `.ReverseMap()` for two types.  
So, you can override default mapping, and set your own behaviour:
```csharp
public class SomeDto : IMapWith<SomeEntity>
{
    public string Value { get;set; }
    // override Map method
    public void Map(MapProfileDecorator<SomeEntity> decorator)
    {
        decorator.Profile.CreateMap<DataDto, DataEntity>()
            .ForMember(m => m.Value, opt 
                => opt.MapFrom(f => "SomeOverride")
            );
    }
}
```
----
### Use multiple interfaces
```csharp
public class SomeDto : IMapWith<SomeEntity>,
    IMapTo<AnotherOne>, IMapFrom<AndAnotherOne>
{
    public string Value { get;set; }
    
    // also overrides available for all intrfaces
}
```
----

## ✨ Conclusion

- Use `IMapWith<T>` for reverse mapping
- Use `IMapFrom<T>` to map from `T` to an implementing type
- Use `IMapTo<T>` to map from an implementing type to `T`

All of these interfaces allow you to override the `Map(MapProfileDecorator<T>)` method to customize the mapping.


## ☕ Contribution

If you wanna to buy me a coffee, send any tokens in TON network:  
💎 `noncommunicado.ton`  
💎 `UQD0zFgp0p-eFnbL4cPA6DYqoeWzGbCA81KuU6BKwdFmf8jv`
