namespace Main.Tests.MapTo;

[TestFixture]
public class MainBehaviourTests
{
	[Test]
	public void TwoNotConcurrentMappings_Success()
	{
		AssemblyMappingProfile profile = new(typeof(TestDto));
		IMapper mapper = new Mapper(new MapperConfiguration(c => {
			c.AddProfile(profile);
		}));
		
		var fromModel = mapper.Map<TestDto>(new TestModel {Value = "Model"});
		var fromEntity = mapper.Map<TestDto>(new TestEntity {Value = "Entity"});

		fromEntity.Should().NotBeNull();
		fromEntity.Value.Should().Be("Entity");
		
		fromModel.Should().NotBeNull();
		fromModel.Value.Should().Be("SomeOverride");
	}
	
	public class TestEntity
	{
		public string Value { get; set; }
	}
	public class TestModel
	{
		public string Value { get; set; }
	}
	public interface ISomeInterface { }
	public class TestDto : IMapTo<TestEntity>, IMapTo<TestModel>, ISomeInterface
	{
		public string Value { get; set; }

		public void Map(MapProfileDecorator<TestModel> decorator)
		{
			decorator.Profile.CreateMap<TestModel, TestDto>()
				.ForMember(x => x.Value, opt => 
					opt.MapFrom(x => "SomeOverride"));
		}
	}
}