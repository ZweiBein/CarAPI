using CarAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<CarContext>();

// Swagger stuff.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Populate a little bit of dummy data...
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var ctx = serviceProvider.GetRequiredService<CarContext>();

    if (!ctx.Cars.Any())
    {
        ctx.Cars.AddRange(
            new Car("Acura", 17, "Legend", "JH4KA7650RC007283"),
            new Car("Mercury", 17, "Capri", "1MEBP67D5BF617327"),
            new Car("Toyota", 17, "Camri", "4T1BF3EK5BU638805")
            );

        ctx.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
