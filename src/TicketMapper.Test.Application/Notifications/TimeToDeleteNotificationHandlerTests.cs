using Xunit;
using Moq;
using MediatR;
using Microsoft.Extensions.Logging;
using TicketMapper.Application.Commands;
using TicketMapper.Domain.Notifications;
using TicketMapper.Application.IntegrationEventHandlers;
using Shouldly;

namespace TicketMapper.Test.Application.Notifications;
public class TimeToDeleteNotificationHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<TimeToDeleteNotificationHandler>> _loggerMock;
    private readonly TimeToDeleteNotificationHandler _handler;

    public TimeToDeleteNotificationHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<TimeToDeleteNotificationHandler>>();
        _handler = new TimeToDeleteNotificationHandler(_mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldLogInformation_WhenOperationCanceledExceptionThrown()
    {
        // Arrange
        var notification = new TimeToDeleteNotification { FileName = "testfile.txt" };
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteDocumentCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        await _handler.Handle(notification, new CancellationToken());

        _loggerMock.Invocations.Count(i => i.Arguments[2].ToString()!.Contains("Cancelled:")).ShouldBe(1);

    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenExceptionThrown()
    {
        // Arrange
        var notification = new TimeToDeleteNotification { FileName = "testfile.txt" };
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteDocumentCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        await _handler.Handle(notification, new CancellationToken());

        // Assert
        _loggerMock.Invocations.Count(i => i.Arguments[2].ToString()!.Contains("An error occurred")).ShouldBe(1);
    }
    
    [Fact]
    public async Task Handle_ShouldSendDeleteDocumentCommand_WhenCalled()
    {
        // Arrange
        var notification = new TimeToDeleteNotification
        {
            Id = Guid.NewGuid(),
            FileName = "testfile.txt",
            TimeSent = DateTime.UtcNow
        };

        var deleteDocumentCommand = new DeleteDocumentCommand("testfile.txt");

        _mediatorMock.Setup(s => s.Send(It.IsAny<DeleteDocumentCommand>(), It.IsAny<CancellationToken>()));

        // Act
        await _handler.Handle(notification, new CancellationToken());

        // Assert
        _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteDocumentCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(deleteDocumentCommand);
        Assert.Equal(notification.FileName, deleteDocumentCommand.Path);
    }

}
