using MediatR;

namespace TicketMapper.Application.Queries
{
	public class GetDocumentQuery(string path) : IRequest<byte[]>
    {
        public string Path { get; set; } = path ?? throw new ArgumentNullException(nameof(path));

        public class GetDocumentQueryHandler : IRequestHandler<GetDocumentQuery, byte[]>
        {
            public async Task<byte[]> Handle(GetDocumentQuery request, CancellationToken cancellationToken)
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

