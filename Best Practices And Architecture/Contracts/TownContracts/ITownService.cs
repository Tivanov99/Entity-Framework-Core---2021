using MyProject.Services.DTO_S.TownDTOS;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProject.Services.Contracts.TownContracts
{
    public interface ITownService
    {
        TownOutputDTO GetTownById(int id);

        TownOutputDTO GetTwonByName(string townName);

        IEnumerable<TownOutputDTO> GetAllTowns();
    }
}
