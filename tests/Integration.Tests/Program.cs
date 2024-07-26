global using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAllMappings(); // <----- just add this
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/get-some-data-1", (IMapper autoMapper) => {
		var entity = new DataEntity {Value = "entity-test-data"};
		return autoMapper.Map<DataDto>(entity);
	})
	.WithName("GetSomeData1")
	.WithOpenApi();

app.MapGet("/get-some-data-2", (IMapper autoMapper) => {
		var entity = new DataModel {Value = "model-test-data"};
		return autoMapper.Map<DataDto>(entity);
	})
	.WithName("GetSomeData2")
	.WithOpenApi();

app.Run();


public class DataEntity
{
	public string Value { get; set; }
}

public class DataModel
{
	public string Value { get; set; }
}

public class DataDto : IMapWith<DataEntity>, IMapWith<DataModel>
{
	public string Value { get; set; }
	public void Map(Profile<DataEntity> profile)
	{
		profile.CreateMap<DataEntity, DataDto>();
	}
}