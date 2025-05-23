global using AutoMapper;

public interface ISomeDummyTestInterface {}
public interface ISomeDummyTestInterface<T> {}

public class DataEntity
{
	public string Value { get; set; } = string.Empty;
}

public class DataModel
{
	public string Value { get; set; } = string.Empty;
}

public class DataDto : 
	IHaveMap,
	IMapTo<DataEntity>,
	IMapWith<DataModel>,
	ISomeDummyTestInterface, ISomeDummyTestInterface<object>
{
	public string Value { get; set; } = string.Empty;
	
	public static void Map(Profile profile)
	{
		profile.CreateMap<DataDto, DataEntity>()
			.ForMember(m => m.Value, opt 
				=> opt.MapFrom(f => "SomeOverride")
			);
	}
}