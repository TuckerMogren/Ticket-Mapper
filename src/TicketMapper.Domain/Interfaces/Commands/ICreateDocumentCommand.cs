using MediatR;
using TicketMapper.Domain.DataModels;

namespace TicketMapper.Domain.Interfaces.Commands;

public interface ICreateDocumentCommand : IRequest
{ 
    TicketDetails TicketDetails { get; }
}