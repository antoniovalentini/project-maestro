namespace ProjectMaestro.EngineCore;

using System.Text.Json.Serialization;

/* DEFINITION */
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(TaskDefinitionAutomated), "automated")]
[JsonDerivedType(typeof(TaskDefinitionHuman), "human")]
public class TaskDefinition
{
    public string Name { get; set; } = null!;
    public List<string> NextTasks { get; set; } = new();
    public bool? Root { get; set; }
}

public class TaskDefinitionAutomated : TaskDefinition;

public class TaskDefinitionHuman : TaskDefinition
{
    public string AssignedTo { get; set; } = null!;
}

/* INSTANCE */
public record TaskInstance(string Id, string Name, string WorkflowId)
{
    public string State { get; set; } = TaskState.Pending;
}

public static class TaskState
{
    public const string Pending = "Pending";
    public const string InProgress = "InProgress";
    public const string WaitingForInput = "WaitingForInput";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
}
