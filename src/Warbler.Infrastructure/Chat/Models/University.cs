namespace Warbler.Infrastructure.Chat.Models
{
    public class University : WarblerEntity
    {
        public string Name { get; set; }
        public float Lat { get; set; }
        public float Long { get; set; }

        public University(int id, string name, float lat, float lng)
            : base(id)
        {
            Name = name;
            Lat = lat;
            Long = lng;
        }
    }
}
