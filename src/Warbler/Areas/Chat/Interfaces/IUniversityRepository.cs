using System.Collections.Generic;
using System.Threading.Tasks;
using GoogleApi.Entities.Places.Search.NearBy.Response;
using Warbler.Areas.Chat.Models;
using Warbler.Areas.Chat.Models.Enums;

namespace Warbler.Areas.Chat.Interfaces
{
    /// <summary>
    ///   Defines an API for university-based queries against a repository.
    /// </summary>
    public interface IUniversityRepository
    {
        /// <summary>
        ///   Adds a new university based on a Google Places Search result.
        /// </summary>
        /// <param name="uni">The result whose properties will be used for creation.</param>
        /// <returns>The created university.</returns>
        Task<University> CreateAsync(NearByResult uni);

        /// <summary>
        ///   Updates the properties of the university object in the database.
        /// </summary>
        /// <param name="university">The university to update.</param>
        Task UpdateAsync(University university);

        /// <summary>
        ///   Checks to see if a university for a given Google Place ID exists.
        /// </summary>
        /// <param name="placeId">The place ID to use for lookup. These are unique.</param>
        /// <returns>The university (at user level) if it exists.</returns>
        /// <exception cref="Exceptions.UniversityNotFoundException"></exception>
        Task<University> LookupAsync(string placeId);

        /// <summary>
        ///   Gets a list of all universities in the database.
        /// </summary>
        /// <param name="depth">At what level navigation properties should be retrieved.</param>
        /// <example>
        ///   Using a query depth of <see cref="Server"/> will make all <see cref="University"/>
        ///   objects have a non-null Server property. However, those servers will have a null
        ///   <see cref="ICollection{Channel}"/> property.
        /// </example>
        Task<List<University>> GetAllAsync(QueryDepth depth = QueryDepth.University);
    }
}
