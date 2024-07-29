using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AutoMapper;

/// <summary>
/// Default container injections
/// </summary>
public static class DependencyInjection
{
	private static Type _iMapWith = typeof(IMapWith<>);
	
	/// <summary>
	/// Add all profiles from Application Domain assemblies
	/// </summary>
	/// <param name="services">A collection of service descriptors</param>
	public static IServiceCollection AddAllMappings(this IServiceCollection services)
		=> AddMappings(services, AppDomain.CurrentDomain.GetAssemblies());
	
	/// <summary>
	/// Add sprecified profiles from assemblies
	/// </summary>
	/// <param name="services">A collection of service descriptors</param>
	/// <param name="assemblies">Assemblies to scan object inhirited from <see cref="IMapTo{TDestination}"/></param>
	public static IServiceCollection AddMappings(this IServiceCollection services, params Assembly[] assemblies)
	{
		services.AddAutoMapper(cfg =>{
			foreach (var assembly in assemblies) {
				var types = assembly.GetExportedTypes()
					.Where(x => x.IsInterface == false)
					.Where(type => type.GetInterfaces()
						.Any(i => i.IsGenericType && i.GetGenericTypeDefinition().IsAssignableFrom(_iMapWith)))
					
					.ToArray();
				if (types.Length == 0) continue;
				cfg.AddProfile(new AssemblyMappingProfile(types));
			}
		});
		
		return services;
	}
}