using System.Reflection;

namespace AutoMapper;

/// <summary>
/// Profile with all assembly's profiles
/// </summary>
internal class AssemblyMappingProfile : Profile
{
	public static Type GenericProfileType = typeof(MapProfileDecorator<>);
	
	/// <summary>
	/// Scan assembly and sum all profiles in one
	/// </summary>
	public AssemblyMappingProfile(params Type[] types)
	{
		foreach (var type in types)
			ManageTypeMapping(type);
	}
	
	private void ManageTypeMapping(Type objectType)
	{
		var objectInterfaces = objectType.GetInterfaces().ToList();
		foreach (var interfaceType in objectInterfaces.Where(x => x.IsGenericType)) {
			var genericTypeDef = interfaceType.GetGenericTypeDefinition();
			var typeOfMapPartner = interfaceType.GenericTypeArguments[0];
			if (genericTypeDef == typeof(IMapWith<>)) {
				if (GetMapMethod(objectType, typeOfMapPartner, nameof(IMapWith<object>.Map)) is var mapMethod && mapMethod is null) {
					this.CreateMap(objectType, typeOfMapPartner).ReverseMap();
				}
				else {
					var instance = objectType.IsValueType || objectType.GetConstructor(Type.EmptyTypes) != null 
						? Activator.CreateInstance(objectType) : null;
					mapMethod.Invoke(instance, new object?[] {
						Activator.CreateInstance(GenericProfileType.MakeGenericType(typeOfMapPartner), new object?[] {this})
					});
				}
			}
			else if (genericTypeDef == typeof(IMapTo<>)) {
				this.CreateMap(objectType, typeOfMapPartner);
			}
			else if (genericTypeDef == typeof(IMapFrom<>)) {
				this.CreateMap(typeOfMapPartner, objectType);
			}
		}
	}

	/// <summary>
	/// Get Map Method Information Object to call it from instance
	/// </summary>
	/// <param name="objectType">Type of main object</param>
	/// <param name="mapPartnerType">objectType partner in mapping</param>
	/// <param name="mapMethodName">name of the map method (because we have three interfaces with different map method names)</param>
	private static MethodInfo? GetMapMethod(Type objectType, Type mapPartnerType, string mapMethodName)
	{
		var mapMethod = objectType.GetMethods()
			.Where(m => 
				m.Name == mapMethodName
				&& m.GetParameters()
					.Where(z => 
						z.ParameterType.IsGenericType).ToArray()[0]
						.ParameterType.GenericTypeArguments[0] == mapPartnerType
				)
			.SingleOrDefault();
		return mapMethod;
	}
}