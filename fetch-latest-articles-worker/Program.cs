using fetch_latest_articles_worker;
using fetch_latest_articles_worker.Services;
using infrastructure;
using infrastructure.Configs;
using infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Supabase;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
    optional: true,
    reloadOnChange: true);

builder.Services.AddHostedService<Worker>();
builder.Services.AddHttpClient();

var supabaseConfig = builder.Configuration.GetSection("SupabaseConfig").Get<SupabaseConfig>();
builder.Services.AddSupabase(supabaseConfig.Url, supabaseConfig.Key, new SupabaseOptions
{
    AutoConnectRealtime = true,
    AutoRefreshToken = true,
});

builder.Services.AddTransient<FetchLatestArticlesService>();
builder.Services.TryAddSingleton<ISubscribedBoardRepository, SubscribedBoardRepository>();
builder.Services.AddTransient<IPttClient, PttClient>();

var host = builder.Build();
host.Run();