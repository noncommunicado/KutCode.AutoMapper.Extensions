namespace Main.Tests.MapWith;

[TestFixture]
public class AdvancedMappingTests
{
    [Test]
    public void MapWith_BidirectionalMapping_Success()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(BidirectionalDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var entity = new BidirectionalEntity { Id = 1, Name = "Entity", Value = 100m };
        
        // Act - Map to DTO
        var dto = mapper.Map<BidirectionalDto>(entity);
        
        // Act - Map back to Entity
        var backToEntity = mapper.Map<BidirectionalEntity>(dto);
        
        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(1);
        dto.Name.Should().Be("Entity");
        dto.Value.Should().Be(100m);
        
        backToEntity.Should().NotBeNull();
        backToEntity.Id.Should().Be(1);
        backToEntity.Name.Should().Be("Entity");
        backToEntity.Value.Should().Be(100m);
    }

    [Test]
    public void MapWith_WithComplexHierarchy_Success()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(HierarchyDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var entity = new HierarchyEntity
        {
            Id = 1,
            Parent = new HierarchyEntity { Id = 2, Name = "Parent" },
            Children = new List<HierarchyEntity>
            {
                new() { Id = 3, Name = "Child1" },
                new() { Id = 4, Name = "Child2" }
            }
        };
        
        // Act
        var dto = mapper.Map<HierarchyDto>(entity);
        var backToEntity = mapper.Map<HierarchyEntity>(dto);
        
        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(1);
        dto.Parent.Should().NotBeNull();
        dto.Parent.Name.Should().Be("Parent");
        dto.Children.Should().HaveCount(2);
        
        backToEntity.Should().NotBeNull();
        backToEntity.Children.Should().HaveCount(2);
    }

    [Test]
    public void MapWith_WithValueObjects_Success()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(ValueObjectDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var entity = new ValueObjectEntity
        {
            Money = new Money(100.50m, "USD"),
            Address = new Address("123 Main St", "City", "12345")
        };
        
        // Act
        var dto = mapper.Map<ValueObjectDto>(entity);
        var backToEntity = mapper.Map<ValueObjectEntity>(dto);
        
        // Assert
        dto.Should().NotBeNull();
        dto.Money.Should().NotBeNull();
        dto.Money.Amount.Should().Be(100.50m);
        dto.Money.Currency.Should().Be("USD");
        
        backToEntity.Should().NotBeNull();
        backToEntity.Address.Street.Should().Be("123 Main St");
    }

    [Test]
    public void MapWith_WithDifferentCollectionTypes_Success()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(CollectionTypesDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var entity = new CollectionTypesEntity
        {
            StringArray = new[] { "a", "b", "c" },
            IntHashSet = new HashSet<int> { 1, 2, 3 },
            StringListProperty = new List<string> { "x", "y", "z" }
        };
        
        // Act
        var dto = mapper.Map<CollectionTypesDto>(entity);
        var backToEntity = mapper.Map<CollectionTypesEntity>(dto);
        
        // Assert
        dto.Should().NotBeNull();
        dto.StringArray.Should().HaveCount(3);
        dto.IntHashSet.Should().HaveCount(3);
        dto.StringListProperty.Should().HaveCount(3);
        
        backToEntity.Should().NotBeNull();
        backToEntity.StringArray.Should().HaveCount(3);
    }

    [Test]
    public void MapWith_WithNullValues_HandledCorrectly()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(NullableFieldsDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var entity = new NullableFieldsEntity
        {
            RequiredField = "Required",
            OptionalField = null,
            NullableInt = null,
            OptionalObject = null
        };
        
        // Act
        var dto = mapper.Map<NullableFieldsDto>(entity);
        var backToEntity = mapper.Map<NullableFieldsEntity>(dto);
        
        // Assert
        dto.Should().NotBeNull();
        dto.RequiredField.Should().Be("Required");
        dto.OptionalField.Should().BeNull();
        dto.NullableInt.Should().BeNull();
        dto.OptionalObject.Should().BeNull();
        
        backToEntity.Should().NotBeNull();
        backToEntity.OptionalField.Should().BeNull();
    }

    [Test]
    public void MapWith_WithInterfaceProperties_Success()
    {
        // Arrange
        var profile = new AssemblyMappingProfile(typeof(InterfaceDto));
        var mapper = new Mapper(new MapperConfiguration(c => c.AddProfile(profile)));
        
        var entity = new InterfaceEntity
        {
            Service = new ConcreteService { Name = "TestService" },
            Items = new List<IItem> 
            { 
                new ConcreteItem { Value = "Item1" },
                new ConcreteItem { Value = "Item2" }
            }
        };
        
        // Act
        var dto = mapper.Map<InterfaceDto>(entity);
        
        // Assert
        dto.Should().NotBeNull();
        dto.Service.Should().NotBeNull();
        dto.Items.Should().HaveCount(2);
    }

    // Test classes
    public class BidirectionalEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }

    public class BidirectionalDto : IMapWith<BidirectionalEntity>
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public decimal Value { get; init; }
    }

    public class HierarchyEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public HierarchyEntity? Parent { get; set; }
        public List<HierarchyEntity> Children { get; set; } = new();
    }

    public class HierarchyDto : IMapWith<HierarchyEntity>
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public HierarchyDto? Parent { get; init; }
        public List<HierarchyDto> Children { get; init; } = new();
    }

    public record Money(decimal Amount, string Currency);
    public record Address(string Street, string City, string ZipCode);

    public class ValueObjectEntity
    {
        public Money Money { get; set; } = new(0, "USD");
        public Address Address { get; set; } = new("", "", "");
    }

    public class ValueObjectDto : IMapWith<ValueObjectEntity>
    {
        public Money Money { get; init; } = new(0, "USD");
        public Address Address { get; init; } = new("", "", "");
    }

    public class CollectionTypesEntity
    {
        public string[] StringArray { get; set; } = Array.Empty<string>();
        public HashSet<int> IntHashSet { get; set; } = new();
        public List<string> StringListProperty { get; set; } = new();
    }

    public class CollectionTypesDto : IMapWith<CollectionTypesEntity>
    {
        public string[] StringArray { get; init; } = Array.Empty<string>();
        public HashSet<int> IntHashSet { get; init; } = new();
        public List<string> StringListProperty { get; init; } = new();
    }

    public class NullableFieldsEntity
    {
        public string RequiredField { get; set; } = string.Empty;
        public string? OptionalField { get; set; }
        public int? NullableInt { get; set; }
        public object? OptionalObject { get; set; }
    }

    public class NullableFieldsDto : IMapWith<NullableFieldsEntity>
    {
        public string RequiredField { get; init; } = string.Empty;
        public string? OptionalField { get; init; }
        public int? NullableInt { get; init; }
        public object? OptionalObject { get; init; }
    }

    public interface IService
    {
        string Name { get; }
    }

    public interface IItem
    {
        string Value { get; }
    }

    public class ConcreteService : IService
    {
        public string Name { get; set; } = string.Empty;
    }

    public class ConcreteItem : IItem
    {
        public string Value { get; set; } = string.Empty;
    }

    public class InterfaceEntity
    {
        public IService Service { get; set; } = new ConcreteService();
        public List<IItem> Items { get; set; } = new();
    }

    public class InterfaceDto : IMapWith<InterfaceEntity>
    {
        public IService Service { get; init; } = new ConcreteService();
        public List<IItem> Items { get; init; } = new();
    }
} 