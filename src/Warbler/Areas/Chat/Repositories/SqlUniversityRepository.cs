﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleApi.Entities.Places.Search.NearBy.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Warbler.Areas.Chat.Exceptions;
using Warbler.Areas.Chat.Interfaces;
using Warbler.Areas.Chat.Models;
using Warbler.Areas.Chat.Models.Enums;
using Warbler.Identity.Data;

namespace Warbler.Areas.Chat.Repositories
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
                    Type = ServerType.Public,
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
            await Context.SaveChangesAsync();
            return university;
        }

        public async Task UpdateAsync(University university)
        {
            Context.Universities.Update(university);
            await Context.SaveChangesAsync();
        }

        public async Task<University> LookupAsync(string placeId)
        {
            try
            {
                return await AtUserLevel()
                    .FirstAsync(u => u.PlaceId == placeId);
            }
            catch (InvalidOperationException)
            {
                // Give the service layer a more accurate exception
                throw new UniversityNotFoundException();
            }
        }

        public async Task<List<University>> GetAllAsync(QueryDepth depth)
        {
            switch (depth)
            {
                case QueryDepth.University: return await AtRootLevel().ToListAsync();
                case QueryDepth.Server:     return await AtServerLevel().ToListAsync();
                case QueryDepth.Channel:    return await AtChannelLevel().ToListAsync();
                case QueryDepth.User:       return await AtUserLevel().ToListAsync();
                case QueryDepth.Message:    return await AtMessageLevel().ToListAsync();
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