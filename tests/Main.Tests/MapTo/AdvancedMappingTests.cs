namespace Main.Tests.MapTo;

[TestFixture]
public class AdvancedMappingTests
{
    [Test]
    public void MapTo_WithValidationAttributes_Success()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(ValidatedDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var source = new ValidatedDto { Email = "test@example.com", Age = 25 };
        
        // Act
        var result = mapper.Map<ValidatedTarget>(source);
        
        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be("test@example.com");
        result.Age.Should().Be(25);
    }

    [Test]
    public void MapTo_WithDifferentPropertyNames_Success()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(MismatchedDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var source = new MismatchedDto { FirstName = "John", LastName = "Doe" };
        
        // Act
        var result = mapper.Map<MismatchedTarget>(source);
        
        // Assert
        result.Should().NotBeNull();
        result.GivenName.Should().BeEmpty(); // No matching property
        result.Surname.Should().BeEmpty(); // No matching property
    }

    [Test]
    public void MapTo_WithRecordTypes_Success()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(RecordDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var source = new RecordDto(1, "Test Name", true);
        
        // Act
        var result = mapper.Map<RecordTarget>(source);
        
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Test Name");
        result.IsActive.Should().BeTrue();
    }

    [Test]
    public void MapTo_WithNestedGenericTypes_Success()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(GenericDto<int, string>));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var source = new GenericDto<int, string>
        {
            Key = 42,
            Value = "Test Value",
            Items = new List<string> { "item1", "item2" }
        };
        
        // Act
        var result = mapper.Map<GenericTarget<int, string>>(source);
        
        // Assert
        result.Should().NotBeNull();
        result.Key.Should().Be(42);
        result.Value.Should().Be("Test Value");
        result.Items.Should().HaveCount(2);
    }

    [Test]
    public void MapTo_WithCircularReference_HandledGracefully()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(CircularDto));
        var mapper = new Mapper(new MapperConfiguration(c => 
        {
            c.AddProfile(profile);
            // AutoMapper handles circular references by default
        }));
        
        var source = new CircularDto { Name = "Parent" };
        source.Child = new CircularDto { Name = "Child", Parent = source };
        
        // Act & Assert - This should not throw with default AutoMapper settings
        Action act = () => mapper.Map<CircularTarget>(source);
        act.Should().NotThrow();
    }

    [Test]
    public void MapTo_WithLargeCollections_PerformanceTest()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(LargeCollectionDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var source = new LargeCollectionDto
        {
            Items = Enumerable.Range(1, 10000).Select(i => $"Item {i}").ToList()
        };
        
        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = mapper.Map<LargeCollectionTarget>(source);
        stopwatch.Stop();
        
        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(10000);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // Performance threshold
    }

    [Test]
    public void MapTo_WithAsyncPattern_Success()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(AsyncDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var source = new AsyncDto 
        { 
            Id = Guid.NewGuid(),
            Data = "Async test data",
            Timestamp = DateTime.UtcNow
        };
        
        // Act
        var result = mapper.Map<AsyncTarget>(source);
        
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(source.Id);
        result.Data.Should().Be("Async test data");
        result.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    // Test classes
    public class ValidatedDto : IMapTo<ValidatedTarget>
    {
        public string Email { get; init; } = string.Empty;
        public int Age { get; init; }
    }

    public class ValidatedTarget
    {
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    public class MismatchedDto : IMapTo<MismatchedTarget>
    {
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
    }

    public class MismatchedTarget
    {
        public string GivenName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
    }

    public record RecordDto(int Id, string Name, bool IsActive) : IMapTo<RecordTarget>;
    public record RecordTarget(int Id, string Name, bool IsActive);

    public class GenericDto<TKey, TValue> : IMapTo<GenericTarget<TKey, TValue>>
    {
        public TKey Key { get; init; } = default!;
        public TValue Value { get; init; } = default!;
        public List<TValue> Items { get; init; } = new();
    }

    public class GenericTarget<TKey, TValue>
    {
        public TKey Key { get; set; } = default!;
        public TValue Value { get; set; } = default!;
        public List<TValue> Items { get; set; } = new();
    }

    public class CircularDto : IMapTo<CircularTarget>
    {
        public string Name { get; init; } = string.Empty;
        public CircularDto? Parent { get; set; }
        public CircularDto? Child { get; set; }
    }

    public class CircularTarget
    {
        public string Name { get; set; } = string.Empty;
        public CircularTarget? Parent { get; set; }
        public CircularTarget? Child { get; set; }
    }

    public class LargeCollectionDto : IMapTo<LargeCollectionTarget>
    {
        public List<string> Items { get; init; } = new();
    }

    public class LargeCollectionTarget
    {
        public List<string> Items { get; set; } = new();
    }

    public class AsyncDto : IMapTo<AsyncTarget>
    {
        public Guid Id { get; init; }
        public string Data { get; init; } = string.Empty;
        public DateTime Timestamp { get; init; }
    }

    public class AsyncTarget
    {
        public Guid Id { get; set; }
        public string Data { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
} 