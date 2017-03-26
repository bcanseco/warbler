using System.Collections.Generic;
using System.Threading.Tasks;
using Warbler.Infrastructure.Chat.Models;

namespace Warbler.Infrastructure.Chat.Interfaces
{
    public interface IUniversityRepository
    {
        /// <summary>
        ///   Gets all universitites encapsulated as <see cref="University"/> objects.
        /// </summary>
        Task<List<University>> GetUniversities();
    }
}
