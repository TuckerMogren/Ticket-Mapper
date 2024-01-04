using System.Diagnostics.CodeAnalysis;
using MediatR;
using TicketMapper.Application.Commands;
using TicketMapper.Application.Queries;
using TicketMapper.Domain.Interfaces.Commands;

namespace TicketMapper.WebApi.Configs;

[ExcludeFromCodeCoverage]
public static class MediatRConfiguration
{

    public static void ConfigureMediatR(this IServiceCollection services)
    {
        services.AddMediatR(
            typeof(Program).Assembly,
            typeof(GetDocumentQuery).Assembly,
            typeof(DeleteDocumentCommand).Assembly,
            typeof(IDeleteDocumentCommand).Assembly,
            typeof(CreateDocumentCommand).Assembly);
    }
}