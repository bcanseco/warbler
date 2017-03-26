using System.Diagnostics;

namespace Warbler.Infrastructure.Chat.Models
{
    [DebuggerDisplay("{Id}: {Name} ({Lat}, {Lng})")]
    public class University : WarblerEntity
    {
        public string Name { get; }
        public float Lat { get; }
        public float Lng { get; }
        public Server Server { get; }

        public University(int id, string name, float lat, float lng, Server server)
            : base(id)
        {
            Name = name;
            Lat = lat;
            Lng = lng;
            Server = server;
        }
    }
}
