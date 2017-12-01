using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Warbler.Models
{
    [DebuggerDisplay("Msg #{Id}: {Text}")]
    public class Message
    {
        public int Id { get; set; }
        public int ChannelId { get; set; }
        public string UserId { get; set; }
        public string Text { get; set; }
        public DateTime SendDate { get; set; }

        [JsonIgnore]
        public Channel Channel { get; set; }
        //[JsonIgnore]
        public User Sender { get; set; }
    }
}
