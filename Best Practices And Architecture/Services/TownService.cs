namespace MyProject.Services.Services
{
    using MyPorject.Data;
    using MyProject.Services.Contracts.TownContracts;
    using MyProject.Services.DTO_S.TownDTOS;
    using System.Collections.Generic;
    using System.Linq;
    public class TownService : ITownService
    {
        private readonly SoftUniContext context;
        public TownService(SoftUniContext _context)
        {
            context = _context;
        }
        public IEnumerable<TownOutputDTO> GetAllTowns()
        {
            HashSet<TownOutputDTO> towns = context
                .Towns
                .Select(x => new TownOutputDTO()
                {
                    TownName = x.Name,
                })
                .ToHashSet();
            return towns;
        }

        public TownOutputDTO GetTownById(int id)
        {
            TownOutputDTO town = context
                .Towns
                .Where(x => x.TownId == id)
                .Select(x => new TownOutputDTO()
                {
                    TownName = x.Name,
                })
                .FirstOrDefault();

            return town;
        }

        public TownOutputDTO GetTwonByName(string townName)
        {
            TownOutputDTO town = context
                .Towns
                .Where(x => x.Name == townName)
                .Select(x => new TownOutputDTO()
                {
                    TownName = x.Name,
                })
                .FirstOrDefault();

            return town;
        }
    }
}
