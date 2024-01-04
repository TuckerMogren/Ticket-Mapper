using MediatR;
using TicketMapper.Domain.Interfaces.Queries;

namespace TicketMapper.Application.Queries
{
	public class GetDocumentQuery(string path) : IGetDocumentsQuery<byte[]>
    {
        public string Path { get; set; } = path ?? throw new ArgumentNullException(nameof(path));

        public class GetDocumentQueryHandler : IRequestHandler<IGetDocumentsQuery<byte[]>, byte[]>
        {
            public async Task<byte[]> Handle(IGetDocumentsQuery<byte[]> request, CancellationToken cancellationToken)
            {
                return await ReadFileToByteArray(request.Path);
            }

            private async Task<byte[]> ReadFileToByteArray(string filePath)
            {
                return await File.ReadAllBytesAsync(filePath);
            }
        }

    }
}

