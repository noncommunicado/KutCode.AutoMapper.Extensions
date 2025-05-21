namespace DependencyInjection.Tests;

[TestFixture]
public class AddMappingsTests
{
    public class SourceModel
    {
        public string Value { get; set; } = string.Empty;
    }

    public class DestinationDto : IMapWith<SourceModel>
    {
        public string Value { get; set; } = string.Empty;
    }

    public class CustomModel
    {
        public string SourceProperty { get; set; } = string.Empty;
    }

    public class CustomDto
    {
        public string DestProperty { get; set; } = string.Empty;
    }

    [Test]
    public void AddMappings_WithAssemblies_RegistersMappings_Success()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act - Use the extension method
        services.AddMappings(typeof(AddMappingsTests).Assembly);
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetService<IMapper>();

        // Assert
        mapper.Should().NotBeNull();
        var model = new SourceModel { Value = "Test" };
        var dto = mapper!.Map<DestinationDto>(model);
        
        dto.Should().NotBeNull();
        dto.Value.Should().Be("Test");
    }

    [Test]
    public void AddMappings_WithConfigAction_AppliesConfiguration_Success()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act - Use the extension method
        services.AddMappings(cfg =>
        {
            cfg.CreateMap<CustomModel, CustomDto>()
                .ForMember(dest => dest.DestProperty, 
                    opt => opt.MapFrom(src => src.SourceProperty));
        });
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetService<IMapper>();

        // Assert
        mapper.Should().NotBeNull();
        var model = new CustomModel { SourceProperty = "Custom" };
        var dto = mapper!.Map<CustomDto>(model);
        
        dto.Should().NotBeNull();
        dto.DestProperty.Should().Be("Custom");
    }

    [Test]
    public void AddMappings_WithConfigAndAssemblies_AppliesAll_Success()
    {
        // Arrange
        var services = new ServiceCollection();
        var configApplied = false;

        // Act - Use the extension method with config and assemblies
        services.AddMappings(
            cfg => {
                configApplied = true;
                cfg.CreateMap<CustomModel, CustomDto>()
                    .ForMember(dest => dest.DestProperty, 
                        opt => opt.MapFrom(src => src.SourceProperty));
            },
            catchDefaultProfiles: true,
            typeof(AddMappingsTests).Assembly);
        
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetService<IMapper>();

        // Assert
        mapper.Should().NotBeNull();
        configApplied.Should().BeTrue();
        
        // Test explicit mapping
        var customModel = new CustomModel { SourceProperty = "Custom" };
        var customDto = mapper!.Map<CustomDto>(customModel);
        customDto.Should().NotBeNull();
        customDto.DestProperty.Should().Be("Custom");
        
        // Test interface-based mapping
        var sourceModel = new SourceModel { Value = "Source" };
        var destDto = mapper.Map<DestinationDto>(sourceModel);
        destDto.Should().NotBeNull();
        destDto.Value.Should().Be("Source");
    }
} 