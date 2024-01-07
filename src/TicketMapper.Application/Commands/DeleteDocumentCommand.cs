using MediatR;
using Microsoft.Extensions.Logging;
using TicketMapper.Domain.Interfaces.Commands;

namespace TicketMapper.Application.Commands
{
    public class DeleteDocumentCommand(string? path) : IDeleteDocumentCommand
    {
        public string Path { get; set; } = path ?? throw new ArgumentNullException(nameof(path));


        public class DeleteDocumentCommandHandler(ILogger<DeleteDocumentCommandHandler> logger)
            : IRequestHandler<IDeleteDocumentCommand, Unit>
        {
            public async Task<Unit> Handle(IDeleteDocumentCommand request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.Path))
                {
                    throw new ArgumentNullException(nameof(request));
                }

                try
                {   
                    await Task.Run(() => File.Delete(request.Path), cancellationToken);
                }
                catch (OperationCanceledException e)
                {
                    logger.LogInformation($"Operation was canceled: {e.Message}");
                }
                catch (Exception e)
                {
                    logger.LogInformation($"There was an exception, see logs: {e.Message}");
                }

                return Unit.Value;
            }
        }
    }
}

