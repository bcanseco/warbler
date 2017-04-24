using System;
using System.Linq;
using GoogleApi.Entities.Places.Common.Enums;
using GoogleApi.Entities.Places.Search.NearBy.Response;

namespace Warbler.Areas.Chat.Extensions
{
    public static class NearbyResultExtensions
    {
        /// <summary>
        ///   Returns true if the <paramref name="result"/>'s Types 
        ///   collection includes the provided <paramref name="type"/>.
        /// </summary>
        public static bool Is(this NearByResult result, PlaceLocationType type)
            => result.Types.Contains(type);

        /// <summary>
        ///   Uses heuristics to determine if the given <paramref name="result"/>
        ///   may refer to a university building or other subsidiary.
        /// </summary>
        public static bool IsDepartment(this NearByResult result)
            => result.Name.StartsWith("College of", StringComparison.OrdinalIgnoreCase)
               || result.Name.IndexOf("Department", StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
