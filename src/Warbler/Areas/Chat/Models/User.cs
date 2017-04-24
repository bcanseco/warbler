﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json;
using Warbler.Areas.Chat.Models.Enums;

namespace Warbler.Areas.Chat.Models
{
    [DebuggerDisplay("User #{Id}: {UserName, nq}")]
    public class User : IdentityUser
    {
        public int AvatarId { get; set; }
        public UserFlag Flag { get; set; }

        [JsonIgnore]
        public ICollection<Membership> Memberships { get; set; }

        private List<Channel> _channels;

        public List<Channel> Channels
        {
            get { return _channels ?? (_channels = Memberships?.Select(m => m.Channel).ToList()); }
        }
    }
}