using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Google.Apis.Drive.v3;
using TicketMapper.Infrastructure.DocumentService;
using TicketMapper.Infrastructure.DocumentService.Interfaces;
using TicketMapper.Infrastructure.Persistence.Repositories;
using MediatR;
using TicketMapper.Application.Commands;
using TicketMapper.Application.IntegrationEventHandlers;
using TicketMapper.Application.Queries;
using TicketMapper.Domain.Interfaces.Repositories;
using TicketMapper.Domain.Interfaces.Settings;
using TicketMapper.Domain.Notifications;

namespace TicketMapper.WebApi.Configs;

[ExcludeFromCodeCoverage]
public static class ConfigureDependencies
{
    public static void DependencyConfiguration(this IServiceCollection services, IApplicationSettings applicationSettings)
    {
        //Repositories
        services.AddTransient<IGoogleDrive, GoogleDrive>();
        services.AddTransient<IFileSystem, FileSystem>();
        services.AddTransient<IGoogleDriveRepository, GoogleDriveRepository>();
        //MediatR
        services.AddTransient<INotificationHandler<TimeToDeleteNotification>, TimeToDeleteNotificationHandler>();
        services.AddTransient<IRequestHandler<DeleteDocumentCommand, Unit>, DeleteDocumentCommand.DeleteDocumentCommandHandler>();
        services.AddTransient<IRequestHandler<CreateDocumentCommand, Unit>, CreateDocumentCommand.CreateDocumentCommandHandler>();
        services.AddTransient<IRequestHandler<GetDocumentQuery, byte[]>, GetDocumentQuery.GetDocumentQueryHandler>();
    }
}