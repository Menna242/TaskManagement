using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace TaskManagement.Tests.Integration;

public class TaskWorkflowTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public TaskWorkflowTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private record AuthResult(Guid UserId, string AccessToken);
    private record ProjectResult(Guid Id, string Name);
    private record TaskResult(Guid Id, string Status, Guid? AssignedToUserId);
    private static async Task AssertOk(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(
            response.IsSuccessStatusCode,
            $"Expected success but got {(int)response.StatusCode}. Body: {body}");
    }

    [Fact]
    public async Task Should_CompleteFullWorkflow_From_Register_To_CompleteTask()
    {
        // ---------- 1) Register ----------
        var email = $"user-{Guid.NewGuid():N}@test.com";
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            fullName = "Test User",
            email,
            password = "Test1234_"
        });
        await AssertOk(registerResponse);
        var registered = await registerResponse.Content.ReadFromJsonAsync<AuthResult>();
        Assert.NotNull(registered);

        // ---------- 2) Login ----------
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email,
            password = "Test1234_"
        });
        await AssertOk(loginResponse);

        var login = await loginResponse.Content.ReadFromJsonAsync<AuthResult>();
        Assert.False(string.IsNullOrWhiteSpace(login!.AccessToken));

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", login.AccessToken);

        // ---------- 3) Create Project ----------
        var projectResponse = await _client.PostAsJsonAsync("/api/projects", new
        {
            name = "Website",
            description = "Integration test project"
        });
        await AssertOk(projectResponse);
        var project = await projectResponse.Content.ReadFromJsonAsync<ProjectResult>();
        Assert.Equal("Website", project!.Name);

        // ---------- 4) Create Task ----------
        var taskResponse = await _client.PostAsJsonAsync("/api/tasks", new
        {
            title = "Design page",
            description = "first task",
            ProjecgtId = project.Id

        });
        await AssertOk(taskResponse);
        var task = await taskResponse.Content.ReadFromJsonAsync<TaskResult>();
        Assert.Equal("Todo", task!.Status);

        // ---------- 5) Assign Task ----------
        var assignResponse = await _client.PutAsJsonAsync($"/api/tasks/{task.Id}/assign", new
        {
            userId = registered!.UserId
        });
        await AssertOk(assignResponse);
        var assigned = await assignResponse.Content.ReadFromJsonAsync<TaskResult>();
        Assert.Equal(registered.UserId, assigned!.AssignedToUserId);

        // ---------- 6) InProgress -> Completed ----------
        var inProgressResponse = await _client.PatchAsJsonAsync($"/api/tasks/{task.Id}/status", new
        {
            status = "InProgress"
        });
        await AssertOk(inProgressResponse);

        var completedResponse = await _client.PatchAsJsonAsync($"/api/tasks/{task.Id}/status", new
        {
            status = "Completed"
        });
        await AssertOk(completedResponse);
        var completed = await completedResponse.Content.ReadFromJsonAsync<TaskResult>();
        Assert.Equal("Completed", completed!.Status);
    }
}