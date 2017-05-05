﻿using System.Collections.Generic;
using System.Threading.Tasks;
using GoogleApi.Entities.Places.Search.NearBy.Response;
using Warbler.Exceptions;
using Warbler.Interfaces;
using Warbler.Models;
using Warbler.Models.Enums;

namespace Warbler.Services
{
    /// <summary>
    ///   The business logic layer for manipulation of university data.
    /// </summary>
    public class UniversityService
    {
        private IUniversityRepository Repository { get; }

        public UniversityService(IUniversityRepository repository)
        {
            Repository = repository;
        }

        /// <summary>
        ///   Gets the university associated with the given
        ///   Google Places result or creates it if nonexistent.
        /// </summary>
        /// <param name="uni">The Google Places search result to use.</param>
        /// <returns>The related University object.</returns>
        public async Task<University> GetOrCreateAsync(NearByResult uni)
        {
            University university;
            try
            {
                university = await Repository.LookupAsync(uni.PlaceId);
            }
            catch (UniversityNotFoundException)
            {
                university = await Repository.CreateAsync(uni);
            }
            return university;
        }

        /// <summary>
        ///   Adds a user as a member to every channel in a university.
        /// </summary>
        /// <param name="user">The user to add.</param>
        /// <param name="university">The university whose channels will be added to.</param>
        /// <remarks>
        ///   This will create new rows in the Membership table (User x Channel).
        /// </remarks>
        public async Task JoinAsync(User user, University university)
        {
            var channels = university.Server.Channels;

            foreach (var channel in channels)
                channel.Memberships.Add(new Membership {User = user});

            await Repository.UpdateAsync(university);
        }

        /// <summary>
        ///   Gets a list of all universities and
        ///   their properties down to the message level.
        /// </summary>
        public async Task<List<University>> GetAllAsync()
            => await Repository.GetAllAsync(QueryDepth.Message);
    }
}