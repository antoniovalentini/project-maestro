using Microsoft.EntityFrameworkCore;

namespace ProjectMaestro.TaskWorkerService;

using EngineCore;
using EasyNetQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

public class TaskWorker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IBus _bus;
    private readonly ILogger<TaskWorker> _logger;

    public TaskWorker(IServiceScopeFactory serviceScopeFactory, IBus bus, ILogger<TaskWorker> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _bus = bus;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[*] Starting TaskWorker");

        while (!stoppingToken.IsCancellationRequested)
        {
            WaitForMessages();
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }

        _logger.LogInformation("[*] TaskWorker stopped.");
    }

    private void WaitForMessages()
    {
        _logger.LogInformation("[*] Waiting for messages.");

        _bus.PubSub.Subscribe<TaskInstance>("test", async (taskInstance, _) =>
        {
            _logger.LogInformation("[*] Received Task message");

            using var scope = _serviceScopeFactory.CreateScope();
            var taskExecutorFactory = scope.ServiceProvider.GetRequiredService<TaskExecutorFactory>();
            var dbContext = scope.ServiceProvider.GetRequiredService<WorkflowEngineDbContext>();

            // update task state

            /*
             * TODO
             * this delay is a dirty workaround to avoid rare cases where the message gets processed before the task instance is stored in the db,
             */
            await Task.Delay(500);


            var task = await dbContext.TaskInstances.FirstOrDefaultAsync(x => x.Id == taskInstance.Id);
            if (task == null)
            {
                _logger.LogWarning("Task '{ID}' not found", taskInstance.Id);
                return;
            }
            task.State = TaskState.InProgress;
            await dbContext.SaveChangesAsync();

            // execute task
            var executor = taskExecutorFactory.CreateTaskExecutor(taskInstance);
            await executor.ExecuteAsync(taskInstance);
        }, _ => {}, cancellationToken: default);
    }
}
