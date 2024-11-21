using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using ProjectMaestro.ApiGateway;
using ProjectMaestro.ApiGateway.Controllers;

namespace ApiGateway.IntegrationTests;

public class WorkflowControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public WorkflowControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Start_Workflow()
    {
        // Arrange
        var client = _factory.CreateClient();
        var startRequest = new StartWorkflowRequest("order_fulfillment_1", "1");
        var httpContent = new StringContent(JsonSerializer.Serialize(startRequest), Encoding.UTF8, MediaTypeNames.Application.Json);

        // Act
        var response = await client.PostAsync("api/workflow/start", httpContent);
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.True(response.IsSuccessStatusCode, content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
