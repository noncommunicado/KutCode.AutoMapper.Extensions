
namespace Main.Tests.MapTo;

[TestFixture]
public class ConflictMappingTests
{
	[Test]
	public void TwoSimmilarMappings_ErrorThrowing()
	{
		AssemblyMappingProfile profile = new(typeof(ParentDto), typeof(ParentEntity));
		IMapper mapper = new Mapper(new MapperConfiguration(c => {
			c.AddProfile(profile);
		}));
		var mapped = mapper.Map<ParentDto>(new ParentEntity());
		Assert.Pass();
	}
	
	public class ParentEntity : IMapFrom<ParentEntity>
	{
		public string Value { get; set; } = "parent";

		public void MapFrom(MapProfileDecorator<ParentEntity> decorator)
		{
			decorator.Profile.CreateMap<ParentEntity, ParentDto>().ForMember(x => x.Value,
				opt => opt.MapFrom(z => "1111"));
		}
	}
	public record ParentDto : IMapFrom<ParentEntity>
	{
		public string Value { get; set; }


		public void MapFrom(MapProfileDecorator<ParentEntity> decorator)
		{
			decorator.Profile.CreateMap<ParentEntity, ParentDto>().ForMember(x => x.Value,
				opt => opt.MapFrom(z => "2222"));
		}
	}
}