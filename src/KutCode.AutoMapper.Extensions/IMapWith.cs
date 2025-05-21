namespace AutoMapper;

/// <summary>
/// Interface for types that provide custom mapping configurations
/// </summary>
public interface IHaveMap
{
    /// <summary>
    /// Static method to configure custom mappings with Profile
    /// </summary>
    /// <param name="profile">Map Profile for configuring custom mappings</param>
    static abstract void Map(Profile profile);
}

/// <summary>
/// Base interface for mapping between types.
/// Implementing this interface creates a two-way mapping (with ReverseMap).
/// Serves as a base interface for <see cref="IMapFrom{TSource}"/> and <see cref="IMapTo{TDestination}"/> interfaces.
/// </summary>
/// <typeparam name="TMember">The type to map with</typeparam>
public interface IMapWith<TMember>;

/// <summary>
/// Interface that creates a mapping from <typeparamref name="TSource"/> to the implementing type.
/// </summary>
/// <typeparam name="TSource">The source type to map from</typeparam>
public interface IMapFrom<TSource> : IMapWith<TSource> {}

/// <summary>
/// Interface that creates a mapping from the implementing type to <typeparamref name="TDestination"/>.
/// </summary>
/// <typeparam name="TDestination">The destination type to map to</typeparam>
public interface IMapTo<TDestination> : IMapWith<TDestination> {}
