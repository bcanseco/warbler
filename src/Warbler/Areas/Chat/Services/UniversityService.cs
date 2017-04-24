using System.Collections.Generic;
using System.Threading.Tasks;
using GoogleApi.Entities.Places.Search.NearBy.Response;
using Warbler.Areas.Chat.Interfaces;
using Warbler.Areas.Chat.Models;
using Warbler.Areas.Chat.Models.Enums;
using Warbler.Areas.Chat.Repositories;
using Warbler.Identity.Data;

namespace Warbler.Areas.Chat.Services
{
    public class UniversityService
    {
        private IUniversityRepository UniversityRepository { get; set; }

        public UniversityService(WarblerDbContext context)
        {
            UniversityRepository = new AzureUniversityRepository(context);
        }

        public async Task AddUniversity(NearByResult uni)
        {
            await ((AzureUniversityRepository)UniversityRepository)
                .CreateUniversity(uni);
        }

        public async Task<List<University>> GetUniversities()
        {
            return await UniversityRepository.GetUniversitiesAsync(QueryDepth.Message);
        }
    }
}
