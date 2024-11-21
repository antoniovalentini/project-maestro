using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace ProjectMaestro.EngineCore;

public class WorkflowEngineDbContext : DbContext
{
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public WorkflowEngineDbContext(DbContextOptions<WorkflowEngineDbContext> options) : base(options) {}

    public DbSet<WorkflowDefinition> WorkflowDefinitions { get; set; }
    public DbSet<TaskDefinition> TaskDefinitions { get; set; }

    public DbSet<WorkflowInstance> WorkflowInstances { get; set; }
    public DbSet<TaskInstance> TaskInstances { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkflowDefinition>()
            .HasKey(u => u.Id);
        modelBuilder.Entity<TaskDefinition>()
            .HasKey(u => u.Name);

        modelBuilder.Entity<WorkflowInstance>()
            .HasKey(u => u.Id);
        modelBuilder.Entity<TaskInstance>()
            .HasKey(u => u.Id);
    }

    /// <summary>
    /// Feed database with a workflow sample
    /// </summary>
    public async Task SeedDefinitionsAsync()
    {
        if (!WorkflowDefinitions.Any())
        {
            var workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var orderWorkflowDefinition = await File.ReadAllTextAsync($"{workingDirectory}/WorkflowDefinitions/order-fulfillment.json");

            var wf1 = JsonSerializer.Deserialize<WorkflowDefinition>(orderWorkflowDefinition, _jsonOptions);
            if (wf1 == null) return;

            WorkflowDefinitions.AddRange(new List<WorkflowDefinition> { wf1 });
            await SaveChangesAsync();
        }
    }
}
