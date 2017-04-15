using System.Collections.Generic;
using System.Diagnostics;
using Warbler.Infrastructure.Chat.Models.Enums;

namespace Warbler.Infrastructure.Chat.Models
{
    [DebuggerDisplay("Server #{Id}: {Type}")]
    public class Server
    {
        public int Id { get; set; }
        public bool? IsAuthEnabled { get; set; }
        public ServerType Type { get; set; }
        public int UniversityId { get; set; }

        public University University { get; set; }
        public ICollection<Channel> Channels { get; set; }
    }
}
