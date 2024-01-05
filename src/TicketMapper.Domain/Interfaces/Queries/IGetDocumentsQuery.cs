using MediatR;

namespace TicketMapper.Domain.Interfaces.Queries;

public interface IGetDocumentsQuery<out T>: IRequest<T>
{
    string Path { get; }
}