using Application.Services;
using Application.Services.Interfaces;
using Domain.Models;
using Domain.Services.Budget;
using Domain.Services.Delivery;
using Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Http;
using Persistence.Http.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IBudget, Budget>();
builder.Services.AddScoped<IDelivery, Delivery>();

builder.Services.AddTransient<IViaCep, ViaCep>();
builder.Services.AddHttpClient();

//Mappers
builder.Services.AddAutoMapper(typeof(OrderInputMapper));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(opt =>  
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Adding CORS Policies
builder.Services.AddCors( opt => {
    opt.AddPolicy("CorsPolicy", polcy => {
        polcy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:3000");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Adding CORS Policies
app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// setting all the migration to db
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try 
{
    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
    await Seed.SeedData(context);
} 
catch (System.Exception ex) 
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
}


app.Run();
