using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using ProjectMaestro.EngineCore;
using ProjectMaestro.TaskNotificationWorkerService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<TaskNotificationWorker>();
builder.Services.AddDbContext<WorkflowEngineDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEasyNetQ("host=localhost;username=user;password=password").UseSystemTextJson();
builder.Services.AddScoped<IWorkflowEngine, WorkflowEngine>();

var host = builder.Build();
host.Run();
