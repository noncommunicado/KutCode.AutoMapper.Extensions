namespace AutoMapper;

/// <summary>
/// Simple interface, for Reverse two-side mapping   
/// Also, provides base interface for IMapFrom and IMapTo interfaces
/// </summary>
public interface IMapWith<TMember>
{
	/// <summary>
	/// By default adds Reverse mapping to map-profile;
	/// Override it, for customize mapping
	/// </summary>
	/// <param name="decorator">Decorated Map Profile</param>
	public void Map(MapProfileDecorator<TMember> decorator)
		=> decorator.Profile.CreateMap(typeof(TMember), GetType()).ReverseMap();
}

/// <summary>
/// With applying of this interface, applying default AutoMapper CreateMap() from TDestination to Object Type
/// </summary>
/// <typeparam name="TSource">Destination type</typeparam>
public interface IMapFrom<TSource> : IMapWith<TSource> {}
/// <summary>
/// With applying of this interface, applying default AutoMapper CreateMap() from Object Type to TDestination
/// </summary>
/// <typeparam name="TDestination">Destination type</typeparam>
public interface IMapTo<TDestination> : IMapWith<TDestination> {}
