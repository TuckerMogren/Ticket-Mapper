using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using MediatR;
using TicketMapper.Application.Commands;
using TicketMapper.Application.IntegrationEventHandlers;
using TicketMapper.Application.Queries;
using TicketMapper.Domain.Interfaces.Commands;
using TicketMapper.Domain.Notifications;

namespace TicketMapper.WebApi.Configs;

[ExcludeFromCodeCoverage]
public static class ConfigureDependencies
{
    public static void DependencyConfiguration(this IServiceCollection services)
    {
        //Repositories
        services.AddTransient<IFileSystem, FileSystem>();
        //MediatR
        services.AddTransient<INotificationHandler<TimeToDeleteNotification>, TimeToDeleteNotificationHandler>();
        services.AddTransient<IRequestHandler<DeleteDocumentCommand, Unit>, DeleteDocumentCommand.DeleteDocumentCommandHandler>();
        services.AddTransient<IRequestHandler<CreateDocumentCommand, Unit>, CreateDocumentCommand.CreateDocumentCommandHandler>();
        services.AddTransient<IRequestHandler<GetDocumentQuery, byte[]>, GetDocumentQuery.GetDocumentQueryHandler>();
    }
}