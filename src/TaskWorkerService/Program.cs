using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using ProjectMaestro.EngineCore;
using ProjectMaestro.TaskWorkerService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<TaskWorker>();
builder.Services.AddScoped<TaskExecutorFactory>();
builder.Services.AddScoped<ITaskExecutor, NotifyCustomerTaskExecutor>();
builder.Services.AddScoped<ITaskExecutor, ValidateOrderTaskExecutor>();
builder.Services.AddScoped<ITaskExecutor, ProcessPaymentTaskExecutor>();
builder.Services.AddScoped<ITaskExecutor, ShipOrderTaskExecutor>();
builder.Services.AddEasyNetQ("host=localhost;username=user;password=password").UseSystemTextJson();
builder.Services.AddDbContext<WorkflowEngineDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var host = builder.Build();
host.Run();
