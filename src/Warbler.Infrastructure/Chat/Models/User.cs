namespace Warbler.Infrastructure.Chat.Models
{
    public class User : WarblerEntity
    {
        public string Name { get; set; }
        public int AvatarId { get; set; }

        public User(int id, string name, int avatarId) : base(id)
        {
            Name = name;
            AvatarId = avatarId;
        }
    }
}
