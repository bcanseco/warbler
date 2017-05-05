using System;
using System.Linq;
using Warbler.Misc;
using Warbler.Models;
using Warbler.Models.Enums;

namespace Warbler.Repositories
{
    public class SqlSampleRepository
    {
        [Obsolete("This method is out-of-date with the current schema.")]
        public static void AddTestData(WarblerDbContext context)
        {
            if (!context.Users.Any() || context.Universities.Any())
            {
                return; // DB has been seeded, or it hasn't and can't be seeded without a user
            }

            var universities = new[]
            {
                new University{Name="Florida Institute of Technology",Lat=250.43f,Lng=-60.90f},
                new University{Name="Georgia Institute of Technology",Lat=1337.5f,Lng=9001f},
                new University{Name="California Institute of Technology",Lat=-42f,Lng=66.66f},
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
                new Channel{ServerId=1,Name="#politics",Description="Talk about politics",State=ChannelState.Archive,Type=ChannelType.Normal,LastUsed=DateTime.Today},
                new Channel{ServerId=2,Name="#general",Description="Talk about anything",State=ChannelState.Active,Type=ChannelType.Normal,LastUsed=DateTime.Now},
            };
            foreach (var c in channels)
            {
                context.Channels.Add(c);
            }
            context.SaveChanges();

            var user = context.Users.First();
            
            context.Memberships.Add(new Membership { UserId = user.Id, ChannelId = 1 });
            context.SaveChanges();

            var messages = new[]
            {
                new Message{ChannelId=1,UserId=user.Id,Text="Hello World!",SendDate=DateTime.Now},
                new Message{ChannelId=1,UserId=user.Id,Text="lmao cyah",SendDate=DateTime.Now},
            };
            foreach (var m in messages)
            {
                context.Messages.Add(m);
            }
            context.SaveChanges();
        }
    }
}
