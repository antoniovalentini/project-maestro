using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ProjectMaestro.EngineCore;

public interface IWorkflowEngine
{
    Task<Result<WorkflowInstance, string>> Start(string id, string version);
    Task ProcessNextTask(string currentTaskId);
}

public class WorkflowEngine : IWorkflowEngine
{
    private readonly WorkflowEngineDbContext _dbContext;
    private readonly IBus _bus;
    private readonly ILogger<WorkflowEngine> _logger;

    public WorkflowEngine(WorkflowEngineDbContext dbContext, IBus bus, ILogger<WorkflowEngine> logger)
    {
        _dbContext = dbContext;
        _bus = bus;
        _logger = logger;
    }

    public async Task<Result<WorkflowInstance, string>> Start(string id, string version)
    {
        // brief validation
        var workflowDefinition = await _dbContext.WorkflowDefinitions
            .Include(d => d.Tasks)
            .FirstOrDefaultAsync(w => w.Id == id && w.Version == version);
        if (workflowDefinition == null)
        {
            return "Workflow definition not found";
        }

        var taskDefinition = workflowDefinition.Tasks.First(x => x.Root == true);
        var taskId = Guid.NewGuid().ToString();
        var workflowInstance = new WorkflowInstance(Guid.NewGuid().ToString(), workflowDefinition.Id, workflowDefinition.Version)
        {
            CurrentTaskId = taskId,
        };
        var taskInstance = new TaskInstance(taskId, taskDefinition.Name, workflowInstance.Id);

        // store instance in database
        await _dbContext.WorkflowInstances.AddAsync(workflowInstance);
        await _dbContext.TaskInstances.AddAsync(taskInstance);
        await _dbContext.SaveChangesAsync();

        // enqueue first task
        await _bus.PubSub.PublishAsync(taskInstance);

        return workflowInstance;
    }

    public async Task ProcessNextTask(string currentTaskId)
    {
        // get task instance and set it to complete
        var task = await _dbContext.TaskInstances.FirstOrDefaultAsync(x => x.Id == currentTaskId);
        if (task is null)
        {
            _logger.LogWarning("No task instance found with ID: {Id}", currentTaskId);
            return;
        }
        task.State = TaskState.Completed;
        _logger.LogInformation("[*] Task {Id} set to COMPLETE", currentTaskId);

        // fetch related wf instance
        var workflowInstance = await _dbContext.WorkflowInstances.FirstOrDefaultAsync(x => x.Id == task.WorkflowId);
        if (workflowInstance is null)
        {
            _logger.LogWarning("No workflow instance found with ID: {Id}", task.WorkflowId);
            return;
        }

        // fetch wf definition and look for eventual next tasks. This logic only works for sequential tasks.
        // TODO: implement logic to handle independent tasks branches
        var workflowDefinition = await _dbContext.WorkflowDefinitions
            .Include(d => d.Tasks)
            .FirstAsync(w => w.Id == workflowInstance.DefinitionId && w.Version == workflowInstance.Version);
        var nextIds = workflowDefinition.Tasks.First(x => x.Name == task.Name).NextTasks;

        // complete the flow
        if (nextIds.Count == 0)
        {
            workflowInstance.State = WorkflowState.Completed;
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("[*] Workflow {Id} set to COMPLETE", workflowInstance.Id);
            return;
        }

        // process next tasks
        foreach (var nextId in nextIds)
        {
            var taskDefinition = workflowDefinition.Tasks.First(x => x.Name == nextId);
            var taskInstance = new TaskInstance(Guid.NewGuid().ToString(), taskDefinition.Name, workflowInstance.Id);

            workflowInstance.CurrentTaskId = taskInstance.Id;
            await _dbContext.TaskInstances.AddAsync(taskInstance);
            await _dbContext.SaveChangesAsync();

            await _bus.PubSub.PublishAsync(taskInstance);
            _logger.LogInformation("[*] Enqueue next task {Id}", taskInstance.Id);
        }
    }
}
