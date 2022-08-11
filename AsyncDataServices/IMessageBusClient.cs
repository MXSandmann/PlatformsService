using PlatformService.Dto;

namespace PlatformService.AsyncDataServices
{
    public interface IMessageBusClient : IDisposable
    {
        void PublishNewPlatform(PlatformPublishedDto platformPublishedDto);
    }
}