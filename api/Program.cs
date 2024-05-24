using infrastructure;
using infrastructure.Configs;
using infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Supabase;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
    optional: true,
    reloadOnChange: true);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var supabaseConfig = builder.Configuration.GetSection("SupabaseConfig").Get<SupabaseConfig>();
builder.Services.AddSupabase(supabaseConfig.Url, supabaseConfig.Key, new SupabaseOptions
{
    AutoConnectRealtime = true,
    AutoRefreshToken = true,
});
builder.Services.AddSingleton<ITelegramBotClient>(_ =>
{
    var telegramConfig = builder.Configuration.GetSection("TelegramConfig").Get<TelegramConfig>();
    return new TelegramBotClient(telegramConfig.Token);
});

// custom services
builder.Services.TryAddSingleton<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.TryAddSingleton<ISubscribedBoardRepository, SubscribedBoardRepository>();
builder.Services.AddTransient<IPttClient, PttClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();