using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Supabase;

namespace infrastructure.Extensions;

public static class SupabaseExtension
{
    public static void AddSupabase(this IServiceCollection services, string url, string key, SupabaseOptions options)
    {
        services.TryAddSingleton(_ => new Client(url, key, options));
    }
}