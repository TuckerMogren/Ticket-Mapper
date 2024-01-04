using MediatR;
using Microsoft.Extensions.Logging;
using TicketMapper.Application.Commands;
using TicketMapper.Domain.Notifications;

namespace TicketMapper.Application.IntegrationEventHandlers
{
    public class TimeToDeleteNotificationHandler(IMediator mediator, ILogger<TimeToDeleteNotificationHandler> logger) : INotificationHandler<TimeToDeleteNotification>
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        private readonly ILogger<TimeToDeleteNotificationHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task Handle(TimeToDeleteNotification notification, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.Send(new DeleteDocumentCommand(notification.FileName), cancellationToken);
            }
            catch (OperationCanceledException e)
            {
                _logger.LogInformation($"Operation was canceled {e.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
            }
        }
    }
}

