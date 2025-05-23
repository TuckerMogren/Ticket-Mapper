using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using MediatR;
using TicketMapper.Application.Commands;
using TicketMapper.Application.Queries;
using TicketMapper.Domain.Interfaces.Repositories; // Added for IGoogleDriveRepository
using TicketMapper.Infrastructure.Persistence.Repositories;

namespace TicketMapper.WebApi.Configs;

[ExcludeFromCodeCoverage]
public static class ConfigureDependencies
{
    public static void DependencyConfiguration(this IServiceCollection services)
    {
        services.AddTransient<IRequestHandler<DeleteDocumentCommand, Unit>, DeleteDocumentCommand.DeleteDocumentCommandHandler>(); 
        services.AddTransient<IRequestHandler<CreateDocumentCommand, Unit>, CreateDocumentCommand.CreateDocumentCommandHandler>();
        services.AddTransient<IRequestHandler<GetDocumentQuery,byte[]>, GetDocumentQuery.GetDocumentQueryHandler>();
        services.AddTransient<IGoogleDriveRepository, GoogleDriveRepository>();


    }
}