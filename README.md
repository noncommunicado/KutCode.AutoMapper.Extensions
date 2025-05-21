# <img src="./img/icon.png" style="width: 40px" /> KutCode.AutoMapper.Extensions

.NET library that allows you to:    
✅ Configure Mappings in the type declaration  
✅ Use inheritance of interfaces for "default" mappings, without complex rules  
✅ Create custom mapping with `Profile` **in type declaration**!  

## 📖 Table of Contents
- [Installation](#-installation)
- [Quick Start](#-quick-start)
  - [Basic Example](#basic-example)
  - [IMapFrom&lt;T&gt; and IMapTo&lt;T&gt;](#imapfromt-and-imaptot)
  - [Override Default Mapping](#override-default-mapping)
  - [Use Multiple Interfaces](#use-multiple-interfaces)
- [Dependency Injection](#-dependency-injection)
  - [Register All Mappings](#register-all-mappings)
  - [Register Specific Assemblies](#register-specific-assemblies)
  - [Custom Configuration](#custom-configuration)
- [Conclusion](#-conclusion)
- [Contribution](#-contribution)

## 📜 Installation

`KutCode.AutoMapper.Extensions` is designed for `net7.0`, `net8.0`, `net9.0` and higher.

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
⚠️ Whereas, you can also use those interfaces, which just calls `CreateMap()`:
- `IMapFrom<T>` create map from `T` to implementing class
- `IMapTo<T>` create map from implementing class to `T`

----
### Override default mapping
So, you can override default mapping, just inherit interface `IHaveMap`.  
And set your own behaviour right in type definition:  
```csharp
public class SomeDto : IHaveMap // ✅ Inherit interface 
{
    public string Value { get;set; }

    // ✅ Implement Map method
    public static void Map(Profile profile)
    {
        profile.CreateMap<DataDto, DataEntity>()
            .ForMember(m => m.Value, opt 
                => opt.MapFrom(f => "SomeOverride")
            );
        // any other mappings...
    }
}
```
----
### Use multiple interfaces
```csharp
public class SomeDto : 
    IHaveMap,
    IMapWith<SomeEntity>,
    IMapTo<AnotherOne>,
    IMapFrom<AndAnotherOne>
{
    public string Value { get;set; }
    
    public static void Map(Profile profile)
    {
        // profile.CreateMap...
    }
}
```
----

## 💉 Dependency Injection

The library provides several extension methods for registering AutoMapper profiles in your application's dependency injection container.

### Register All Mappings

To scan and register all mappings from all loaded assemblies:

```csharp
// Register all mappings from all loaded assemblies
builder.Services.AddAllMappings();

// With custom configuration
builder.Services.AddAllMappings(cfg => {
    // Custom configurations here
    cfg.AllowNullDestinationValues = true;
});

// Including default AutoMapper profiles
builder.Services.AddAllMappings(catchDefaultProfiles: true);
```

### Register Specific Assemblies

To scan only specific assemblies for mappings:

```csharp
// Register mappings from specific assemblies
builder.Services.AddMappings(
    typeof(Program).Assembly, 
    typeof(Domain).Assembly
);

// With custom configuration
builder.Services.AddMappings(
    cfg => {
        cfg.CreateMap<CustomSource, CustomDestination>();
    },
    catchDefaultProfiles: true,
    typeof(Program).Assembly
);
```

### Custom Configuration

For explicit AutoMapper configuration:

```csharp
// Register AutoMapper with explicit configuration
builder.Services.AddMappings(cfg => {
    cfg.CreateMap<Source, Destination>()
        .ForMember(dest => dest.Property, opt => opt.MapFrom(src => src.OtherProperty));
});
```

## ✨ Conclusion

- Use `IMapWith<T>` for reverse mapping
- Use `IMapFrom<T>` to map from `T` to an implementing type
- Use `IMapTo<T>` to map from an implementing type to `T`
- Use `IHaveMap` to customize mapping

## ☕ Contribution

If you wanna to buy me a coffee, send any tokens in TON network:  
💎 `noncommunicado.ton`  
💎 `UQD0zFgp0p-eFnbL4cPA6DYqoeWzGbCA81KuU6BKwdFmf8jv`
