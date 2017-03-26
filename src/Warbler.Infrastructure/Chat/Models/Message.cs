using System;

namespace Warbler.Infrastructure.Chat.Models
{
    public class Message : WarblerEntity
    {
        public int ChannelId { get; set; }
        public int UserId { get; set; }
        public int ServerId { get; set; }
        public string Text { get; set; }
        public DateTime SendDate { get; set; }

        public Message(
            int id,
            int channelId,
            int userId,
            int serverId,
            string text,
            DateTime sendDate) : base(id)
        {
            ChannelId = channelId;
            UserId = userId;
            ServerId = serverId;
            Text = text;
            SendDate = sendDate;
        }
    }
}