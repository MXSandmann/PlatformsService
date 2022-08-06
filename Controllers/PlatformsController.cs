using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dto;
using PlatformService.Models;
using PlatformService.SyncDataServices;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(IPlatformRepository repository, IMapper mapper, ICommandDataClient commandDataClient)
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("--> Getting Platforms...");

            var platformItems = _repository.GetAll();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }

        [HttpGet("{id:int}", Name = "GetPlatform")]
        public ActionResult<PlatformReadDto> GetPlatform(int id)
        {
            Console.WriteLine($"--> Getting a Platform with id {id}...");

            var platform = _repository.Get(id);

            if (platform == null)
                return NotFound();

            return Ok(_mapper.Map<PlatformReadDto>(platform));
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            Console.WriteLine($"--> Creating a new Platform with name {platformCreateDto.Name}...");

            var platformModel = _mapper.Map<Platform>(platformCreateDto);

            _repository.Create(platformModel);
            _repository.SaveChanges();

            var platformOut = _mapper.Map<PlatformReadDto>(platformModel);

            try
            {
                await _commandDataClient.SendPlatformToCommand(platformOut);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
            }     

            return CreatedAtRoute(nameof(GetPlatform), new { Id = platformOut.Id }, platformOut);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<int> DeletePlatform(int id)
        {
            Console.WriteLine($"--> Deleting a Platform with id {id}...");

            var deletedId = _repository.Delete(id);
            
            if (deletedId == null)
                return NotFound($"A plarform with id {id} was not forund");

            _repository.SaveChanges();

            return Ok(deletedId);
        }
    }
}
