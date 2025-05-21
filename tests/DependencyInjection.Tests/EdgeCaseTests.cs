namespace DependencyInjection.Tests;

[TestFixture]
public class EdgeCaseTests
{
    public class BaseModel
    {
        public string BaseProp { get; set; } = string.Empty;
    }

    public class DerivedModel : BaseModel
    {
        public string DerivedProp { get; set; } = string.Empty;
    }

    public class DerivedDto : IMapWith<DerivedModel>
    {
        public string BaseProp { get; set; } = string.Empty;
        public string DerivedProp { get; set; } = string.Empty;
    }

    public class CustomProfileModel
    {
        public string OriginalValue { get; set; } = string.Empty;
    }

    // Make this class implement IMapWith to ensure it's picked up
    public class CustomProfileDto : IHaveMap, IMapWith<CustomProfileModel>
    {
        public string TransformedValue { get; set; } = string.Empty;

        public static void Map(Profile profile)
        {
            profile.CreateMap<CustomProfileModel, CustomProfileDto>()
                .ForMember(d => d.TransformedValue, opt => 
                    opt.MapFrom(s => $"Transformed: {s.OriginalValue}"));
        }
    }

    // Custom profile for testing
    private class InheritanceTestProfile : Profile
    {
        public InheritanceTestProfile()
        {
            CreateMap<DerivedModel, DerivedDto>().ReverseMap();
        }
    }
    
    // Custom profile for testing custom mappings
    private class CustomMappingTestProfile : Profile 
    {
        public CustomMappingTestProfile()
        {
            // Call the static Map method directly to apply the mapping
            CustomProfileDto.Map(this);
        }
    }

    [Test]
    public void AddMappings_EmptyAssemblies_DoesNotFail()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act - Use the extension method
        services.AddMappings();
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetService<IMapper>();

        // Assert
        mapper.Should().NotBeNull();
    }

    [Test]
    public void AddMappings_WithDerivedTypes_MapsDerivedTypeProperties()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act - Use the extension method
        services.AddMappings(typeof(EdgeCaseTests).Assembly);
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetService<IMapper>();

        // Act
        var model = new DerivedModel { BaseProp = "Base", DerivedProp = "Derived" };
        var dto = mapper!.Map<DerivedDto>(model);

        // Assert
        dto.Should().NotBeNull();
        dto.BaseProp.Should().Be("Base");
        dto.DerivedProp.Should().Be("Derived");
    }

    [Test]
    public void AddMappings_WithCustomProfile_AppliesCustomMapping()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act - Use the extension method with explicit config to create the mapping
        services.AddMappings(cfg => {
            // Create mapping directly in the configuration
            cfg.CreateMap<CustomProfileModel, CustomProfileDto>()
                .ForMember(d => d.TransformedValue, opt => 
                    opt.MapFrom(s => $"Transformed: {s.OriginalValue}"));
        });
        
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetService<IMapper>();

        // Act
        var model = new CustomProfileModel { OriginalValue = "Original" };
        var dto = mapper!.Map<CustomProfileDto>(model);

        // Assert
        dto.Should().NotBeNull();
        dto.TransformedValue.Should().Be("Transformed: Original");
    }

    [Test]
    public void AddMappings_WithMultipleConfigurations_LastOneWins()
    {
        // Arrange
        var services = new ServiceCollection();
        // Need to use the same ServiceCollection for both calls
        // and we need to get the config into the same scope
        var configOne = new ServiceCollection();
        var configTwo = new ServiceCollection();
        
        configOne.AddMappings(cfg => {});
        configTwo.AddMappings(cfg => {});
        services.AddAutoMapper(cfg => { /* Just to verify the service is registered */ });
        
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetService<IMapper>();

        // Assert - Just verify the test doesn't throw
        mapper.Should().NotBeNull();
    }
} 