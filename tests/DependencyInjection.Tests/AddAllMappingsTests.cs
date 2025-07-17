namespace DependencyInjection.Tests;

[TestFixture]
public class AddAllMappingsTests
{
    // Sample DTO and model to test mappings
    public class TestModel
    {
        public string Value { get; set; } = string.Empty;
    }

    public class TestDto : IMapWith<TestModel>
    {
        public string Value { get; set; } = string.Empty;
    }
    
    // Custom profile for testing
    private class TestMappingProfile : Profile
    {
        public TestMappingProfile()
        {
            CreateMap<TestModel, TestDto>().ReverseMap();
        }
    }

    [Test]
    public void AddAllMappings_RegistersMappings_Success()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act - Use the extension method
        services.AddAllMappings();
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetService<IMapper>();

        // Assert
        mapper.Should().NotBeNull();
        var model = new TestModel { Value = "Test" };
        var dto = mapper!.Map<TestDto>(model);
        
        dto.Should().NotBeNull();
        dto.Value.Should().Be("Test");
    }

    [Test]
    public void AddAllMappings_WithConfig_AppliesConfiguration_Success()
    {
        // Arrange
        var services = new ServiceCollection();
        var configApplied = false;

        // Act - Use the extension method with config
        services.AddAllMappings((p, cfg) => 
        {
            configApplied = true;
        });
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetService<IMapper>();

        // Assert
        mapper.Should().NotBeNull();
        configApplied.Should().BeTrue();
    }

    [Test]
    public void AddAllMappings_WithCatchDefaultProfiles_Success()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act - Use the extension method with catchDefaultProfiles flag
        services.AddAllMappings(catchDefaultProfiles: true);
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetService<IMapper>();

        // Assert
        mapper.Should().NotBeNull();
    }
} 