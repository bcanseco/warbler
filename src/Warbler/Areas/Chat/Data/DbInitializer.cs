using System;
using System.Linq;
using Warbler.Areas.Chat.Models;
using Warbler.Areas.Chat.Models.Enums;

namespace Warbler.Areas.Chat.Data
{
    public class DbInitializer
    {
        /// <summary>
        ///   Initializes DB with test data.
        /// </summary>
        /// <param name="context">Context to use for inserting rows.</param>
        public static void Initialize(ChatContext context)
        {
            context.Database.EnsureCreated();
            
            if (context.Universities.Any())
            {
                return; // DB has been seeded
            }

            var universities = new[]
            {
                new University{ServerId=1,Name="Florida Institute of Technology",Lat=0.0f,Lng=1.0f},
                new University{ServerId=2,Name="Georgia Institute of Technology",Lat=1337f,Lng=9001f},
                new University{ServerId=3,Name="California Institute of Technology",Lat=42f,Lng=66f},
            };
            foreach (var u in universities)
            {
                context.Universities.Add(u);
            }
            context.SaveChanges();

            var servers = new[]
            {
                new Server{UniversityId=1,Type=ServerType.Public},
                new Server{UniversityId=2,Type=ServerType.Private},
                new Server{UniversityId=3,Type=ServerType.Private,IsAuthEnabled = true},
            };
            foreach (var s in servers)
            {
                context.Servers.Add(s);
            }
            context.SaveChanges();

            var channels = new[]
            {
                new Channel{ServerId=1,Name="#general",Description="Talk about anything",State=ChannelState.Active,Type=ChannelType.Normal,LastUsed=DateTime.Now},
                new Channel{ServerId=2,Name="#politics",Description="Talk about politics",State=ChannelState.Archive,Type=ChannelType.Normal,LastUsed=DateTime.Today},
                new Channel{ServerId=2,Name="#general",Description="Talk about anything",State=ChannelState.Active,Type=ChannelType.Normal,LastUsed=DateTime.Now},
            };
            foreach (var c in channels)
            {
                context.Channels.Add(c);
            }
            context.SaveChanges();

            var users = new[]
            {
                new User{AvatarId=1,Name="Greg"},
                new User{AvatarId=2,Name="Roger"},
            };
            foreach (var user in users)
            {
                context.Users.Add(user);
            }
            context.SaveChanges();

            var memberships = new[]
            {
                new Membership{UserId=1,ChannelId=1},
                new Membership{UserId=2,ChannelId=1},
            };
            foreach (var membership in memberships)
            {
                context.Memberships.Add(membership);
            }
            context.SaveChanges();

            var messages = new[]
            {
                new Message{ChannelId=1,UserId=1,Text="Hello World!",SendDate=DateTime.Now},
                new Message{ChannelId=1,UserId=2,Text="shut up lmao",SendDate=DateTime.Now},
                new Message{ChannelId=1,UserId=1,Text="reported",SendDate=DateTime.Now},
            };
            foreach (var m in messages)
            {
                context.Messages.Add(m);
            }
            context.SaveChanges();
        }
    }
}
