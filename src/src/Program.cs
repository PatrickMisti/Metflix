using Metflix.Services.Akka;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// creates instance of IPublicHashingService that can be accessed by ASP.NET
builder.Services.AddSingleton<IActorBridge, HubService>();
// starts the IHostedService, which creates the ActorSystem and actors
builder.Services.AddMemoryCache();
builder.Services.AddHostedService(sp => (HubService)sp.GetRequiredService<IActorBridge>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
