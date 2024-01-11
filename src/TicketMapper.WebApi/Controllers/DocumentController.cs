using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TicketMapper.Application.Commands;
using TicketMapper.Application.Queries;
using TicketMapper.Domain.DataModels;
using TicketMapper.Domain.Notifications;

namespace TicketMapper.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DocumentController(ILogger<DocumentController> logger, IMediator mediator) : ControllerBase
{
    private readonly ILogger<DocumentController> _logger = logger ?? throw new ArgumentNullException((nameof(logger))); 
    private readonly IMediator _mediatr = mediator ?? throw new ArgumentNullException((nameof(mediator)));

    [HttpGet("DownloadDocument")]
    [SwaggerOperation(Summary = "This service will generate and allow user to download file.")]
    [SwaggerResponse(200, "Returns the file as byte array", typeof(FileContentResult))]
    [SwaggerResponse(404, "File not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> DownloadFile([FromQuery, SwaggerParameter("Denotes the start", Required = true)] int startNumber, [FromQuery, SwaggerParameter("Denotes the end", Required = true)] int endNumber, CancellationToken cancellationToken)
    {

        var ticketDetails = new TicketDetails
        {
            EndNumber = endNumber,
            StartNumber = startNumber,
            FileName = $"Output Files/{Guid.NewGuid()}.docx"
        };
        try
        {

            CreateDocumentCommand cmd = new(ticketDetails);
            await _mediatr.Send(cmd,cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Operation was canceled");
            return StatusCode(400, "Operation was canceled by the client");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while downloading the file.");
            return StatusCode(500, "Internal server error: Unable to generate document.");
        }

        try
        {
            GetDocumentQuery request = new(ticketDetails.FileName); // Initialize with required parameters
            var fileBytes = await _mediatr.Send(request,cancellationToken);

            //Send a notification saying the file can be deleted, and then handle that to delete

            var notify = new TimeToDeleteNotification
            {
                Id = Guid.NewGuid(),
                FileName = ticketDetails.FileName,
                TimeSent = DateTime.UtcNow
            };


            await _mediatr.Publish(notify, cancellationToken);

            return new FileContentResult(fileBytes, "application/octet-stream")
            {
                FileDownloadName = "Tickets.docx"
            };
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Operation was canceled");
            return StatusCode(400, "Operation was canceled by the client");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while downloading the file.");
            return StatusCode(500, "Internal server error");
        }
    }
}