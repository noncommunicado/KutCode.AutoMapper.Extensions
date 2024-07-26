
namespace AutoMapper;

public sealed class Profile<TDestination> : Profile
{
	private Profile _profile;
	public Profile(Profile profile)
	{
		_profile = profile;
	}
}