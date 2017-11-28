using System.Threading.Tasks;
using Warbler.Models;

namespace Warbler.Interfaces
{
    /// <summary>
    ///   Defines an API for working with servers.
    /// </summary>
    public interface IServerRepository
    {
        /// <summary>
        ///   Forces new users to authenticate through a third-party
        ///   identity provider before joining as defined via an
        ///   <see cref="AuthConfig"/>.
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        Task EnableAuthAsync(Server server);
    }
}
