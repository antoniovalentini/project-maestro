namespace ProjectMaestro.TaskNotificationWorkerService;

using EngineCore;
using EasyNetQ;

public class TaskNotificationWorker : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IBus _bus;
    private readonly ILogger<TaskNotificationWorker> _logger;

    public TaskNotificationWorker(
        IServiceScopeFactory serviceScopeFactory,
        IBus bus,
        ILogger<TaskNotificationWorker> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _bus = bus;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[*] Starting TaskNotificationWorker");
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("[*] Waiting for messages.");

            WaitForMessages();

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }

        _logger.LogInformation("[*] Worker stopped.");
    }

    private void WaitForMessages()
    {
        _bus.PubSub.Subscribe<TaskNotification>("test", async (notification, _) =>
        {
            _logger.LogInformation("[*] Received TaskNotification message");

            using var scope = _serviceScopeFactory.CreateScope();
            var engine = scope.ServiceProvider.GetRequiredService<IWorkflowEngine>();
            await engine.ProcessNextTask(notification.TaskId);
        }, _ => {}, cancellationToken: default);
    }
}
