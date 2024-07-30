namespace Main.Tests;

[TestFixture]
public class NoParameterlessCtorTests
{
	[Test]
	public void ParameterlessCtorType_NotNull()
	{
		AssemblyMappingProfile profile = new(typeof(Dto));
		IMapper mapper = new Mapper(new MapperConfiguration(c => {
			c.AddProfile(profile);
		}));

		var model = mapper.Map<Dto>(Model.Some);

		model.Should().NotBeNull();
	} 
	public class Model
	{
		private Model(){}
		public string Value { get; set; }

		public static Model Some => new Model {
			Value = "Dddddd"
		};
	}
	
	public class Dto : IMapWith<Model>
	{
		public string Value { get; set; }

		private Dto()
		{
			
		}
	}
}