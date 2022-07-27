using PlatformService.Dto;

namespace PlatformService.SyncDataServices
{
    public interface ICommandDataClient
    {
        Task SendPlatformToCommand(PlatformReadDto platform);
    }
}