using NSubstitute;
using TaskManagement.Application.Abstractions.Authentication;
using TaskManagement.Application.Abstractions.Persistence;
using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Features.Tasks.Commands.AssignTask;
using TaskManagement.Application.Features.Tasks.Commands.UpdateTask;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Tests.Application;

public class AssignTaskTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly IIdentityService _identityService = Substitute.For<IIdentityService>();
    private readonly IRepository<TaskItem> _tasks = Substitute.For<IRepository<TaskItem>>();
    private readonly AssignTaskCommandHandler _handler;

    private readonly Guid _ownerId = Guid.NewGuid();

    public AssignTaskTests()
    {
        _unitOfWork.Tasks.Returns(_tasks);
        _currentUser.UserId.Returns(_ownerId);

        _handler = new AssignTaskCommandHandler(_unitOfWork, _currentUser, _identityService);
    }

    [Fact]
    public async Task Should_Throw_NotFound_When_Assignee_DoesNotExist()
    {
        // Arrange
        var task = new TaskItem { OwnerId = _ownerId, Title = "Design page" };
        _tasks.GetByIdAsync(task.Id, Arg.Any<CancellationToken>()).Returns(task);

        var unknownUserId = Guid.NewGuid();
        _identityService.UserExistsAsync(unknownUserId, Arg.Any<CancellationToken>())
                        .Returns(false);

        var command = new AssignTaskCommand(task.Id, unknownUserId);

        // Act + Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_LeaveTaskUnassigned_When_Assignee_DoesNotExist()
    {
        var task = new TaskItem { OwnerId = _ownerId, Title = "Design page" };
        _tasks.GetByIdAsync(task.Id, Arg.Any<CancellationToken>()).Returns(task);

        var unknownUserId = Guid.NewGuid();
        _identityService.UserExistsAsync(unknownUserId, Arg.Any<CancellationToken>())
                        .Returns(false);

        var command = new AssignTaskCommand(task.Id, unknownUserId);

        try { await _handler.Handle(command, CancellationToken.None); }
        catch (KeyNotFoundException) { }

        Assert.Null(task.AssignedToUserId);
    }

    [Fact]
    public async Task Should_AssignTask_When_Assignee_Exists()
    {
        // Arrange
        var task = new TaskItem { OwnerId = _ownerId, Title = "Design page" };
        _tasks.GetByIdAsync(task.Id, Arg.Any<CancellationToken>()).Returns(task);

        var assigneeId = Guid.NewGuid();
        _identityService.UserExistsAsync(assigneeId, Arg.Any<CancellationToken>())
                        .Returns(true);

        var command = new AssignTaskCommand(task.Id, assigneeId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(assigneeId, result.AssignedToUserId);
        Assert.Equal(assigneeId, task.AssignedToUserId);
    }

    [Fact]
    public async Task Should_Throw_Forbidden_When_User_DoesNotOwnTask_And_IsNotAdmin()
    {
        // Arrange
        var task = new TaskItem { OwnerId = Guid.NewGuid(), Title = "Someone else's task" };
        _tasks.GetByIdAsync(task.Id, Arg.Any<CancellationToken>()).Returns(task);

        var command = new AssignTaskCommand(task.Id, Guid.NewGuid());

        // Act + Assert
        await Assert.ThrowsAsync<ForbiddenAccessException>(
            () => _handler.Handle(command, CancellationToken.None));
    }
}