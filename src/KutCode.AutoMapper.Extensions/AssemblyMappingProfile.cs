using AutoMapper.Internal;

namespace AutoMapper;

/// <summary>
/// Profile with all assembly's profiles
/// </summary>
internal class AssemblyMappingProfile : Profile
{
	private readonly MapOptions _options = new();
	public static Type GenericProfileType = typeof(MapProfileDecorator<>);
	
	/// <summary>
	/// Scan assembly and sum all profiles in one
	/// </summary>
	public AssemblyMappingProfile(params Type[] types)
	{
		foreach (var type in types)
			ManageTypeMapping(type);
	}
	
	/// <summary>
	/// Scan assembly and sum all profiles in one
	/// </summary>
	/// <param name="options">Mapping options</param>
	public AssemblyMappingProfile(MapOptions options, params Type[] types)
	{
		_options = options;
		foreach (var type in types)
			ManageTypeMapping(type);
	}

	private void ManageTypeMapping(Type interfacedObjectType)
	{
		var typeMapInterfaces = interfacedObjectType.GetInterfaces()
			.Where(x => x.Name == typeof(IMapTo<>).Name)
			.ToList();

		HashSet<Type> typesForDefaultMap = new(typeMapInterfaces.Count);
		foreach (var inter in typeMapInterfaces) {
			var gens = inter.GetGenericArguments();
			if (_options.ThrowOnDuplicateMappings && typesForDefaultMap.Contains(gens[0]))
				throw new DuplicateTypeMapConfigurationException(new[] {
					new DuplicateTypeMapConfigurationException.TypeMapConfigErrors(
						new TypePair(interfacedObjectType, gens[0]), new []{ProfileName})
				});
			typesForDefaultMap.Add(gens[0]);
		}

		var mapMethods = interfacedObjectType.GetMethods().Where(x => x.Name == nameof(IMapTo<int>.MapTo)).ToList();
		var instance = Activator.CreateInstance(interfacedObjectType);
		foreach (var methodInfo in mapMethods) {
			var genericParams = methodInfo.GetParameters()
				.Where(x => x.ParameterType.IsGenericType)
				.ToArray();
			var genArgs = genericParams[0].ParameterType.GetGenericArguments();
			if (genArgs.Length == 1) {
				var ctorParam = Activator.CreateInstance(GenericProfileType.MakeGenericType(genArgs[0]), new object?[] {this});
				typesForDefaultMap.Remove(genArgs[0]);
				methodInfo.Invoke(instance, new object?[] {ctorParam});
			}
		}

		// create map by default, if no Map() method override presented
		foreach (Type type in typesForDefaultMap)
			if (type != interfacedObjectType)
				CreateMap(type, interfacedObjectType);
	}
}