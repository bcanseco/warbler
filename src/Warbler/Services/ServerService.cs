using System.Threading.Tasks;
using Warbler.Interfaces;
using Warbler.Models;

namespace Warbler.Services
{
    /// <summary>
    ///   The business logic layer for manipulation of Server data.
    /// </summary>
    public class ServerService
    {
        private IServerRepository Repository { get; }

        public ServerService(IServerRepository repository)
            => Repository = repository;

        public async Task EnableAuthAsync(Server server)
            => await Repository.EnableAuthAsync(server);
    }
}
