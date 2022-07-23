using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepository : IPlatformRepository
    {
        private readonly AppDbContext _context;

        public PlatformRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Create(Platform platform)
        {
            if (platform == null)
                throw new ArgumentNullException(nameof(platform));

            _context.Platforms.Add(platform);
        }


        public Platform Get(int id)
        {
            return _context.Platforms.FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Platform> GetAll()
        {
            return _context.Platforms.ToList();
        }

        public int Delete(int id)
        {
            var platformToDelete = _context.Platforms.FirstOrDefault(p => p.Id == id);
            _context.Platforms.Remove(platformToDelete);    

            return platformToDelete.Id;
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);  
        }
    }
}
