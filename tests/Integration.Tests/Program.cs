global using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAllMappings(); // <----- just add this
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
	IMapFrom<DataEntity>, IMapWith<DataModel>,
	ISomeDummyTestInterface, ISomeDummyTestInterface<object>
{
	public string Value { get; set; }
}