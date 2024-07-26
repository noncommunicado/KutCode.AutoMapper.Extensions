using System.Reflection;

namespace AutoMapper;

/// <summary>
/// Profile with all assembly's profiles
/// </summary>
internal class AssemblyMappingProfile : Profile 
{
	/// <summary>
	/// Scan assembly and sum all profiles in one
	/// </summary>
	public AssemblyMappingProfile(params Type[] types)
	{
		foreach (var type in types)
			ManageTypeMapping(type);
	}

	private void ManageTypeMapping(Type type)
	{
		var interfaces = type.GetInterfaces()
			.Where(x => x.Name == typeof(IMapWith<>).Name)
			.ToList();

		HashSet<Type> mapWithTypes = new HashSet<Type>(interfaces.Count);
		foreach (var inter in interfaces) {
			var gens = inter.GetGenericArguments();
			if (gens.Length == 1)
				mapWithTypes.Add(gens[0]);
		}

		var instance = Activator.CreateInstance(type);
		//Dictionary<Type, MethodInfo> overridedMethods = new(mapWithTypes.Count);
		foreach (var methodInfo in type.GetMethods().Where(x => x.Name == nameof(IMapWith<int>.Map))) {
			var genericParams = methodInfo.GetParameters()
				.Where(x => x.ParameterType.IsGenericType)
				.ToArray();
			if (genericParams.Length == 1) {
				var genArgs = genericParams[0].ParameterType.GetGenericArguments();
				if (genArgs.Length == 1) {
					//overridedMethods.Add(genArgs[0], methodInfo);
					mapWithTypes.Remove(genArgs[0]);
					methodInfo.Invoke(instance, new object?[] {this as Profile});
				}
			}
		}
		
	}
}