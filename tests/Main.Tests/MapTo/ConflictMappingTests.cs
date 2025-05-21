
namespace Main.Tests.MapTo;

[TestFixture]
public class ConflictMappingTests
{
	[Test]
	public void TwoSimmilarMappings_ErrorNotThrowing()
	{
		AssemblyMappingProfile profile = new(typeof(ParentDto), typeof(ParentEntity));
		IMapper mapper = new Mapper(new MapperConfiguration(c => {
			c.AddProfile(profile);
		}));
		var mapped = mapper.Map<ParentDto>(new ParentEntity());
		mapped.Value.Should().NotBeNullOrEmpty();
		Assert.Pass();
	}
	
	public class ParentEntity : IHaveMap
	{
		public string Value { get; set; } = "parent";

		public static void Map(Profile profile)
		{
			profile.CreateMap<ParentEntity, ParentDto>().ForMember(x => x.Value,
				opt => opt.MapFrom(z => "1111"));
		}
	}
	public record ParentDto : IHaveMap
	{
		public string Value { get; set; }

		public static void Map(Profile profile)
		{
			profile.CreateMap<ParentEntity, ParentDto>().ForMember(x => x.Value,
				opt => opt.MapFrom(z => "2222"));
		}
	}
}