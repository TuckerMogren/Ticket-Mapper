using System;
using MediatR;

namespace TicketMapper.Domain.Notifications
{
	public class TimeToDeleteNotification : INotification
	{
		public Guid Id { get; set; }
        public required string FileName { get; set; }
		public DateTime TimeSent { get; set; }
	}
}

