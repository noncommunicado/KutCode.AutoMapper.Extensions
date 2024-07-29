namespace Main.Tests.MapWith;

[TestFixture]
public class NestingMappingsTests
{
	[Test]
	public void NestedMapping_Success()
	{
		AssemblyMappingProfile profile = new(typeof(ParentDto), typeof(NestedDto));
		IMapper mapper = new Mapper(new MapperConfiguration(c => {
			c.AddProfile(profile);
		}));
		var fromModel = mapper.Map<ParentDto>(new ParentEntity {
			Value = "parent", Nested = new () { Counter = 100 }
		});
		fromModel.Should().NotBeNull();
		fromModel.Value.Should().Be("parent");
		fromModel.Nested.Should().NotBeNull();
		fromModel.Nested.Counter.Should().Be(100);
		
		
		var toModel = mapper.Map<ParentEntity>(new ParentDto {
			Value = "parent", Nested = new () { Counter = 100 }
		});
		toModel.Should().NotBeNull();
		toModel.Value.Should().Be("parent");
		toModel.Nested.Should().NotBeNull();
		toModel.Nested.Counter.Should().Be(100);
	}
	
	public class ParentEntity
	{
		public string Value { get; set; }
		public NestedEntity Nested { get; set; } 
	}
	public class NestedEntity
	{
		public int Counter { get; set; } 
	}
	
	public record ParentDto : IMapWith<ParentEntity>
	{
		public string Value { get; set; } 
		public NestedDto Nested { get; set; }
	}
	public record NestedDto : IMapWith<NestedEntity>
	{
		public int Counter { get; set; }
	}
}