using MediatR;
using Microsoft.Extensions.Logging;
using System.IO.Abstractions;
using TicketMapper.Domain.Interfaces.Queries;

namespace TicketMapper.Application.Queries
{
	public class GetDocumentQuery(string path) : IGetDocumentsQuery<byte[]>
    {
        public string Path { get; set; } = path ?? throw new ArgumentNullException(nameof(path));

        public class GetDocumentQueryHandler(ILogger<GetDocumentQueryHandler> logger, IFileSystem fileSystem)
            : IRequestHandler<IGetDocumentsQuery<byte[]>, byte[]>
        {
            public async Task<byte[]> Handle(IGetDocumentsQuery<byte[]> request, CancellationToken cancellationToken)
            {
                byte[]? file;
                try
                {
                    if (string.IsNullOrWhiteSpace(request.Path))
                    {
                        throw new ArgumentNullException(nameof(request));
                    }

                    logger.LogInformation($"Starting to get the document at path: {request.Path}");
                    file = await fileSystem.File.ReadAllBytesAsync(request.Path, cancellationToken);
                }
                catch (ArgumentNullException e)
                {
                    logger.LogCritical(message: e.Message);
                    throw;
                }
                catch (OperationCanceledException e)
                {
                    logger.LogCritical($"Cancellation token received: {e.Message}");
                    throw;
                }
                catch (Exception e)
                {
                    logger.LogCritical($"Issue reading file: {e.Message}");
                    throw;
                }

                return file;
            }
        }

    }
}

