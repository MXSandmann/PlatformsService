using PlatformService.Models;

namespace PlatformService.Data
{
    public interface IPlatformRepository
    {
        bool SaveChanges();

        IEnumerable<Platform> GetAll();
        Platform Get(int id);
        void Create(Platform platform);
        int Delete(int id);
        
    }
}
