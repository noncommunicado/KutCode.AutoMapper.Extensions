
namespace AutoMapper;

public sealed class MapProfileDecorator<TDestination>
{
	public Profile Profile { get; }
	public MapProfileDecorator(Profile profile)
	{
		Profile = profile;
	}
}