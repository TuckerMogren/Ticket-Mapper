using System.IO.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using TicketMapper.Domain.Interfaces.Queries;

namespace TicketMapper.Application.Queries
{
	public class GetDocumentQuery(string path) : IGetDocumentsQuery<byte[]>
    {
        public string Path { get; set; } = path ?? throw new ArgumentNullException(nameof(path));

        public class GetDocumentQueryHandler(IFileSystem fileSystem, ILogger<GetDocumentQueryHandler> logger)
            : IRequestHandler<IGetDocumentsQuery<byte[]>, byte[]>
        {
            public async Task<byte[]> Handle(IGetDocumentsQuery<byte[]> request, CancellationToken cancellationToken)
            {
                var file = Array.Empty<byte>();
                
                try
                {
                    if (string.IsNullOrWhiteSpace(request.Path))
                    {
                        throw new ArgumentNullException(nameof(request.Path));
                    }

                    logger.LogInformation($"Starting to get the document at path: {request.Path}");
                    file = await ReadFileToByteArray(request.Path);
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

            private async Task<byte[]> ReadFileToByteArray(string filePath)
            {
                return await fileSystem.File.ReadAllBytesAsync(filePath);
            }
        }

    }
}

