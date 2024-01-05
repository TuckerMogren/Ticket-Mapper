using Moq;
using Xunit;
using TicketMapper.Application.Queries;
using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Shouldly;
using TicketMapper.Domain.Interfaces.Queries;

namespace TicketMapper.Test.Application.Queries;

public class GetDocumentQueryTests
{
    [Fact]
    public async Task Handle_ReturnsCorrectByteArray()
    {
        // Arrange
        var filePath = "test.txt";
        var expectedContent = new byte[] { 0x1, 0x2, 0x3, 0x4 };
        var mockFileSystem = new Mock<IFileSystem>();
        var mockLogger = new Mock<ILogger<GetDocumentQuery.GetDocumentQueryHandler>>();
        
        mockFileSystem.Setup(f => f.File.ReadAllBytesAsync(filePath, CancellationToken.None)).ReturnsAsync((expectedContent));    
        
        var query = new GetDocumentQuery(filePath);
        var handler = new GetDocumentQuery.GetDocumentQueryHandler( mockLogger.Object);
        

        // Act
        var result = await handler.Handle(query, new CancellationToken());

        // Assert
        Assert.Equal(expectedContent, result);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("             ")]
    public async Task Handle_ArgumentNullExceptionThrownEmptyStringsPath_LogsInformation(string path)
    {
        // Arrange
        var mockFileSystem = new Mock<IFileSystem>();
        var mockLogger = new Mock<ILogger<GetDocumentQuery.GetDocumentQueryHandler>>();
        var handler = new GetDocumentQuery.GetDocumentQueryHandler( mockLogger.Object);
        var loggerMock = new Mock<ILogger<GetDocumentQuery.GetDocumentQueryHandler>>();
        var query = new GetDocumentQuery(path);

        // Act
        await handler.Handle(query, CancellationToken.None);

        // Assert
        mockLogger.Invocations.Count(i => i.Arguments[2].ToString()!.Contains("Value cannot be null")).ShouldBe(1);
    }

    [Fact]
    public async Task Handle_ArgumentNullException_logs()
    {
        // Arrange
        var mockFileSystem = new Mock<IFileSystem>();
        var mockLogger = new Mock<ILogger<GetDocumentQuery.GetDocumentQueryHandler>>();
        var handler = new GetDocumentQuery.GetDocumentQueryHandler( mockLogger.Object);
        var interfaceMock = new Mock<IGetDocumentsQuery<byte[]>>();
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(interfaceMock.Object, CancellationToken.None));
    }
}