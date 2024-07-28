
namespace AutoMapper;

public class MapOptions
{
	/// <summary>
	/// If True, throws <see cref="DuplicateTypeMapConfigurationException"/> when duplicated map configurations occures
	/// </summary>
	public bool ThrowOnDuplicateMappings { get; set; } = false;
}