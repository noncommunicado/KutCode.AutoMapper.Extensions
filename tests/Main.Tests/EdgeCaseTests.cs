namespace Main.Tests;

[TestFixture]
public class EdgeCaseTests
{
    [Test]
    public void AssemblyMappingProfile_WithEmptyTypeArray_DoesNotThrow()
    {
        // Arrange & Act
        Action act = () => new AssemblyMappingProfile(Array.Empty<Type>());
        
        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void AssemblyMappingProfile_WithNullTypeArray_DoesNotThrow()
    {
        // Arrange & Act
        Action act = () => new AssemblyMappingProfile((Type[])null!);
        
        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void AssemblyMappingProfile_WithInterfaceType_IgnoresInterface()
    {
        // Arrange & Act
        var profile = new AssemblyMappingProfile(typeof(ITestInterface));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        // Assert - Should not throw, interface should be ignored
        mapper.Should().NotBeNull();
    }

    [Test]
    public void AssemblyMappingProfile_WithAbstractClass_IgnoresAbstractClass()
    {
        // Arrange & Act
        var profile = new AssemblyMappingProfile(typeof(AbstractTestClass));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        // Assert - Should not throw, abstract class should be ignored
        mapper.Should().NotBeNull();
    }

    [Test]
    public void Mapping_WithMissingParameterlessConstructor_NotThrowsException()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(NoParameterlessCtorDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var source = new NoParameterlessCtorSource { Value = "test" };
        
        // Act & Assert
        var result = mapper.Map<NoParameterlessCtorDto>(source);
        result.Value.Should().Be("test");
    }

    [Test]
    public void Mapping_WithGenericTypeDefinition_HandledCorrectly()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(GenericDto<>));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        // Assert - Should not throw during configuration
        mapper.Should().NotBeNull();
    }

    [Test]
    public void HaveMap_WithInvalidMapMethod_IgnoresMapping()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(InvalidMapMethodDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        // Assert - Should not throw, invalid map method should be ignored
        mapper.Should().NotBeNull();
    }

    [Test]
    public void Mapping_WithSelfReference_HandledGracefully()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(SelfReferenceDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var source = new SelfReferenceEntity { Name = "Self", Self = null };
        source.Self = source; // Create self-reference
        
        // Act & Assert
        Action act = () => mapper.Map<SelfReferenceDto>(source);
        act.Should().NotThrow();
    }

    [Test]
    public void Mapping_WithMultipleInterfaceImplementations_AllMappingsCreated()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(MultiInterfaceDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var sourceA = new MultiSourceA { Id = 1, Name = "A" };
        var sourceB = new MultiSourceB { Id = 2, Description = "B" };
        
        // Act
        var resultA = mapper.Map<MultiInterfaceDto>(sourceA);
        var resultB = mapper.Map<MultiInterfaceDto>(sourceB);
        
        // Assert
        resultA.Should().NotBeNull();
        resultA.Id.Should().Be(1);
        
        resultB.Should().NotBeNull();
        resultB.Id.Should().Be(2);
    }

    [Test]
    public void Mapping_WithNestedGenericTypes_Success()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(NestedGenericDto<string, int>));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var source = new NestedGenericSource<string, int>
        {
            Data = new Dictionary<string, List<int>>
            {
                { "key1", new List<int> { 1, 2, 3 } },
                { "key2", new List<int> { 4, 5, 6 } }
            }
        };
        
        // Act
        var result = mapper.Map<NestedGenericDto<string, int>>(source);
        
        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.Data["key1"].Should().HaveCount(3);
    }

    // Test classes
    public interface ITestInterface
    {
        string Value { get; }
    }

    public abstract class AbstractTestClass
    {
        public abstract string Value { get; set; }
    }

    public class NoParameterlessCtorSource
    {
        public string Value { get; set; } = string.Empty;
    }

    public class NoParameterlessCtorDto : IMapFrom<NoParameterlessCtorSource>
    {
        public NoParameterlessCtorDto(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }

    public class GenericDto<T> : IMapWith<GenericSource<T>>
    {
        public T Value { get; init; } = default!;
    }

    public class GenericSource<T>
    {
        public T Value { get; set; } = default!;
    }

    public class InvalidMapMethodDto : IHaveMap
    {
        public string Value { get; init; } = string.Empty;

        // Required implementation
        public static void Map(Profile profile)
        {
            // Empty implementation for testing
        }

        // Invalid signature - should be ignored
        public static void Map(string invalidParameter)
        {
            // This should be ignored
        }
    }

    public class SelfReferenceEntity
    {
        public string Name { get; set; } = string.Empty;
        public SelfReferenceEntity? Self { get; set; }
    }

    public class SelfReferenceDto : IMapWith<SelfReferenceEntity>
    {
        public string Name { get; init; } = string.Empty;
        public SelfReferenceDto? Self { get; init; }
    }

    public class MultiSourceA
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class MultiSourceB
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class MultiInterfaceDto : IMapFrom<MultiSourceA>, IMapFrom<MultiSourceB>
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
    }

    public class NestedGenericSource<TKey, TValue>
    {
        public Dictionary<TKey, List<TValue>> Data { get; set; } = new();
    }

    public class NestedGenericDto<TKey, TValue> : IMapWith<NestedGenericSource<TKey, TValue>>
    {
        public Dictionary<TKey, List<TValue>> Data { get; init; } = new();
    }
} 