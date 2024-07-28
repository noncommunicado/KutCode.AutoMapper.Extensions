namespace Main.Tests.MapTo;

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
		var fromModel = mapper.Map<ParentDto>(new ParentEntity());
		fromModel.Should().NotBeNull();
		fromModel.Value.Should().Be("parent");
		fromModel.Nested.Should().NotBeNull();
		fromModel.Nested.Counter.Should().Be(100);
	}
	
	public class ParentEntity
	{
		public string Value { get; set; } = "parent";
		public NestedEntity Nested { get; set; } = new();
	}
	public class NestedEntity
	{
		public int Counter { get; set; } = 100;
	}
	
	public record ParentDto : IMapTo<ParentEntity>
	{
		public string Value { get; set; }
		public NestedDto Nested { get; set; }
	}
	public record NestedDto : IMapTo<NestedEntity>
	{
		public int Counter { get; set; }
	}
}