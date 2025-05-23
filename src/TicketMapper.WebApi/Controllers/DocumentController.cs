using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TicketMapper.Application.Commands;
// using TicketMapper.Application.Queries; // Removed as GetDocumentQuery is no longer used
using TicketMapper.Domain.DataModels;
using TicketMapper.Domain.Notifications;
using TicketMapper.Domain.Interfaces.Repositories; // Added for IGoogleDriveRepository
using System.IO; // Added for FileStream, Path

namespace TicketMapper.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DocumentController(ILogger<DocumentController> logger, IMediator mediator, IGoogleDriveRepository googleDriveRepository) : ControllerBase
{
    private readonly ILogger<DocumentController> _logger = logger ?? throw new ArgumentNullException((nameof(logger)));
    private readonly IMediator _mediatr = mediator ?? throw new ArgumentNullException((nameof(mediator)));
    private readonly IGoogleDriveRepository _googleDriveRepository = googleDriveRepository ?? throw new ArgumentNullException(nameof(googleDriveRepository));

    [HttpGet("DownloadDocument")]
    [SwaggerOperation(Summary = "This service will generate a document and upload it to Google Drive.")]
    [SwaggerResponse(200, "File uploaded successfully to Google Drive.", typeof(object))]
    [SwaggerResponse(400, "Operation was canceled by the client")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> DownloadFile([FromQuery, SwaggerParameter("Denotes the start", Required = true)] int startNumber, [FromQuery, SwaggerParameter("Denotes the end", Required = true)] int endNumber, CancellationToken cancellationToken)
    {
        var ticketDetails = new TicketDetails
        {
            EndNumber = endNumber,
            StartNumber = startNumber,
            FileName = $"Output Files/{Guid.NewGuid().ToString()}.docx" // Local path for the generated file
        };

        try
        {
            // Generate the document locally first
            CreateDocumentCommand cmd = new(ticketDetails);
            await _mediatr.Send(cmd, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Document generation was canceled.");
            return StatusCode(400, "Operation was canceled by the client during document generation.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while generating the document.");
            return StatusCode(500, "Internal server error: Unable to generate document.");
        }

        // Upload to Google Drive and then clean up local file
        string localFilePath = ticketDetails.FileName;
        string uploadedFileId;

        try
        {
            using (var fileStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read))
            {
                // Use Path.GetFileName to only send the file name, not the full local path, to Google Drive
                uploadedFileId = await _googleDriveRepository.UploadFileAsync(Path.GetFileName(localFilePath), fileStream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            }
            _logger.LogInformation($"File uploaded to Google Drive with ID: {uploadedFileId}");

            // Send a notification to delete the local file
            var notify = new TimeToDeleteNotification
            {
                Id = Guid.NewGuid(),
                FileName = localFilePath, // This is the local file path to be deleted
                TimeSent = DateTime.UtcNow
            };
            await _mediatr.Publish(notify, cancellationToken);

            return Ok(new { Message = "File generated and uploaded to Google Drive successfully.", GoogleDriveFileId = uploadedFileId });
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Google Drive upload or local file cleanup was canceled.");
            // Potentially attempt to clean up the local file if it exists and upload failed or was cancelled.
            // For now, just returning the status.
            return StatusCode(400, "Operation was canceled by the client during file upload or cleanup.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during Google Drive upload or local file handling after generation.");
            // Potentially attempt to clean up the local file if it exists.
            return StatusCode(500, "Internal server error: Failed to upload file to Google Drive or clean up local file.");
        }
    }
}