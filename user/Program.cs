using Microsoft.AspNetCore.Http.HttpResults;
using user.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var userSubscriptions = new HashSet<UserSubscription>(UserSubscription.UserIdBoardKeywordComparer);
app.MapGet("/api/user/subscribe", (int userId, string board, string keyword) =>
    {
        userSubscriptions.Add(new UserSubscription
        {
            UserId = userId,
            Board = board,
            Keyword = keyword
        });

        return Results.Ok(userSubscriptions);
    })
    .WithOpenApi();

app.Run();
