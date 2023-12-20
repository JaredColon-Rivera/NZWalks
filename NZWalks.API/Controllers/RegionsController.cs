using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Text.Json;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;

        private readonly ILogger<RegionsController> logger;

        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository, IMapper mapper, ILogger<RegionsController> logger)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        // GET ALL REGIONS
        // https://localhost:1234/api/regions
        [HttpGet]
        //[Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetAll()
        {
            logger.LogInformation("GetAllRegions action method was invoke");

            logger.LogWarning("This is a warning log");

            logger.LogError("This is an error log");

            // Get data from database - domain models
            var regionsDomain = await regionRepository.GetAllAsync();

            var regionsDto = mapper.Map<List<RegionDto>>(regionsDomain);

            logger.LogInformation($"Finished get all regions requset with data: {JsonSerializer.Serialize(regionsDomain)}");

            // Return DTOS
            return Ok(regionsDto);
        }

        // GET SINGLE REGION (Get Region By ID)
        // GET: https://localhost:portnumber/api/regions/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        //[Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            // var region = dbContext.Regions.Find(id);
            // Get region Domain Model From database
            var regionDomain = await regionRepository.GetRegionByIdAsync(id);

            if(regionDomain == null)
            {
                return NotFound();
            }

            var regionDto = mapper.Map<RegionDto>(regionDomain);

            // Return DTO back to client
            return Ok(regionDto);
        }

        // POST To create a new region
        // POST: https://localhost:portnumber/api/regions
        [HttpPost]
        [ValidateModel]
        // [Authorize]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateRegion([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
           
            // Map or convert DTO to domain model
            var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

            // Use domain model to create region
            regionDomainModel = await regionRepository.CreateRegionAsync(regionDomainModel);

            // Map back to DTO
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            return CreatedAtAction(
                nameof(GetById),
                new { id = regionDomainModel.Id },
                regionDto
            );

        }

        // Update Region
        // PUT: https://localhost:portnumber/api/regions/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> UpdateRegion([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
           
            // var region = dbContext.Regions.Find(id);
            // Get region Domain Model From database
            // Map domain to dto
            var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

            // Check if region exists
            regionDomainModel = await regionRepository.UpdateRegionAsync(id, regionDomainModel);

            if (regionDomainModel == null)
            {
                return NotFound();
            }

            // Convert domain model to dto
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            return Ok(regionDto);
          
        }

        // Delete region
        // DELETE: https://localhost:portnumber/api/regions/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        //[Authorize(Roles = "Writer,Reader")]
        public async Task<IActionResult> DeleteRegion([FromRoute] Guid id)
        {
            var regionDomainModel = await regionRepository.DeleteRegionAsync(id);

            if(regionDomainModel == null)
            {
                return NotFound();
            }

            // Return deleted region back
            // Map domain model to dto
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            return Ok(regionDto);

        }
    }
}
