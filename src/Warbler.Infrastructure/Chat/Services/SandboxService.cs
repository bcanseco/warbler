using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Warbler.Infrastructure.Chat.Interfaces;
using Warbler.Infrastructure.Chat.Models;
using Warbler.Infrastructure.Chat.Repositories;

namespace Warbler.Infrastructure.Chat.Services
{
    public class SandboxService
    {
        private IUniversityRepository UniversyRepository { get; }
        private List<University> Universities { get; set; }

        public SandboxService()
        {
            UniversyRepository = new SqlUniversityRepository(DbConnections.MySql);
        }

        public async Task GetUniversities()
        {
            try
            {
                Universities = Universities ?? await UniversyRepository.GetUniversities();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to get universities: {ex}.");
            }
        }
    }
}
