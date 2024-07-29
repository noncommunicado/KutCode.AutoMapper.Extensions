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
	
	private sealed record TypeManageDto(Type MainObjectType, Type TypeOfMapPartner);
	private void ManageTypeMapping(Type objectType)
	{
		var objectInterfaces = objectType.GetInterfaces().ToList();
		foreach (var interfaceType in objectInterfaces.Where(x => x.IsGenericType)) {
			var genericTypeDef = interfaceType.GetGenericTypeDefinition();
			var typeOfMapPartner = interfaceType.GenericTypeArguments[0];
			TypeManageDto manageDto = new(objectType, typeOfMapPartner);
			if (genericTypeDef == typeof(IMapWith<>)) {
				InvokeOverrideOrDefaultMapping(manageDto, nameof(IMapWith<object>.Map),
					() => this.CreateMap(objectType, typeOfMapPartner).ReverseMap());
			}
			else if (genericTypeDef == typeof(IMapTo<>)) {
				InvokeOverrideOrDefaultMapping(manageDto, nameof(IMapTo<object>.MapTo),
					() => this.CreateMap(objectType, typeOfMapPartner));
			}
			else if (genericTypeDef == typeof(IMapFrom<>)) {
				InvokeOverrideOrDefaultMapping(manageDto, nameof(IMapFrom<object>.MapFrom),
					() => this.CreateMap(typeOfMapPartner, objectType));
			}
		}
	}

	private void InvokeOverrideOrDefaultMapping(
		TypeManageDto manageDto,
		string mapMethodName,
		Func<IMappingExpression> defaultMapMethod)
	{
		// if mapping method is not implemented use default and simpliest mapping
		if (GetMapMethod(manageDto.MainObjectType, manageDto.TypeOfMapPartner, mapMethodName) is var mapMethod && mapMethod is null) {
			defaultMapMethod.Invoke();
		}
		else {
			var instance = manageDto.MainObjectType.IsValueType || manageDto.MainObjectType.GetConstructor(Type.EmptyTypes) != null 
				? Activator.CreateInstance(manageDto.MainObjectType) : null;
			mapMethod.Invoke(instance, new object?[] {
				Activator.CreateInstance(GenericProfileType.MakeGenericType(manageDto.TypeOfMapPartner), new object?[] {this})
			});
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