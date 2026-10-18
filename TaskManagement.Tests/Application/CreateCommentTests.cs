using NSubstitute;
using TaskManagement.Application.Abstractions.Authentication;
using TaskManagement.Application.Abstractions.Persistence;
using TaskManagement.Application.Features.Comments.Commands.CreateComment;
using TaskManagement.Application.Features.Comments.Commands.CreateCommentCommandHandler;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Tests.Application;

public class CreateCommentTests
{
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly IRepository<TaskItem> _tasks = Substitute.For<IRepository<TaskItem>>();
    private readonly IRepository<Comment> _comments = Substitute.For<IRepository<Comment>>();
    private readonly AddCommentCommandHandler _handler;

    private readonly Guid _userId = Guid.NewGuid();

    public CreateCommentTests()
    {
        _unitOfWork.Tasks.Returns(_tasks);
        _unitOfWork.Comments.Returns(_comments);
        _currentUser.UserId.Returns(_userId);

        _handler = new AddCommentCommandHandler(_unitOfWork, _currentUser);
    }

    [Fact]
    public async Task Should_Throw_NotFound_When_Task_DoesNotExist()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        _tasks.GetByIdAsync(taskId, Arg.Any<CancellationToken>())
              .Returns((TaskItem?)null);

        var command = new CreateCommentCommand(taskId, "Looks good");

        // Act + Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_NotSaveAnything_When_Task_DoesNotExist()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        _tasks.GetByIdAsync(taskId, Arg.Any<CancellationToken>())
              .Returns((TaskItem?)null);

        var command = new CreateCommentCommand(taskId, "Looks good");

        // Act
        try { await _handler.Handle(command, CancellationToken.None); }
        catch (KeyNotFoundException) { }

        // Assert
        _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_CreateComment_When_Task_Exists_And_UserOwnsIt()
    {
        // Arrange
        var task = new TaskItem { OwnerId = _userId, Title = "Design page" };
        _tasks.GetByIdAsync(task.Id, Arg.Any<CancellationToken>())
              .Returns(task);

        var command = new CreateCommentCommand(task.Id, "Looks good");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("Looks good", result.Content);
        Assert.Equal(task.Id, result.TaskItemId);
    }
}