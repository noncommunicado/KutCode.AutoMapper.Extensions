global using AutoMapper;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddAllMappings(catchDefaultProfiles: true); // <----- just add this
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/map-from-entity", (IMapper autoMapper) => {
		var entity = new DataEntity {Value = "entity-test-data"};
		return autoMapper.Map<DataDto>(entity);
	})
	.WithName("map-from-entity")
	.WithOpenApi();

app.MapGet("/map-from-model", (IMapper autoMapper) => {
		var entity = new DataModel {Value = "model-test-data"};
		return autoMapper.Map<DataDto>(entity);
	})
	.WithName("map-from-model")
	.WithOpenApi();

app.Run();

public interface ISomeDummyTestInterface {}
public interface ISomeDummyTestInterface<T> {}
public class DataEntity
{
	public string Value { get; set; }
}

public class DataModel
{
	public string Value { get; set; }
}

public class DataDto : 
	IHaveMap,
	IMapTo<DataEntity>,
	IMapWith<DataModel>,
	ISomeDummyTestInterface, ISomeDummyTestInterface<object>
{
	public string Value { get; set; }
	
	public static void Map(Profile profile)
	{
		profile.CreateMap<DataDto, DataEntity>()
			.ForMember(m => m.Value, opt 
				=> opt.MapFrom(f => "SomeOverride")
			);
	}
}