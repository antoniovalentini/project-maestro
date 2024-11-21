using EasyNetQ;
using ProjectMaestro.EngineCore;

namespace ProjectMaestro.TaskWorkerService;

public interface ITaskExecutor
{
    string Id { get; }
    Task ExecuteAsync(TaskInstance taskInstance);
}

public class ValidateOrderTaskExecutor : ITaskExecutor
{
    private readonly IBus _bus;
    private readonly ILogger<ValidateOrderTaskExecutor> _logger;

    public string Id => "validate_order";

    public ValidateOrderTaskExecutor(IBus bus, ILogger<ValidateOrderTaskExecutor> logger)
    {
        _logger = logger;
        _bus = bus;
    }

    public async Task ExecuteAsync(TaskInstance taskInstance)
    {
        // simulate task completion
        await Task.Delay(1000);
        _logger.LogInformation("Notify task {Name} completion {Id}", taskInstance.Name, taskInstance.Id);
        await _bus.PubSub.PublishAsync(new TaskNotification(taskInstance.Id));
    }
}

public class NotifyCustomerTaskExecutor : ITaskExecutor
{
    public string Id => "notify_customer";

    private readonly IBus _bus;
    private readonly ILogger<ValidateOrderTaskExecutor> _logger;

    public NotifyCustomerTaskExecutor(IBus bus, ILogger<ValidateOrderTaskExecutor> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task ExecuteAsync(TaskInstance taskInstance)
    {
        // simulate task completion
        await Task.Delay(1000);
        _logger.LogInformation("Notify task {Name} completion {Id}", taskInstance.Name, taskInstance.Id);
        await _bus.PubSub.PublishAsync(new TaskNotification(taskInstance.Id));
    }
}

public class ProcessPaymentTaskExecutor : ITaskExecutor
{
    public string Id => "process_payment";

    private readonly IBus _bus;
    private readonly ILogger<ValidateOrderTaskExecutor> _logger;

    public ProcessPaymentTaskExecutor(IBus bus, ILogger<ValidateOrderTaskExecutor> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task ExecuteAsync(TaskInstance taskInstance)
    {
        // simulate task completion
        await Task.Delay(1000);
        _logger.LogInformation("Notify task {Name} completion {Id}", taskInstance.Name, taskInstance.Id);
        await _bus.PubSub.PublishAsync(new TaskNotification(taskInstance.Id));
    }
}

public class ShipOrderTaskExecutor : ITaskExecutor
{
    public string Id => "ship_order";

    private readonly IBus _bus;
    private readonly ILogger<ValidateOrderTaskExecutor> _logger;

    public ShipOrderTaskExecutor(IBus bus, ILogger<ValidateOrderTaskExecutor> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task ExecuteAsync(TaskInstance taskInstance)
    {
        // simulate task completion
        await Task.Delay(1000);
        _logger.LogInformation("Notify task {Name} completion {Id}", taskInstance.Name, taskInstance.Id);
        await _bus.PubSub.PublishAsync(new TaskNotification(taskInstance.Id));
    }
}
