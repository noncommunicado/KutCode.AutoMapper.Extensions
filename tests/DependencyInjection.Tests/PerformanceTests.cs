namespace DependencyInjection.Tests;

[TestFixture]
public class PerformanceTests
{
    [Test]
    public void AddMappings_WithManyTypes_PerformanceTest()
    {
        // Arrange
        var services = new ServiceCollection();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        // Act - Add mappings for many types
        services.AddMappings(typeof(PerformanceTests).Assembly);
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetService<IMapper>();
        stopwatch.Stop();
        
        // Assert
        mapper.Should().NotBeNull();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000); // Should complete within 5 seconds
    }

    [Test]
    public void AddAllMappings_LargeAssemblySet_PerformanceTest()
    {
        // Arrange
        var services = new ServiceCollection();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        // Act
        services.AddAllMappings();
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetService<IMapper>();
        stopwatch.Stop();
        
        // Assert
        mapper.Should().NotBeNull();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(10000); // Should complete within 10 seconds
    }

    [Test]
    public void Mapping_LargeObjectGraph_PerformanceTest()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMappings(typeof(PerformanceTests).Assembly);
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetRequiredService<IMapper>();
        
        var largeEntity = CreateLargeObjectGraph();
        
        // Act - Measure mapping performance
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 1000; i++)
        {
            var dto = mapper.Map<LargeDto>(largeEntity);
        }
        stopwatch.Stop();
        
        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(2000); // 1000 mappings in under 2 seconds
    }

    [Test]
    public void Mapping_ConcurrentAccess_ThreadSafetyTest()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMappings(typeof(PerformanceTests).Assembly);
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetRequiredService<IMapper>();
        
        var entity = new ThreadSafeEntity { Id = 1, Name = "Test", Value = 100m };
        var tasks = new List<Task>();
        var exceptions = new ConcurrentBag<Exception>();
        
        // Act - Run concurrent mappings
        for (int i = 0; i < 50; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                try
                {
                    for (int j = 0; j < 100; j++)
                    {
                        var dto = mapper.Map<ThreadSafeDto>(entity);
                        dto.Should().NotBeNull();
                        dto.Id.Should().Be(1);
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }));
        }
        
        Task.WaitAll(tasks.ToArray());
        
        // Assert
        exceptions.Should().BeEmpty("Mapping should be thread-safe");
    }

    [Test]
    public void AddMappings_MemoryUsage_WithinReasonableLimits()
    {
        // Arrange
        var initialMemory = GC.GetTotalMemory(true);
        var services = new ServiceCollection();
        
        // Act
        services.AddMappings(typeof(PerformanceTests).Assembly);
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetService<IMapper>();
        
        var finalMemory = GC.GetTotalMemory(true);
        var memoryUsed = finalMemory - initialMemory;
        
        // Assert
        mapper.Should().NotBeNull();
        memoryUsed.Should().BeLessThan(50 * 1024 * 1024); // Less than 50MB
    }

    [Test]
    public void AddMappings_WithComplexInheritance_NoStackOverflow()
    {
        // Arrange
        var services = new ServiceCollection();
        
        // Act & Assert - Should not throw StackOverflowException
        Action act = () =>
        {
            services.AddMappings(typeof(PerformanceTests).Assembly);
            var serviceProvider = services.BuildServiceProvider();
            var mapper = serviceProvider.GetRequiredService<IMapper>();
            
            var complexEntity = new ComplexInheritanceEntity
            {
                BaseProperty = "Base",
                DerivedProperty = "Derived",
                NestedEntity = new NestedEntity { Value = "Nested" }
            };
            
            var result = mapper.Map<ComplexInheritanceDto>(complexEntity);
            result.Should().NotBeNull();
        };
        
        act.Should().NotThrow();
    }

    [Test]
    public void Mapping_DeepNesting_HandledGracefully()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddMappings(typeof(PerformanceTests).Assembly);
        var serviceProvider = services.BuildServiceProvider();
        var mapper = serviceProvider.GetRequiredService<IMapper>();
        
        var deepEntity = CreateDeeplyNestedEntity(10); // 10 levels deep
        
        // Act & Assert
        Action act = () =>
        {
            var result = mapper.Map<DeepNestedDto>(deepEntity);
            result.Should().NotBeNull();
        };
        
        act.Should().NotThrow();
    }

    // Helper methods
    private LargeEntity CreateLargeObjectGraph()
    {
        return new LargeEntity
        {
            Id = 1,
            Name = "Large Entity",
            Items = Enumerable.Range(1, 1000)
                .Select(i => new ItemEntity { Id = i, Name = $"Item {i}" })
                .ToList(),
            Metadata = Enumerable.Range(1, 500)
                .ToDictionary(i => $"Key{i}", i => $"Value{i}"),
            Tags = Enumerable.Range(1, 100).Select(i => $"Tag{i}").ToList()
        };
    }

    private DeepNestedEntity CreateDeeplyNestedEntity(int depth)
    {
        if (depth <= 0)
            return new DeepNestedEntity { Value = "Leaf", Level = 0 };
        
        return new DeepNestedEntity 
        { 
            Value = $"Level {depth}",
            Level = depth,
            Child = CreateDeeplyNestedEntity(depth - 1)
        };
    }

    // Test classes
    public class LargeEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ItemEntity> Items { get; set; } = new();
        public Dictionary<string, string> Metadata { get; set; } = new();
        public List<string> Tags { get; set; } = new();
    }

    public class ItemEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class LargeDto : IMapWith<LargeEntity>
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public List<ItemEntity> Items { get; init; } = new();
        public Dictionary<string, string> Metadata { get; init; } = new();
        public List<string> Tags { get; init; } = new();
    }

    public class ThreadSafeEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }

    public class ThreadSafeDto : IMapWith<ThreadSafeEntity>
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public decimal Value { get; init; }
    }

    public class BaseEntity
    {
        public string BaseProperty { get; set; } = string.Empty;
    }

    public class ComplexInheritanceEntity : BaseEntity
    {
        public string DerivedProperty { get; set; } = string.Empty;
        public NestedEntity NestedEntity { get; set; } = new();
    }

    public class NestedEntity
    {
        public string Value { get; set; } = string.Empty;
    }

    public class ComplexInheritanceDto : IMapWith<ComplexInheritanceEntity>
    {
        public string BaseProperty { get; init; } = string.Empty;
        public string DerivedProperty { get; init; } = string.Empty;
        public NestedEntity NestedEntity { get; init; } = new();
    }

    public class DeepNestedEntity
    {
        public string Value { get; set; } = string.Empty;
        public int Level { get; set; }
        public DeepNestedEntity? Child { get; set; }
    }

    public class DeepNestedDto : IMapWith<DeepNestedEntity>
    {
        public string Value { get; init; } = string.Empty;
        public int Level { get; init; }
        public DeepNestedDto? Child { get; init; }
    }
} 