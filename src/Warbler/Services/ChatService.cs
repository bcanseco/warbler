using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Warbler.Hubs;
using Warbler.Misc;
using Warbler.Models;
using Warbler.Repositories;

namespace Warbler.Services
{
    /// <summary>
    ///   Used by the <see cref="ChatHub"/> for business logic.
    ///   TODO: Refactor/optimize to use events and event listeners.
    /// </summary>
    public class ChatService : HubResource<ChatService, ChatHub>
    {
        /// <summary> Tracks all channels with online users. (int : Channel.Id) </summary>
        private ConcurrentDictionary<int, Channel> ChannelStatus { get; }
            = new ConcurrentDictionary<int, Channel>();

        private ChannelService ChannelService { get; set; }
        private MessageService MessageService { get; set; }
        private MembershipService MembershipService { get; set; }

        /// <summary>
        ///   Allows the hub to attach a database context to this service instance.
        /// </summary>
        public ChatService With(WarblerDbContext context)
        {
            ChannelService = ChannelService
                ?? new ChannelService(new SqlChannelRepository(context));
            MessageService = MessageService
                ?? new MessageService(new SqlMessageRepository(context));
            MembershipService = MembershipService
                ?? new MembershipService(new SqlMembershipRepository(context));

            return this;
        }

        /// <summary>
        ///   Invoked from the hub on each connected client.
        /// </summary>
        public new async Task OnConnected(User user, string connectionId)
        {
            var firstDevice = await base.OnConnected(user, connectionId);
            var userChannels = await MembershipService.AllChannelsForAsync(user);

            /* This is what the client is sent on connection; will contain all
               info necessary for the UI to initially populate the chat view. */
            var initialPayload = new List<University>();

            // Iterate through basic channel objects (only server/uni info)
            foreach (var channel in userChannels)
            {
                // Add the user to the SignalR group for this channel.
                await HubContext.Groups.Add(connectionId, $"{channel.Id}");

                if (firstDevice)
                {
                    // Notify all other clients that the user has joined.
                    HubContext.Clients.Group($"{channel.Id}", connectionId)
                        .onJoin(user, channel, channel.Server, channel.Server.University);
                }

                if (!ChannelStatus.TryGetValue(channel.Id, out Channel trackedChannel))
                {
                    // This channel is not being watched, we must fetch messages
                    channel.Messages = await MessageService.LatestIn(channel);

                    // Start watching the channel
                    ChannelStatus.TryAdd(channel.Id, trackedChannel = channel);
                }

                /* Regardless of the channel already being watched or not, we
                   must retrieve its memberships due to the newly joined member */
                trackedChannel.Memberships = await MembershipService
                    .AllForAsync(trackedChannel);
                
                trackedChannel.Users
                    .Single(u => u.Id == user.Id)
                    .IsOnline = true;

                initialPayload.Add(trackedChannel.Server.University);
            }

            HubContext.Clients.Client(connectionId)
                .receiveInitialPayload(initialPayload.Distinct());
        }

        /// <summary>
        ///   Invoked from the hub on each disconnected client.
        /// </summary>
        /// <remarks>
        ///   We don't have to remove the connection ID from its 
        ///   HubContext group because SignalR does this automatically.
        /// </remarks>
        public new async Task OnDisconnected(User user, string connectionId)
        {
            var lastConnection = await base.OnDisconnected(user, connectionId);

            // The user is no longer connected on any device, remove from all channels
            if (lastConnection)
            {
                var userChannels = await MembershipService.AllChannelsForAsync(user);

                foreach (var channel in userChannels)
                {
                    if (!ChannelStatus.TryGetValue(channel.Id, out Channel trackedChannel))
                        throw new Exception($"{channel.Name} was not being tracked!");

                    trackedChannel.Users.Single(u => u.Id == user.Id).IsOnline = false;
                    
                    if (trackedChannel.Users.Any(u => u.IsOnline))
                    {
                        // Notify other online channel users that a user left
                        HubContext.Clients.Group($"{trackedChannel.Id}")
                            .onLeave(user, channel, channel.Server, channel.Server.University);
                    }
                    else
                    {
                        // Nobody is online; save messages and stop watching
                        await ChannelService.UpdateAsync(trackedChannel);
                        ChannelStatus.TryRemove(trackedChannel.Id, out _);
                    }
                }
            }
        }
    }
}
