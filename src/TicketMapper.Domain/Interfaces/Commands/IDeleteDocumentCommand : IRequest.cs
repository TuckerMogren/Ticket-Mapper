using MediatR;

namespace TicketMapper.Domain.Interfaces.Commands;

public interface IDeleteDocumentCommand : IRequest
{
    string Path { get;}
}