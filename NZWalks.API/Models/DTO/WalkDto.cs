using NZWalks.API.Models.Domain;

namespace NZWalks.API.Models.DTO
{
    public class WalkDto: Walk
    {
        public new RegionDto Region { get; set; }
        public new DifficultyDto Difficulty { get; set; }
    }
}
