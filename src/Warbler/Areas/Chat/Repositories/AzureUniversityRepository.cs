using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Warbler.Areas.Chat.Data;
using Warbler.Areas.Chat.Interfaces;
using Warbler.Areas.Chat.Models;
using Warbler.Areas.Chat.Models.Enums;

namespace Warbler.Areas.Chat.Repositories
{
    /// <summary>
    ///   Runs queries against the SQL database on Azure using Entity Framework.
    /// </summary>
    public class AzureUniversityRepository : IUniversityRepository
    {
        private ChatContext Database { get; }

        public AzureUniversityRepository(ChatContext context)
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
