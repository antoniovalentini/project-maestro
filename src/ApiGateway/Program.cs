using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using ProjectMaestro.EngineCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IWorkflowEngine, WorkflowEngine>();
builder.Services.AddDbContext<WorkflowEngineDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEasyNetQ("host=localhost;username=user;password=password").UseSystemTextJson();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WorkflowEngineDbContext>();
    context.Database.Migrate();
    await context.SeedDefinitionsAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UsePathBase("/api");

app.Run();

// Necessary for internal visibility in integration tests
namespace ProjectMaestro.ApiGateway
{
    public partial class Program { }
}
