namespace Warbler.Infrastructure.Chat.Models
{
    public class Server : WarblerEntity
    {
        public bool IsAuthEnabled { get; set; }
        public ServerType Type { get; set; }

        public Server(int id, bool auth, int type) : base(id)
        {
            IsAuthEnabled = auth;
            Type = (ServerType) type;
        }
    }
}
