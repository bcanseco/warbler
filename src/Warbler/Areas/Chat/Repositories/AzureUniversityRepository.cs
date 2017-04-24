using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleApi.Entities.Places.Search.NearBy.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Warbler.Areas.Chat.Interfaces;
using Warbler.Areas.Chat.Models;
using Warbler.Areas.Chat.Models.Enums;
using Warbler.Identity.Data;

namespace Warbler.Areas.Chat.Repositories
{
    /// <summary>
    ///   Runs queries against the SQL database on Azure using Entity Framework.
    /// </summary>
    public class AzureUniversityRepository : IUniversityRepository
    {
        private WarblerDbContext Database { get; }

        public AzureUniversityRepository(WarblerDbContext context)
        {
            Database = context;
        }
        
        public async Task<List<University>> GetUniversitiesAsync(QueryDepth depth)
        {
            switch (depth)
            {
                case QueryDepth.University: return await GetUniversitiesAtRootLevel().ToListAsync();
                case QueryDepth.Server:     return await GetUniversitiesAtServerLevel().ToListAsync();
                case QueryDepth.Channel:    return await GetUniversitiesAtChannelLevel().ToListAsync();
                case QueryDepth.User:       return await GetUniversitiesAtUserLevel().ToListAsync();
                case QueryDepth.Message:    return await GetUniversitiesAtMessageLevel().ToListAsync();
                default: throw new ArgumentException($"Unknown depth: {depth}.");
            }
        }

        public async Task CreateUniversity(NearByResult uni)
        {
            var university = new University
            {
                Name = uni.Name,
                PlaceId = uni.PlaceId,
                Address = uni.Vicinity,
                Lat = (float) uni.Geometry.Location.Latitude,
                Lng = (float) uni.Geometry.Location.Longitude,
                Server = new Server
                {
                    IsAuthEnabled = false,
                    Type = ServerType.Public,
                    Channels = new List<Channel>
                    {
                        new Channel
                        {
                            Name = "general",
                            Description = "Talk about anything.",
                            State = ChannelState.Active,
                            Type = ChannelType.Normal
                        },
                        new Channel
                        {
                            Name = "politics",
                            Description = "Talk about the election.",
                            State = ChannelState.Active,
                            Type = ChannelType.Normal
                        }
                    }
                }
            };

            await Database.Universities.AddAsync(university);
            await Database.SaveChangesAsync();
        }

        private IQueryable<University> GetUniversitiesAtRootLevel()
            => Database.Universities;

        private IIncludableQueryable<University, Server> GetUniversitiesAtServerLevel()
            => GetUniversitiesAtRootLevel()
                .Include(uni => uni.Server);

        private IIncludableQueryable<University, ICollection<Channel>> GetUniversitiesAtChannelLevel()
            => GetUniversitiesAtServerLevel()
                .ThenInclude(srv => srv.Channels);

        private IIncludableQueryable<University, User> GetUniversitiesAtUserLevel()
            => GetUniversitiesAtChannelLevel()
                .ThenInclude(ch => ch.Memberships)
                    .ThenInclude(m => m.User);

        private IIncludableQueryable<University, ICollection<Message>> GetUniversitiesAtMessageLevel()
            => GetUniversitiesAtUserLevel()
                .Include(uni => uni.Server) // need to backtrack to get messages branch
                    .ThenInclude(srv => srv.Channels)
                        .ThenInclude(ch => ch.Messages);
    }
}
