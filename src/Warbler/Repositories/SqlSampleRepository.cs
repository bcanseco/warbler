using System;
using System.Linq;
using System.Threading.Tasks;
using Warbler.Misc;
using Warbler.Models;
using Warbler.Models.Enums;

namespace Warbler.Repositories
{
    /// <summary>
    ///   Fills database with example data for development purposes.
    /// </summary>
    public class SqlSampleRepository
    {
        private WarblerDbContext Context { get; }

        public SqlSampleRepository(WarblerDbContext context)
        {
            Context = context;
        }

        [Obsolete("This method is out-of-date with the current schema.")]
        public async Task AddTestData()
        {
            if (!Context.Users.Any() || Context.Universities.Any())
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
                await Context.Universities.AddAsync(u);
            }
            await Context.SaveChangesAsync();

            var servers = new[]
            {
                new Server{UniversityId=1,Type=ServerType.Public},
                new Server{UniversityId=2,Type=ServerType.Private},
                new Server{UniversityId=3,Type=ServerType.Private,IsAuthEnabled = true},
            };
            foreach (var s in servers)
            {
                await Context.Servers.AddAsync(s);
            }
            await Context.SaveChangesAsync();

            var channels = new[]
            {
                new Channel{ServerId=1,Name="#general",Description="Talk about anything",State=ChannelState.Active,Type=ChannelType.Normal,LastUsed=DateTime.Now},
                new Channel{ServerId=1,Name="#politics",Description="Talk about politics",State=ChannelState.Archive,Type=ChannelType.Normal,LastUsed=DateTime.Today},
                new Channel{ServerId=2,Name="#general",Description="Talk about anything",State=ChannelState.Active,Type=ChannelType.Normal,LastUsed=DateTime.Now},
            };
            foreach (var c in channels)
            {
                await Context.Channels.AddAsync(c);
            }
            await Context.SaveChangesAsync();

            var user = Context.Users.First();
            
            await Context.Memberships.AddAsync(new Membership { UserId = user.Id, ChannelId = 1 });
            await Context.SaveChangesAsync();

            var messages = new[]
            {
                new Message{ChannelId=1,UserId=user.Id,Text="Hello World!",SendDate=DateTime.Now},
                new Message{ChannelId=1,UserId=user.Id,Text="lmao cyah",SendDate=DateTime.Now},
            };
            foreach (var m in messages)
            {
                await Context.Messages.AddAsync(m);
            }
            await Context.SaveChangesAsync();
        }
    }
}
