using System.Diagnostics;

namespace Warbler.Areas.Chat.Models
{
    [DebuggerDisplay("{Lat}, {Lng}")]
    public class Location
    {
        public float Lat { get; set; }
        public float Lng { get; set; }
    }
}
