namespace AutoMapper;

/// <summary>
/// Simple interface, for Reverse two-side mapping   
/// Also, provides base interface for IMapFrom and IMapTo interfaces
/// </summary>
public interface IMapWith<TMember>
{
	
}

/// <summary>
/// With applying of this interface, applying default AutoMapper CreateMap() from TDestination to Object Type
/// </summary>
/// <typeparam name="TSource">Destination type</typeparam>
public interface IMapFrom<TSource> : IMapWith<TSource>
{
	/// <summary>
	/// Adds Mapping to <see cref="decorator.Profile"/> Map Profile
	/// </summary>
	/// <param name="decorator">Decorated Map Profile</param>
	public void MapFrom(MapProfileDecorator<TSource> decorator)
		=> decorator.Profile.CreateMap(typeof(TSource), GetType());
}
/// <summary>
/// With applying of this interface, applying default AutoMapper CreateMap() from Object Type to TDestination
/// </summary>
/// <typeparam name="TDestination">Destination type</typeparam>
public interface IMapTo<TDestination> : IMapWith<TDestination>
{
	/// <summary>
	/// Adds Mapping to <see cref="decorator.Profile"/> Map Profile
	/// </summary>
	/// <param name="decorator">Decorated Map Profile</param>
	public void MapTo(MapProfileDecorator<TDestination> decorator)
		=> decorator.Profile.CreateMap(GetType(), typeof(TDestination));
}
