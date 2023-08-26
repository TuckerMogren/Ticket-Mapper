using System;
using MediatR;
using TicketMapper.Application.Commands;
using TicketMapper.Domain.Notifications;

namespace TicketMapper.Application.IntergrationEventHandlers
{
    public class TimeToDeleteNotificationHandler : INotificationHandler<TimeToDeleteNotification>
	{
        private readonly IMediator _mediator;


        public TimeToDeleteNotificationHandler(IMediator mediator)
		{
            _mediator = mediator ?? throw new ArgumentNullException();
        }

        public async Task Handle(TimeToDeleteNotification notification, CancellationToken cancellationToken)
        {
           await _mediator.Send(new DeleteDocumentCommand(notification.FileName));
        }
    }
}

