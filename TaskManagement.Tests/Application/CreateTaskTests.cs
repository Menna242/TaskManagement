using NSubstitute;
using TaskManagement.Application.Abstractions.Authentication;
using TaskManagement.Application.Abstractions.Persistence;
using TaskManagement.Application.Features.Tasks.Commands.CreateTask;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Tests.Application;

public class CreateTaskTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly IRepository<ProjectEntity> _projects = Substitute.For<IRepository<ProjectEntity>>();
    private readonly IRepository<TaskItem> _tasks = Substitute.For<IRepository<TaskItem>>();
    private readonly CreateTaskCommandHandler _handler;

    private readonly Guid _userId = Guid.NewGuid();

    public CreateTaskTests()
    {
        _unitOfWork.Projects.Returns(_projects);
        _unitOfWork.Tasks.Returns(_tasks);
        _currentUser.UserId.Returns(_userId);

        _handler = new CreateTaskCommandHandler(_unitOfWork, _currentUser);
    }

    [Fact]
    public async Task Should_Throw_NotFound_When_Project_DoesNotExist()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        _projects.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
                 .Returns((ProjectEntity?)null);

        var command = new CreateTaskCommand("Design page", "desc", projectId, null);

        // Act + Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_NotSaveAnything_When_Project_DoesNotExist()
    {
        var projectId = Guid.NewGuid();
        _projects.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
                 .Returns((ProjectEntity?)null);

        var command = new CreateTaskCommand("Design page", "desc", projectId, null);

        try { await _handler.Handle(command, CancellationToken.None); }
        catch (KeyNotFoundException) { }

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_CreateTask_InTodoStatus_When_Project_Exists_And_UserOwnsIt()
    {
        // Arrange
        var project = new ProjectEntity { OwnerId = _userId, Name = "Website" };
        _projects.GetByIdAsync(project.Id, Arg.Any<CancellationToken>())
                 .Returns(project);

        var command = new CreateTaskCommand("Design page", "desc", project.Id, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Design page", result.Title);
        Assert.Equal("Todo", result.Status);
        Assert.Equal(project.Id, result.ProjectId);
    }
}