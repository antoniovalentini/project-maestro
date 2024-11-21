/*
 * This code is only used for local migrations.
 *
 * Some of the EF Core Tools commands (for example, the Migrations commands) require a derived DbContext instance to be created
 * at design time in order to gather details about the application's entity types and how they map to a database schema. In most
 * cases, it is desirable that the DbContext thereby created is configured in a similar way to how it would be configured at run time.
 */

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectMaestro.EngineCore;

internal class WorkflowEngineDbContextFactory : IDesignTimeDbContextFactory<WorkflowEngineDbContext>
{
    public WorkflowEngineDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = WorkflowEngineDbContextOptionsBuilder
            .GetNpgsqlWorkflowEngineDbContextOptionsBuilder();

        return new WorkflowEngineDbContext(optionsBuilder.Options);
    }
}

public class WorkflowEngineDbContextOptionsBuilder : DbContextOptionsBuilder<WorkflowEngineDbContext>
{
    private const string DesignTimeConnectionString = "Host=localhost;Port=5432;Database=myappdb;Username=myappuser;Password=StrongPassword123!;Pooling=true;";

    private WorkflowEngineDbContextOptionsBuilder() { }

    public static DbContextOptionsBuilder<WorkflowEngineDbContext> GetNpgsqlWorkflowEngineDbContextOptionsBuilder(
        string? connectionString = null,
        IServiceCollection? serviceCollection = null)
    {
        serviceCollection ??= new ServiceCollection();
        connectionString ??= DesignTimeConnectionString;

        serviceCollection.AddEntityFrameworkNpgsql();

        return new WorkflowEngineDbContextOptionsBuilder()
            .UseNpgsql(connectionString)
            .UseInternalServiceProvider(serviceCollection.BuildServiceProvider());
    }
}
