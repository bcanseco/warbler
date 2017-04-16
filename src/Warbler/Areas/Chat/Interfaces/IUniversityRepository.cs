using System.Collections.Generic;
using System.Threading.Tasks;
using Warbler.Areas.Chat.Models;
using Warbler.Areas.Chat.Models.Enums;

namespace Warbler.Areas.Chat.Interfaces
{
    public interface IUniversityRepository
    {
        /// <summary>
        ///   Gets a list of all universities in the database.
        /// </summary>
        /// <param name="depth">At what level navigation properties should be retrieved.</param>
        /// <example>
        ///   Using a query depth of <see cref="Server"/> will make all <see cref="University"/>
        ///   objects have a non-null Server property. However, those servers will have a null
        ///   <see cref="ICollection{Channel}"/> property.
        /// </example>
        Task<List<University>> GetUniversitiesAsync(QueryDepth depth = QueryDepth.University);
    }
}
