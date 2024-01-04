using System.Diagnostics.CodeAnalysis;
using MediatR;
using TicketMapper.Application.Commands;
using TicketMapper.Domain.Interfaces.Commands;

namespace TicketMapper.WebApi.Configs;

[ExcludeFromCodeCoverage]
public static class MediatRConfiguration
{

    public static void ConfigureMediatR(this IServiceCollection services)
    {
        services.AddMediatR(
            typeof(Program).Assembly,
            typeof(CreateDocumentCommand).Assembly,
            typeof(DeleteDocumentCommand).Assembly,
            typeof(IDeleteDocumentCommand).Assembly,
            typeof(ICreateDocumentCommand).Assembly);
    }
}