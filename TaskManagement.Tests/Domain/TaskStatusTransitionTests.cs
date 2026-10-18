using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Exceptions;

namespace TaskManagement.Tests.Domain;

public class TaskStatusTransitionTests
{

    [Fact]
    public void Should_MoveToInProgress_When_CurrentStatus_IsTodo()
    {
        // Arrange
        var task = new TaskItem();

        // Act
        task.ChangeStatus(TaskItemStatus.InProgress);

        // Assert
        Assert.Equal(TaskItemStatus.InProgress, task.Status);
    }

    [Fact]
    public void Should_Complete_Task_When_CurrentStatus_IsInProgress()
    {
        var task = new TaskItem();
        task.ChangeStatus(TaskItemStatus.InProgress);

        task.ChangeStatus(TaskItemStatus.Completed);

        Assert.Equal(TaskItemStatus.Completed, task.Status);
    }


    //Test an invalid Task status transition.
    [Fact]
    public void Should_Throw_When_CompletedTask_MovesBackToInProgress()
    {
        // Arrange
        var task = new TaskItem();
        task.ChangeStatus(TaskItemStatus.InProgress);
        task.ChangeStatus(TaskItemStatus.Completed);

        // Act + Assert
        Assert.Throws<InvalidStatusTransitionException>(
            () => task.ChangeStatus(TaskItemStatus.InProgress));
    }

    [Fact]
    public void Should_Throw_When_TodoTask_MovesDirectlyToCompleted()
    {
        var task = new TaskItem();

        Assert.Throws<InvalidStatusTransitionException>(
            () => task.ChangeStatus(TaskItemStatus.Completed));
    }

    [Fact]
    public void Should_KeepOriginalStatus_When_TransitionIsInvalid()
    {
        // Arrange
        var task = new TaskItem();

        // Act
        try { task.ChangeStatus(TaskItemStatus.Completed); } catch (InvalidStatusTransitionException) { }

        // Assert
        Assert.Equal(TaskItemStatus.Todo, task.Status);
    }
}