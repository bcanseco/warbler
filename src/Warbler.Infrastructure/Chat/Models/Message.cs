using System;
using System.Diagnostics;

namespace Warbler.Infrastructure.Chat.Models
{
    [DebuggerDisplay("Msg #{Id}: {Text}")]
    public class Message
    {
        public int Id { get; set; }
        public int ChannelId { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; }
        public DateTime SendDate { get; set; }

        public Channel Channel { get; set; }
        public User Sender { get; set; }
    }
}
