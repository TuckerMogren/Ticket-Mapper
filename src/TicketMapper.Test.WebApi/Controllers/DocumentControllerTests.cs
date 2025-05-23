using Xunit;
using Moq;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TicketMapper.WebApi.Controllers;
using TicketMapper.Application.Commands;
using TicketMapper.Domain.DataModels;
using TicketMapper.Domain.Interfaces.Repositories;
using TicketMapper.Domain.Notifications;
using System.Reflection; // Required for anonymous type property access

namespace TicketMapper.Test.WebApi.Controllers
{
    public class DocumentControllerTests
    {
        private readonly Mock<ILogger<DocumentController>> _mockLogger;
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<IGoogleDriveRepository> _mockGoogleDriveRepository;
        private readonly DocumentController _controller;

        public DocumentControllerTests()
        {
            _mockLogger = new Mock<ILogger<DocumentController>>();
            _mockMediator = new Mock<IMediator>();
            _mockGoogleDriveRepository = new Mock<IGoogleDriveRepository>();
            _controller = new DocumentController(
                _mockLogger.Object,
                _mockMediator.Object,
                _mockGoogleDriveRepository.Object
            );
        }

        [Fact]
        public async Task DownloadFile_ShouldGenerateUploadAndNotify_ThenReturnOk()
        {
            // Arrange
            var startNumber = 1;
            var endNumber = 10;
            var expectedDriveFileId = "test_drive_id_123";
            string tempFilePath = string.Empty; // To store the path of the temporary file

            // Setup Mediator for CreateDocumentCommand
            // This command creates the file, so we need to simulate its creation for FileStream to work.
            _mockMediator
                .Setup(m => m.Send(It.IsAny<CreateDocumentCommand>(), It.IsAny<CancellationToken>()))
                .Returns(async (CreateDocumentCommand cmd, CancellationToken token) =>
                {
                    // Create a temporary dummy file that the controller will then try to read.
                    // The FileName property of TicketDetails in the command (cmd.TicketDetails.FileName)
                    // will be used by the controller.
                    tempFilePath = cmd.TicketDetails.FileName;
                    // Ensure the directory exists
                    var dir = Path.GetDirectoryName(tempFilePath);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    await File.WriteAllTextAsync(tempFilePath, "dummy content", token);
                    return Unit.Value;
                });

            // Setup GoogleDriveRepository UploadFileAsync
            _mockGoogleDriveRepository
                .Setup(r => r.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(expectedDriveFileId);

            // Setup Mediator for Publish (TimeToDeleteNotification)
            _mockMediator
                .Setup(m => m.Publish(It.IsAny<TimeToDeleteNotification>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DownloadFile(startNumber, endNumber, CancellationToken.None);

            // Assert
            // Verify Mediator.Send for CreateDocumentCommand was called
            _mockMediator.Verify(m => m.Send(It.Is<CreateDocumentCommand>(cmd => 
                cmd.TicketDetails.StartNumber == startNumber && 
                cmd.TicketDetails.EndNumber == endNumber), 
                It.IsAny<CancellationToken>()), Times.Once);

            // Verify GoogleDriveRepository.UploadFileAsync was called
            _mockGoogleDriveRepository.Verify(r => r.UploadFileAsync(
                It.Is<string>(fileName => fileName.EndsWith(".docx")), // Check if the filename is as expected
                It.IsAny<Stream>(), 
                It.Is<string>(contentType => contentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")), 
                Times.Once);

            // Verify Mediator.Publish for TimeToDeleteNotification was called
            _mockMediator.Verify(m => m.Publish(It.Is<TimeToDeleteNotification>(n => n.FileName == tempFilePath), It.IsAny<CancellationToken>()), Times.Once);

            // Assert that the result is an OkObjectResult
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            // Assert the properties of the anonymous type in OkObjectResult's Value
            var resultValue = okResult.Value;
            var messageProperty = resultValue.GetType().GetProperty("Message");
            var fileIdProperty = resultValue.GetType().GetProperty("GoogleDriveFileId");

            Assert.NotNull(messageProperty);
            Assert.Equal("File generated and uploaded to Google Drive successfully.", messageProperty.GetValue(resultValue));
            Assert.NotNull(fileIdProperty);
            Assert.Equal(expectedDriveFileId, fileIdProperty.GetValue(resultValue));

            // Cleanup: Delete the temporary file if it was created
            if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
                // Optionally, delete the directory if it's empty and was created by the test
                var dir = Path.GetDirectoryName(tempFilePath);
                if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir) && !Directory.EnumerateFileSystemEntries(dir).Any())
                {
                    Directory.Delete(dir);
                }
            }
        }
    }
}
