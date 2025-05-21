using System.Reflection;

namespace AutoMapper;

/// <summary>
/// Profile that configures AutoMapper mappings based on interfaces
/// </summary>
internal class AssemblyMappingProfile : Profile
{
	private static readonly Type GenericMapWith = typeof(IMapWith<>);
	private static readonly Type GenericMapTo = typeof(IMapTo<>);
	private static readonly Type GenericMapFrom = typeof(IMapFrom<>);
	private static readonly Type HaveMap = typeof(IHaveMap);
	
	/// <summary>
	/// Creates a profile with mappings from the provided types
	/// </summary>
	/// <param name="types">Types to scan for mapping interfaces</param>
	public AssemblyMappingProfile(params Type[] types) : this((IEnumerable<Type>)types)
	{
	}
	
	/// <summary>
	/// Creates a profile with mappings from the provided types
	/// </summary>
	/// <param name="types">Types to scan for mapping interfaces</param>
	public AssemblyMappingProfile(IEnumerable<Type> types)
	{
		foreach (var type in types)
		{
			HandleMapping(type);
		}
	}
	
	private void HandleMapping(Type objectType)
	{
		// Get all implemented interfaces to check for mapping interfaces
		var objectInterfaces = objectType.GetInterfaces();
		
		foreach (var interfaceType in objectInterfaces)
		{
			if (interfaceType.IsGenericType)
			{
				var genericTypeDef = interfaceType.GetGenericTypeDefinition();
				var typeOfMapPartner = interfaceType.GenericTypeArguments.FirstOrDefault();
				
				if (typeOfMapPartner == null)
					continue;
					
				if (genericTypeDef == GenericMapWith)
					CreateMap(objectType, typeOfMapPartner).ReverseMap();
				else if (genericTypeDef == GenericMapTo)
					CreateMap(objectType, typeOfMapPartner);
				else if (genericTypeDef == GenericMapFrom)
					CreateMap(typeOfMapPartner, objectType);
			}
			else if (interfaceType.IsAssignableTo(HaveMap))
			{
				HandleCustomMapping(objectType);
			}
		}
	}

	private void HandleCustomMapping(Type objectType)
	{
		// Find the Map method using reflection
		MethodInfo? mapMethod = objectType.GetMethod(
			nameof(IHaveMap.Map),
			BindingFlags.Static | BindingFlags.Public,
			binder: null,
			types: [typeof(Profile)],
			modifiers: null
		);
		
		if (mapMethod is null)
			return;
		
		// Invoke the Map method to configure the mappings
		mapMethod.Invoke(null, [this]);
	}
}