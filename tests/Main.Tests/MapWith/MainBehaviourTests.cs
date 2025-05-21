namespace Main.Tests.MapWith;

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
		var toModel = mapper.Map<TestModel>(new TestDto {Value = "Model"});
		var toEntity = mapper.Map<TestEntity>(new TestDto {Value = "Entity"});

		fromEntity.Should().NotBeNull();
		fromEntity.Value.Should().Be("Entity");
		
		fromModel.Should().NotBeNull();
		fromModel.Value.Should().Be("SomeOverride");
		
		toEntity.Should().NotBeNull();
		toEntity.Value.Should().Be("Entity");
		
		toModel.Should().NotBeNull();
		toModel.Value.Should().Be("SomeOverride");
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
	public class TestDto : IMapWith<TestEntity>, IHaveMap, ISomeInterface
	{
		public string Value { get; set; }

		public static void Map(Profile profile)
		{
			profile.CreateMap<TestModel, TestDto>()
				.ForMember(x => x.Value, opt =>
					opt.MapFrom(x => "SomeOverride"));

			profile.CreateMap<TestDto, TestModel>()
				.ForMember(x => x.Value, opt =>
					opt.MapFrom(x => "SomeOverride"));
		}
	}
}