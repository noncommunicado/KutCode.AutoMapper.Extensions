namespace Main.Tests.MapFrom;

[TestFixture]
public class AdvancedMappingTests
{
    [Test]
    public void MapFrom_WithNullableProperties_Success()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(NullableDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var source = new NullableSource { Id = 1, Name = null, Age = 25 };
        
        // Act
        var result = mapper.Map<NullableDto>(source);
        
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().BeNull();
        result.Age.Should().Be(25);
    }

    [Test]
    public void MapFrom_WithComplexNestedObjects_Success()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(ComplexDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var source = new ComplexSource
        {
            Id = 1,
            Person = new PersonSource { Name = "John", Age = 30 },
            Tags = new List<string> { "tag1", "tag2" },
            Metadata = new Dictionary<string, object> { { "key", "value" } }
        };
        
        // Act
        var result = mapper.Map<ComplexDto>(source);
        
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Person.Should().NotBeNull();
        result.Person.Name.Should().Be("John");
        result.Person.Age.Should().Be(30);
        result.Tags.Should().HaveCount(2);
        result.Tags.Should().Contain("tag1");
        result.Metadata.Should().ContainKey("key");
    }

    [Test]
    public void MapFrom_WithInheritance_Success()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(DerivedDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var source = new DerivedSource 
        { 
            BaseProperty = "base", 
            DerivedProperty = "derived" 
        };
        
        // Act
        var result = mapper.Map<DerivedDto>(source);
        
        // Assert
        result.Should().NotBeNull();
        result.BaseProperty.Should().Be("base");
        result.DerivedProperty.Should().Be("derived");
    }

    [Test]
    public void MapFrom_WithGenericCollections_Success()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(CollectionDto<string>));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var source = new CollectionSource<string>
        {
            Items = new List<string> { "item1", "item2", "item3" },
            Count = 3
        };
        
        // Act
        var result = mapper.Map<CollectionDto<string>>(source);
        
        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
        result.Items.Should().Contain("item1");
        result.Count.Should().Be(3);
    }

    [Test]
    public void MapFrom_WithEnumsAndDateTimes_Success()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(EnumDateDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var source = new EnumDateSource
        {
            Status = SourceStatus.Active,
            CreatedAt = new DateTime(2023, 1, 1),
            ModifiedAt = DateTimeOffset.Now
        };
        
        // Act
        var result = mapper.Map<EnumDateDto>(source);
        
        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(DtoStatus.Active);
        result.CreatedAt.Should().Be(new DateTime(2023, 1, 1));
        result.ModifiedAt.Should().BeCloseTo(DateTimeOffset.Now, TimeSpan.FromSeconds(1));
    }

    // Test classes
    public class NullableSource
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Age { get; set; }
    }

    public class NullableDto : IMapFrom<NullableSource>
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public int? Age { get; init; }
    }

    public class PersonSource
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    public class ComplexSource
    {
        public int Id { get; set; }
        public PersonSource Person { get; set; } = new();
        public List<string> Tags { get; set; } = new();
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class ComplexDto : IMapFrom<ComplexSource>
    {
        public int Id { get; init; }
        public PersonSource Person { get; init; } = new();
        public List<string> Tags { get; init; } = new();
        public Dictionary<string, object> Metadata { get; init; } = new();
    }

    public class BaseSource
    {
        public string BaseProperty { get; set; } = string.Empty;
    }

    public class DerivedSource : BaseSource
    {
        public string DerivedProperty { get; set; } = string.Empty;
    }

    public class DerivedDto : IMapFrom<DerivedSource>
    {
        public string BaseProperty { get; init; } = string.Empty;
        public string DerivedProperty { get; init; } = string.Empty;
    }

    public class CollectionSource<T>
    {
        public List<T> Items { get; set; } = new();
        public int Count { get; set; }
    }

    public class CollectionDto<T> : IMapFrom<CollectionSource<T>>
    {
        public List<T> Items { get; init; } = new();
        public int Count { get; init; }
    }

    public enum SourceStatus { Inactive, Active, Suspended }
    public enum DtoStatus { Inactive, Active, Suspended }

    public class EnumDateSource
    {
        public SourceStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
    }

    public class EnumDateDto : IMapFrom<EnumDateSource>
    {
        public DtoStatus Status { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTimeOffset ModifiedAt { get; init; }
    }
} 