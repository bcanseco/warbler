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
    /// </summary>
    public class ChatService : HubResource<ChatService, ChatHub>
    {
        /// <summary> Watches all channels with online users. (int : Channel.Id) </summary>
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
            // Each service shares the same context to take advantage of EF caching
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
            var onFirstDevice = await base.OnConnected(user, connectionId);
            var userChannels = await MembershipService.AllChannelsForAsync(user);
            
            /* This is what the client is sent on connection; will contain all
             * info necessary for the UI to initially populate the chat view. */
            var initialPayload = new List<University>();

            /* Iterate through channel objects (no Messages loaded on each,
             * but Memberships, Server, and University info are up-to-date). */
            foreach (var channel in userChannels)
            {
                // Add the user to the SignalR group for this channel.
                await HubContext.Groups.Add(connectionId, $"{channel.Id}");

                if (onFirstDevice)
                {
                    // Notify all other clients that the user has joined.
                    HubContext.Clients.Group($"{channel.Id}", connectionId)
                        .onJoin(user, channel, channel.Server, channel.Server.University);
                }

                if (ChannelStatus.TryGetValue(channel.Id, out Channel watchedChannel))
                {
                    /* This channel is already being watched. Entity Framework automatically
                     * updated the watchedChannel object's Memberships collection when we
                     * queried for the same channel at the beginning of this method. */
                    Debug.Assert(watchedChannel.Users.Contains(user));
                }
                else
                {
                    // This channel is not being watched; we must fetch its messages
                    // TODO: Switch to Task => .Entry().Collection().LoadAsync()
                    channel.Messages = await MessageService.LatestIn(channel);

                    // Start watching the channel
                    ChannelStatus.TryAdd(channel.Id, watchedChannel = channel);
                }
                
                watchedChannel.Users.Single(u => u.Id == user.Id).IsOnline = true;
                initialPayload.Add(watchedChannel.Server.University);
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
                // Since user was being tracked, EF autofilled Memberships property
                Debug.Assert(user.Channels?.Any() ?? false);

                foreach (var channel in user.Channels)
                {
                    if (!ChannelStatus.TryGetValue(channel.Id, out Channel watchedChannel))
                        throw new Exception($"{channel.Name} was not being watched!");

                    watchedChannel.Users.Single(u => u.Id == user.Id).IsOnline = false;
                    
                    if (watchedChannel.Users.Any(u => u.IsOnline))
                    {
                        // Notify other online channel users that a user left
                        HubContext.Clients.Group($"{watchedChannel.Id}")
                            .onLeave(user, channel, channel.Server, channel.Server.University);
                    }
                    else
                    {
                        // Nobody is online; save messages and stop watching
                        // TODO: Verify if this is right (Context(.Channels?).Update)?
                        await ChannelService.UpdateAsync(watchedChannel);
                        ChannelStatus.TryRemove(watchedChannel.Id, out _);
                    }
                }
            }
        }
    }
}
