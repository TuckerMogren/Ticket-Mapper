using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TicketMapper.Application.Commands;
using TicketMapper.Application.Queries;
using TicketMapper.Domain.DataModels;
using TicketMapper.Domain.Notifications;

[ApiController]
[Route("api/v1/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly ILogger<DocumentController> _logger;
    private readonly IMediator _mediator;

    public DocumentController(ILogger<DocumentController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }


    [HttpGet("DownloadDocument")]
    [SwaggerOperation(Summary = "This service will generate and allow user to download file.")]
    [SwaggerResponse(200, "Returns the file as byte array", typeof(FileContentResult))]
    [SwaggerResponse(404, "File not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<IActionResult> DownloadFile([FromQuery, SwaggerParameter("Denotes the start", Required = true)] int startNumber, [FromQuery, SwaggerParameter("Denotes the end", Required = true)] int endNumber)
    {

        var ticketDetails = new TicketDetails
        {
            EndNumber = endNumber,
            StartNumber = startNumber,
            FileName = $"Output Files/{Guid.NewGuid().ToString()}.docx"
        };


        try
        {

            CreateDocumentCommand cmd = new(ticketDetails);
            await _mediator.Send(cmd);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while downloading the file.");
            return StatusCode(500, "Internal server error: Unable to generate document.");
        }

        try
        {
            GetDocumentQuery request = new(ticketDetails.FileName); // Initialize with required parameters
            byte[] fileBytes = await _mediator.Send(request);

            if (fileBytes == null)
            {
                _logger.LogWarning("File bytes are null");
                return NotFound("File not found.");
            }

            //Send a notification saying the file can be deleted, and then handle that to delete

            var notify = new TimeToDeleteNotification
            {
                Id = Guid.NewGuid(),
                FileName = ticketDetails.FileName,
                TimeSent = DateTime.UtcNow
            };


            await _mediator.Publish(notify);

            return new FileContentResult(fileBytes, "application/octet-stream")
            {
                FileDownloadName = "Tickets.docx"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while downloading the file.");
            return StatusCode(500, "Internal server error");
        }
    }
}
