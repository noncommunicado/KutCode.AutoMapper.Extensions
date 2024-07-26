using AutoMapper;

namespace Main.Tests;

public class Tests
{
	[SetUp]
	public void Setup() { }

	[Test]
	public void Test1()
	{
		AssemblyMappingProfile profile = new(typeof(TestDto));
		//var mapper = new AutoMapper.Mapper()
		Assert.Pass();
	}
	
	
	public class TestEntity
	{
		public string Value { get; set; }
	}
	public class TestModel
	{
		public string Value { get; set; }
	}
	public interface ISomeInterface
	{
		
	}
	public class TestDto : IMapWith<TestEntity>, IMapWith<TestModel>, ISomeInterface
	{
		public string Value { get; set; }
		public void Map(Profile<TestModel> profile)
		{
			profile.CreateMap<TestModel, TestEntity>()
				.ForMember(x => x.Value, opt => 
					opt.MapFrom(x => "SomeOverride"));
		}
	}
}