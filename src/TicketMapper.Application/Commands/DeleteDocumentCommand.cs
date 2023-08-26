using System;
using MediatR;

namespace TicketMapper.Application.Commands
{
	public class DeleteDocumentCommand : IRequest
	{
        public string Path { get; set; }

        public DeleteDocumentCommand(string path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }
	}

    public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, Unit>
    {

        public DeleteDocumentCommandHandler()
        {
        }

        public async Task<Unit> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {

            await Task.Run(() => File.Delete(request.Path));

            return Unit.Value;
        }
    }
}

