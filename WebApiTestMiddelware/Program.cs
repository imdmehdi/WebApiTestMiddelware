using WebApiTestMiddelware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IMessageWriter, LoggingMessageWriter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.Use(async (context, next) =>
{
    // Do work that can write to the Response.
    Console.WriteLine("One");
    await next.Invoke();
    // Do logging or other work that doesn't write to the Response.
    Console.WriteLine("One Out");

});
app.Use(async (context, next) =>
{
    // Do work that can write to the Response.
    Console.WriteLine("Two");

    await next.Invoke();
    // Do logging or other work that doesn't write to the Response.
    Console.WriteLine("Two Out");

});
app.UseMyMiddleware();
app.UseMyCustomMiddleware();
app.Use(async (context, next) =>
{
    // Do work that can write to the Response.
    Console.WriteLine("Three");

    await next.Invoke();
    // Do logging or other work that doesn't write to the Response.
    Console.WriteLine("Three Out");

});
app.Map("/map1", HandleMapTest1);//branching concept, see more we have lime mapwhen, usewhen

app.Map("/weatherforecast", HandleMapTest2);
//app.Run(async context =>
//{
//    Console.WriteLine("4");//after this no middleware will work as nothing is passed in next. Similar to last app.Run()
//});
app.Use(async (context, next) =>
{
    // Do work that can write to the Response.
    Console.WriteLine("5");

    await next(context);
    // Do logging or other work that doesn't write to the Response.
    Console.WriteLine("5 Out");

});
app.MapGet("/weatherforecast1", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

static void HandleMapTest1(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        await context.Response.WriteAsync("Map Test 1");
    });
}

static void HandleMapTest2(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        await context.Response.WriteAsync("Map Test 2");
    });
}
internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
