using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleApi.Entities.Places.Search.NearBy.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Warbler.Exceptions;
using Warbler.Interfaces;
using Warbler.Models;
using Warbler.Models.Enums;
using Warbler.Misc;

namespace Warbler.Repositories
{
    /// <summary>
    ///   Runs queries against the SQL database using Entity Framework.
    ///   Look at <see cref="IUniversityRepository"/> for docs.
    /// </summary>
    public class SqlUniversityRepository : IUniversityRepository
    {
        private WarblerDbContext Context { get; }

        public SqlUniversityRepository(WarblerDbContext context)
        {
            Context = context;
        }

        public async Task<University> CreateAsync(NearByResult uni)
        {
            var university = new University
            {
                Name = uni.Name,
                PlaceId = uni.PlaceId,
                Address = uni.Vicinity,
                Lat = (float)uni.Geometry.Location.Latitude,
                Lng = (float)uni.Geometry.Location.Longitude,
                Server = new Server
                {
                    IsAuthEnabled = false,
                    Channels = new List<Channel>
                    {
                        new Channel
                        {
                            Name = "general",
                            Description = "Talk about anything.",
                            State = ChannelState.Active,
                            Type = ChannelType.Normal,
                            Memberships = new List<Membership>()
                        },
                        new Channel
                        {
                            Name = "biology",
                            Description = "Talk about biological things.",
                            State = ChannelState.Active,
                            Type = ChannelType.Normal,
                            Memberships = new List<Membership>()
                        }
                    },
                }
            };

            await Context.Universities.AddAsync(university);
            await SaveAsync();
            return university;
        }

        public async Task SaveAsync()
        {
            await Context.SaveChangesAsync();
        }

        public async Task<University> LookupAsync(string placeId)
        {
            try
            {
                return await AtUserLevel().FirstAsync(u => u.PlaceId == placeId);
            }
            catch (InvalidOperationException ex)
            {
                // Give the service layer a more accurate exception
                throw new UniversityNotFoundException($"University not found matching place ID {placeId}", ex);
            }
        }

        public async Task ApplyClaimAsync(University university, string claimeeId)
        {
            university.ClaimedById = claimeeId;
            Context.Update(university);
            await SaveAsync();
        }

        public IQueryable<University> AllQueryable(QueryDepth depth)
        {
            switch (depth)
            {
                case QueryDepth.University: return AtRootLevel();
                case QueryDepth.Server:     return AtServerLevel();
                case QueryDepth.Channel:    return AtChannelLevel();
                case QueryDepth.User:       return AtUserLevel();
                case QueryDepth.Message:    return AtMessageLevel();
                default: throw new ArgumentException($"Unknown depth: {depth}.");
            }
        }

        private IQueryable<University> AtRootLevel()
            => Context.Universities;

        private IIncludableQueryable<University, Server> AtServerLevel()
            => AtRootLevel()
                .Include(uni => uni.Server);

        private IIncludableQueryable<University, ICollection<Channel>> AtChannelLevel()
            => AtServerLevel()
                .ThenInclude(srv => srv.Channels);

        private IIncludableQueryable<University, User> AtUserLevel()
            => AtChannelLevel()
                .ThenInclude(ch => ch.Memberships)
                    .ThenInclude(m => m.User);

        private IIncludableQueryable<University, ICollection<Message>> AtMessageLevel()
            => AtUserLevel()
                .Include(uni => uni.Server) // need to backtrack to get messages branch
                    .ThenInclude(srv => srv.Channels)
                        .ThenInclude(ch => ch.Messages);
    }
}
