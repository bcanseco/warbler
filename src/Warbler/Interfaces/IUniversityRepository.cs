﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleApi.Entities.Places.Search.NearBy.Response;
using Warbler.Models;
using Warbler.Models.Enums;

namespace Warbler.Interfaces
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
        /// <param name="templates">Templates to use for the university's channels.</param>
        /// <returns>The created university.</returns>
        Task<University> CreateAsync(NearByResult uni, IEnumerable<ChannelTemplate> templates);

        /// <summary>
        ///   Updates the properties of any universities in the database
        ///   that have been queried for and subsequently changed.
        /// </summary>
        Task SaveAsync();

        /// <summary>
        ///   Checks to see if a university for a given Google Place ID exists.
        /// </summary>
        /// <param name="placeId">The place ID to use for lookup. These are unique.</param>
        /// <returns>The university (at user level) if it exists.</returns>
        /// <exception cref="Exceptions.UniversityNotFoundException"></exception>
        Task<University> LookupAsync(string placeId);

        /// <summary>
        ///   Associates a university with the user that just claimed it.
        /// </summary>
        /// <param name="university">The claimed university.</param>
        /// <param name="claimeeId">The ID of the user claiming it.</param>
        Task ApplyClaimAsync(University university, string claimeeId);

        /// <summary>
        ///   Removes a university from the database.
        ///   Will also delete any navigation entities.
        /// </summary>
        /// <param name="university">The university to remove.</param>
        Task DeleteAsync(University university);

        /// <summary>
        ///   Gets an executable query for all universities in the database.
        /// </summary>
        /// <param name="depth">At what level navigation properties should be retrieved.</param>
        /// <example>
        ///   Using a query depth of <see cref="Server"/> will make each <see cref="University"/>
        ///   object in the query's result have a filled-in Server property. However, those 
        ///   Server objects will each have a null <see cref="Channel"/> collection property.
        /// </example>
        /// <exception cref="System.ArgumentException">
        ///   Thrown when <paramref name="depth"/>'s value is unknown.
        /// </exception>
        IQueryable<University> AllQueryable(QueryDepth depth = QueryDepth.University);
    }
}
