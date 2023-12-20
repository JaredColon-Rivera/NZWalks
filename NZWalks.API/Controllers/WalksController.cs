using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Net;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkRepository walkRepository;

        public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
            this.mapper = mapper;
            this.walkRepository = walkRepository;
        }

        // Get Walks
        // GET: /api/walks?filterOn=Name&filterQuery=Track&sortBy=Name&isAscending=true&pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAllWalks(
            [FromQuery] string? filterOn, 
            [FromQuery] string? filterQuery, 
            [FromQuery] string? sortBy, 
            [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 1000)
        {
           
            var walksDomainModel = await walkRepository.GetAllWalksAsync(filterOn, filterQuery, sortBy, isAscending ?? true, pageNumber, pageSize);

            // Create an exception
            // throw new Exception("This is a new exception");

            // Map domain model to dto
            var walksDto = mapper.Map<List<WalkDto>>(walksDomainModel);

            return Ok(walksDto);

        }

        // Get walk by Id
        // GET: /api/walks/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetWalkById([FromRoute] Guid id)
        {
            var walksDomainModel = await walkRepository.GetWalkByIdAsync(id);

            if(walksDomainModel == null)
            {
                return NotFound();
            }

            var walksDto = mapper.Map<WalkDto>(walksDomainModel);

            return Ok(walksDto);
        }

        // Create Walk
        // Post: /api/walks
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateWalk([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
            // Map Dto to Domain model
            var walkDomainModel = mapper.Map<Walk>(addWalkRequestDto);

            walkDomainModel = await walkRepository.CreateWalkAsync(walkDomainModel);

            // Map domain model to dto
            var walkDto = mapper.Map<WalkDto>(walkDomainModel);

            return Ok(walkDto);
          
        }

        // Update Walk by Id
        // PUT: /api/walks/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateWalk([FromRoute] Guid id, UpdateWalkRequestDto updateWalkRequest)
        {
 
            // Map Dto to domain model
            var walkDomainModel = mapper.Map<Walk>(updateWalkRequest);

            walkDomainModel = await walkRepository.UpdateWalkAsync(id, walkDomainModel);

            if (walkDomainModel == null)
            {
                return NotFound();
            }

            // Map domain model to dto
            var walkDto = mapper.Map<Walk>(walkDomainModel);

            return Ok(walkDto);
           
        
        }

        // Delete walk by id
        // DELETE: /api/walks/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteWalk([FromRoute] Guid id)
        {
            var deletedWalkDomainModel = await walkRepository.DeleteWalkAsync(id);

            if(deletedWalkDomainModel == null)
            {
                return NotFound();
            }

            // Map domain model to dto
            var deletedWalkDto = mapper.Map<Walk>(deletedWalkDomainModel);

            return Ok(deletedWalkDto);
        }
    }
}
