namespace ProjectMaestro.EngineCore;

/* DEFINITION */
public class WorkflowDefinition
{
    public string Id { get; set; } = null!;
    public string Version { get; set; } = null!;
    public List<TaskDefinition> Tasks { get; set; } = new();
}

/* INSTANCE */
public record WorkflowInstance(string Id, string DefinitionId, string Version)
{
    public string State { get; set; } = WorkflowState.InProgress;
    public string? CurrentTaskId { get; set; }
    public string? ErrorMessage { get; set; }
}

public static class WorkflowState
{
    public const string Pending = "Pending";
    public const string InProgress = "InProgress";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
}
