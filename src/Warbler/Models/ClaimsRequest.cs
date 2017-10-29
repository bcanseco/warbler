using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Warbler.Models
{
    public class ClaimsRequest: IEquatable<ClaimsRequest>
    {
        public int Id { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String EmailAddress { get; set; }
        public String PositionTitle { get; set; }
        public String UniversityName { get; set; }
        public String UniversityWebsite { get; set; }

        public bool Equals(ClaimsRequest other)
        {
            throw new NotImplementedException();
        }
    }
}
