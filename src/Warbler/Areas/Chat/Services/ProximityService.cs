using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleApi;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Places.Search.NearBy.Request;
using GoogleApi.Entities.Places.Search.NearBy.Response;
using Warbler.Areas.Chat.Extensions;
using static GoogleApi.Entities.Places.Common.Enums.PlaceLocationType;

namespace Warbler.Areas.Chat.Services
{
    public static class ProximityService
    {
        /// <summary>
        ///    TODO: Store this elsewhere
        /// </summary>
        private const string GoogleApiKey = "AIzaSyCo9UePsjcg75Y2ZtJVsM33xJaWM6D1Qno";

        public static async Task<List<NearByResult>> UniversitiesNear(float lat, float lng)
        {
            var request = new PlacesNearBySearchRequest
            {
                Keyword = "university",
                Location = new Location(lat, lng),
                Radius = 25000,
                Key = GoogleApiKey
            };

            var response = await GooglePlaces.NearBySearch.QueryAsync(request);

            // Some results are mistagged; this filter reduces misses and false positives.
            return response.Results
                .Where(u => (u.Is(University) || u.Is(Library))
                            && !(!u.Is(University) && u.Is(School)))
                .Where(u => !u.IsDepartment())
                .Where(u => u.Rating > 0.0)
                .OrderBy(u => u.Name)
                .ToList();
        }
    }
}
