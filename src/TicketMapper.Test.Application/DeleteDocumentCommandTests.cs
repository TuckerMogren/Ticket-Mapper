using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using TicketMapper.Application.Commands;
using TicketMapper.Domain.Interfaces.Commands;
using Xunit;
namespace TicketMapper.Test.Application;

public class DeleteDocumentCommandTests
{
        private readonly Mock<ILogger<DeleteDocumentCommandHandler>> _loggerMock;
        private readonly IRequestHandler<IDeleteDocumentCommand, Unit> _handler;

        public DeleteDocumentCommandTests()
        {
            _loggerMock = new Mock<ILogger<DeleteDocumentCommandHandler>>();
            _handler = new DeleteDocumentCommandHandler(_loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ValidPath_DeletesFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var command = new DeleteDocumentCommand(tempFile);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(File.Exists(tempFile)); // Ensure the file was deleted
        }

        [Fact]
        public async Task Handle_NullPathInCommand_ThrowsArgumentNullException()
        {
            // Arrange
            var moqCommand = new Mock<IDeleteDocumentCommand>();
            
            //Suppressing nulls to force it to be null for testing purposes. 
            moqCommand.Setup(c => c.Path).Returns((string)null!);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(moqCommand.Object, CancellationToken.None));

        }


        [Fact]
        public async Task Handle_OperationCancelled_LogsInformation()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var command = new DeleteDocumentCommand(tempFile);
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act
            await _handler.Handle(command, cts.Token);

            // Assert
            _loggerMock.Verify(
                log => log.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<OperationCanceledException>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ExceptionThrown_LogsInformation()
        {
            // Arrange
            var invalidPath = "/invalid/path";
            var command = new DeleteDocumentCommand(invalidPath);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _loggerMock.Verify(
                log => log.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!),
                Times.Once);
        }
}
