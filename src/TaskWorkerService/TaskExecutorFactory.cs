namespace ProjectMaestro.TaskWorkerService;

using EngineCore;

public class TaskExecutorFactory
{
    private readonly IEnumerable<ITaskExecutor> _taskExecutors;

    public TaskExecutorFactory(IEnumerable<ITaskExecutor> taskExecutors) => _taskExecutors = taskExecutors;

    public ITaskExecutor CreateTaskExecutor(TaskInstance taskInstance)
        => _taskExecutors.First(x => x.Id == taskInstance.Name);
}
