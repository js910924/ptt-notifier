using infrastructure;
using infrastructure.Configs;
using infrastructure.Extensions;
using Supabase;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
    optional: true,
    reloadOnChange: true);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddControllers();

var supabaseConfig = builder.Configuration.GetSection("SupabaseConfig").Get<SupabaseConfig>();
builder.Services.AddSupabase(supabaseConfig.Url, supabaseConfig.Key, new SupabaseOptions
{
    AutoConnectRealtime = true,
    AutoRefreshToken = true,
});

var telegramConfig = builder.Configuration.GetSection("TelegramConfig").Get<TelegramConfig>();
builder.Services.AddTelegramBotClient(telegramConfig.Token);

// custom services
builder.Services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddTransient<ISubscribedBoardRepository, SubscribedBoardRepository>();
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