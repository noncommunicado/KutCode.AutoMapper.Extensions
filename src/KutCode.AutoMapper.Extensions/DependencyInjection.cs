using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AutoMapper;

/// <summary>
/// Extension methods for setting up AutoMapper in an <see cref="IServiceCollection" />
/// </summary>
public static class DependencyInjection
{
	private static readonly Type MapWith = typeof(IMapWith<>);
	private static readonly Type HaveMap = typeof(IHaveMap);
	
	/// <summary>
	/// Adds AutoMapper with profiles from all Application Domain assemblies
	/// </summary>
	/// <param name="services">A collection of service descriptors</param>
	/// <param name="configAction">Configuration action to specify configuration of AutoMapper</param>
	/// <param name="catchDefaultProfiles">Should register default Profiles, written in default AutoMapper way</param>
	/// <returns>The <see cref="IServiceCollection"/> for chaining</returns>
	public static IServiceCollection AddAllMappings(
		this IServiceCollection services,
		Action<IServiceProvider,IMapperConfigurationExpression>? additionalConfigActionWithServiceProvider = null,
		bool catchDefaultProfiles = false)
	{
		return AddMappings(services, additionalConfigActionWithServiceProvider, catchDefaultProfiles, AppDomain.CurrentDomain.GetAssemblies());
	}

	/// <summary>
	/// Adds AutoMapper with profiles from specified assemblies
	/// </summary>
	/// <param name="services">A collection of service descriptors</param>
	/// <param name="assemblies">Assemblies to scan</param>
	/// <returns>The <see cref="IServiceCollection"/> for chaining</returns>
	public static IServiceCollection AddMappings(this IServiceCollection services, params Assembly[] assemblies)
	{
		return AddMappings(services, additionalConfigAction: null, false, assemblies);
	}

	/// <summary>
	/// Adds AutoMapper with profiles from specified assemblies with additional configuration
	/// </summary>
	/// <param name="services">A collection of service descriptors</param>
	/// <param name="additionalConfigAction">Additional config action which will be executed after all others</param>
	/// <param name="catchDefaultProfiles">Should register default Profiles, written in default AutoMapper way</param>
	/// <param name="assemblies">Assemblies to scan</param>
	/// <returns>The <see cref="IServiceCollection"/> for chaining</returns>
	public static IServiceCollection AddMappings(
		this IServiceCollection services,
		Action<IMapperConfigurationExpression>? additionalConfigAction = null,
		bool catchDefaultProfiles = false,
		params Assembly[] assemblies)
	{
		services.AddAutoMapper(cfg => {
			foreach (var assembly in assemblies) {
				var types = assembly.GetExportedTypes();

				var interfaces = types.Where(x => x.IsInterface == false)
					.Where(type => type.GetInterfaces()
						.Any(i => 
							(i.IsGenericType && MapWith.IsAssignableFrom(i.GetGenericTypeDefinition())) ||
							HaveMap.IsAssignableFrom(i)))
					.ToArray();
				if (interfaces.Length == 0) continue;
				cfg.AddProfile(new AssemblyMappingProfile(interfaces));

				if (catchDefaultProfiles)
					cfg.AddMaps(assembly);
			}

			additionalConfigAction?.Invoke(cfg);
		});
		
		return services;
	}
	
	/// <summary>
	/// Adds AutoMapper with profiles from specified assemblies with additional configuration
	/// </summary>
	/// <param name="services">A collection of service descriptors</param>
	/// <param name="additionalConfigActionWithServiceProvider">Additional config action which will be executed after all others</param>
	/// <param name="catchDefaultProfiles">Should register default Profiles, written in default AutoMapper way</param>
	/// <param name="assemblies">Assemblies to scan</param>
	/// <returns>The <see cref="IServiceCollection"/> for chaining</returns>
	public static IServiceCollection AddMappings(
		this IServiceCollection services,
		Action<IServiceProvider,IMapperConfigurationExpression>? additionalConfigActionWithServiceProvider = null,
		bool catchDefaultProfiles = false,
		params Assembly[] assemblies)
	{
		services.AddAutoMapper((provider, cfg) => {
			foreach (var assembly in assemblies) {
				var types = assembly.GetExportedTypes();

				var interfaces = types.Where(x => x.IsInterface == false)
					.Where(type => type.GetInterfaces()
						.Any(i => 
							(i.IsGenericType && MapWith.IsAssignableFrom(i.GetGenericTypeDefinition())) ||
							HaveMap.IsAssignableFrom(i)))
					.ToArray();
				if (interfaces.Length == 0) continue;
				cfg.AddProfile(new AssemblyMappingProfile(interfaces));

				if (catchDefaultProfiles)
					cfg.AddMaps(assembly);
			}

			additionalConfigActionWithServiceProvider?.Invoke(provider, cfg);
		}, assemblies);
		
		return services;
	}

	/// <summary>
	/// Adds AutoMapper with explicit configuration
	/// </summary>
	/// <param name="services">A collection of service descriptors</param>
	/// <param name="configAction">Configuration action</param>
	/// <returns>The <see cref="IServiceCollection"/> for chaining</returns>
	public static IServiceCollection AddMappings(this IServiceCollection services, Action<IMapperConfigurationExpression> configAction)
	{
		services.AddAutoMapper(configAction);
		return services;
	}
}