using System.Diagnostics.CodeAnalysis;
using MediatR;
using TicketMapper.Application.Commands;
using TicketMapper.Domain.Interfaces.Commands;

namespace TicketMapper.WebApi.Configs;

[ExcludeFromCodeCoverage]
public static class ConfigureDependencies
{
    public static void DependencyConfiguration(this IServiceCollection services)
    {
        services.AddTransient<IRequestHandler<DeleteDocumentCommand, Unit>, DeleteDocumentCommandHandler>(); 
        services.AddTransient<IRequestHandler<CreateDocumentCommand, Unit>, CreateDocumentCommand.CreateDocumentCommandHandler>(); 
    }
}