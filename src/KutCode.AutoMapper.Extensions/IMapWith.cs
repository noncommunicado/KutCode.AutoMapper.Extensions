namespace AutoMapper;

/// <summary>
/// Interface wich you need to implement to describe mapping profile
/// </summary>
/// <typeparam name="TDestination">Destination type</typeparam>
public interface IMapWith<TDestination>
{
	public void Map(Profile<TDestination> profile) => profile.CreateMap(typeof(TDestination), GetType());
}
