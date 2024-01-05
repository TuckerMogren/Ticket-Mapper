using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
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
            typeof(Program).Assembly);
    }
}