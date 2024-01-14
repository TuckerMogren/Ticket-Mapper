using Infrastructure.DocumentService.Interfaces;
using TicketMapper.Domain.Interfaces.Repositories;

namespace Infrastructure.Persistence.Repositories;

public class GoogleDriveRepository(IGoogleDrive googleDrive) : IGoogleDriveRepository
{

}