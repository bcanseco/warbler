using System.Diagnostics;

namespace Warbler.Areas.Chat.Models
{
    [DebuggerDisplay("Uni #{Id}: {Name, nq}")]
    public class University
    {
        public int Id { get; set; }
        public int ServerId { get; set; }
        public string Name { get; set; }
        public float Lat { get; set;  }
        public float Lng { get; set; }

        public Server Server { get; set; }
    }
}
