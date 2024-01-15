using TicketMapper.Infrastructure.DocumentService.Interfaces;
using TicketMapper.Domain.Interfaces.Repositories;

namespace TicketMapper.Infrastructure.Persistence.Repositories;

public class GoogleDriveRepository(IGoogleDrive googleDrive) : IGoogleDriveRepository
{

}