using MediatR;
using Microsoft.Extensions.Logging;
using System.IO.Abstractions;
using TicketMapper.Domain.Interfaces.Commands;

namespace TicketMapper.Application.Commands
{
    public class DeleteDocumentCommand(string? path) : IDeleteDocumentCommand
    {
        public string Path { get; set; } = path ?? throw new ArgumentNullException(nameof(path));


        public class DeleteDocumentCommandHandler(ILogger<DeleteDocumentCommandHandler> logger, IFileSystem fileSystem)
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
                    logger.LogInformation($"Starting to delete document...");
                    await Task.Run(() => fileSystem.File.Delete(request.Path), cancellationToken);

                    var isFileStillAlive = fileSystem.File.Exists(request.Path);

                    if (!isFileStillAlive)
                    {
                        logger.LogInformation($"File has been removed.");
                    }
                    else
                    {
                        throw new InvalidOperationException($"Deletion failed. File still exists at {request.Path}.");
                    }


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

