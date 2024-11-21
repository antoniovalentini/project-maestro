using ProjectMaestro.EngineCore;

namespace ProjectMaestro.ApiGateway.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class WorkflowController : ControllerBase
{
    private readonly IWorkflowEngine _engine;
    private readonly ILogger<WorkflowController> _logger;

    public WorkflowController(IWorkflowEngine engine, ILogger<WorkflowController> logger)
    {
        _engine = engine;
        _logger = logger;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartWorkflow([FromBody] StartWorkflowRequest startWorkflowRequest)
    {
        if (string.IsNullOrWhiteSpace(startWorkflowRequest.Id))
        {
            return BadRequest(new { message = "The 'Name' parameter is required." });
        }

        var result = await _engine.Start(startWorkflowRequest.Id, startWorkflowRequest.Version);
        if (result.IsError(out var error, out var workflowInstance))
        {
            _logger.LogWarning("Unable to start workflow because {Error}", error);
            return Problem(detail: error, statusCode: 400);
        }

        if (workflowInstance.State is WorkflowState.Failed)
        {
            _logger.LogWarning("Unable to start workflow '{WorkflowId}' because {Error}", startWorkflowRequest.Id, workflowInstance.ErrorMessage);
            return Problem(detail: workflowInstance.ErrorMessage, statusCode: 400);
        }

        _logger.LogInformation("Started workflow '{WorkflowId}': {Id}", startWorkflowRequest.Id, workflowInstance.Id);
        return Created($"api/workflow/status/{workflowInstance.Id}", null);
    }

    // TODO: implement wf status logic
    [HttpGet("{id}/status")]
    public async Task<IActionResult> GetWorkflowStatus(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return BadRequest(new { message = "The 'id' parameter is required." });
        }

        await Task.CompletedTask;
        return Content(id);
    }
}

public record StartWorkflowRequest(string Id, string Version);
