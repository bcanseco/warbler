using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Warbler.Hubs;
using Warbler.Misc;
using Warbler.Models;
using Warbler.Repositories;

namespace Warbler.Services
{
    /// <summary>
    ///   A singleton used by the <see cref="ChatHub"/> for business logic.
    ///   TODO: Refactor and add tests for sanity
    /// </summary>
    public class ChatService : HubResource<ChatHub>
    {
        public ChatService(IHubContext<ChatHub> hubContext, ILogger<ChatService> logger)
            : base(hubContext, logger) => Logger = logger;

        /// <summary> Watches all channels with online users. (int : Channel.Id) </summary>
        private ConcurrentDictionary<int, Channel> ChannelStatus { get; } = new ConcurrentDictionary<int, Channel>();
        private ILogger<ChatService> Logger { get; }
        private ChannelService ChannelService { get; set; }
        private MessageService MessageService { get; set; }
        private UserService UserService { get; set; }
        private MembershipService MembershipService { get; set; }

        public ChatService With(WarblerDbContext context)
        {
            ChannelService = new ChannelService(new SqlChannelRepository(context));
            MessageService = new MessageService(new SqlMessageRepository(context));
            UserService = new UserService(new SqlUserRepository(context));
            MembershipService = new MembershipService(new SqlMembershipRepository(context));
            return this;
        }

        /// <summary>
        ///   Invoked from the hub on each connected client.
        /// </summary>
        public new async Task OnConnectedAsync(User user, string connectionId)
        {
            var onFirstDevice = await base.OnConnectedAsync(user, connectionId);
            var userChannels = await MembershipService.AllChannelsForAsync(user);

            var payloadBuilder = new Dictionary<Server, HashSet<Channel>>();

            // Iterate through channel objects (no Messages loaded on each).
            foreach (var channel in userChannels)
            {
                if (onFirstDevice)
                {
                    await UserService.SetOnlineAsync(user);

                    // Notify all other clients that a new user is joining.
                    await HubContext.Clients.Group($"{channel.Id}")
                        .InvokeAsync("onJoin", user, channel, channel.Server, channel.Server.University);
                }

                // Add the user to the SignalR group for this channel.
                await HubContext.Groups.AddAsync(connectionId, $"{channel.Id}");

                if (!ChannelStatus.TryGetValue(channel.Id, out var watchedChannel))
                {
                    // This channel is not being watched; we must fetch its messages
                    // TODO: Switch to Task => .Entry().Collection().LoadAsync()?
                    channel.Messages = await MessageService.LatestIn(channel, user.blockedUsers.ToList());
                    
                    // Start watching the channel
                    ChannelStatus.TryAdd(channel.Id, watchedChannel = channel);
                }

                // Regardless if the channel was previously watched or not, we need to update memberships
                watchedChannel.Memberships = await MembershipService.AllMembershipsForAsync(watchedChannel);

                // Add this channel to the dictionary under the server it belongs to
                if (!payloadBuilder.ContainsKey(watchedChannel.Server))
                {
                    payloadBuilder[watchedChannel.Server] = new HashSet<Channel>();
                }
                payloadBuilder[watchedChannel.Server].Add(watchedChannel);
            }
            
            // For each server, attach its corresponding channel collection and return its university
            var initialPayload = payloadBuilder.Keys.Select(server =>
            {
                server.Channels = payloadBuilder[server];
                return server.University;
            }).ToList();

            await HubContext.Clients.Client(connectionId)
                .InvokeAsync("receiveInitialPayload", initialPayload);
        }

        /// <summary>
        ///   Invoked from the hub on each disconnected client.
        /// </summary>
        /// <remarks>
        ///   We don't have to remove the connection ID from its 
        ///   HubContext group because SignalR does this automatically.
        /// </remarks>
        public new async Task OnDisconnectedAsync(User user, string connectionId)
        {
            var lastConnection = await base.OnDisconnectedAsync(user, connectionId);

            // The user is no longer connected on any device, remove from all channels
            if (lastConnection)
            {
                await UserService.SetOnlineAsync(user, false);
                var channels = await MembershipService.AllChannelsForAsync(user);

                foreach (var channel in channels)
                {
                    if (!ChannelStatus.TryGetValue(channel.Id, out var watchedChannel))
                        throw new Exception($"{channel.Name} was not being watched!");

                    // We need to update memberships to reflect the now offline user
                    watchedChannel.Memberships = await MembershipService.AllMembershipsForAsync(watchedChannel);

                    if (watchedChannel.Users.Any(u => u.IsOnline))
                    {
                        // Notify other online channel users that a user left
                        await HubContext.Clients.Group($"{watchedChannel.Id}")
                            .InvokeAsync("onLeave", user, channel, channel.Server, channel.Server.University);
                    }
                    else
                    {
                        // Nobody is online; stop watching channel
                        ChannelStatus.TryRemove(watchedChannel.Id, out _);
                        Logger.LogInformation($"No longer watching {watchedChannel.Name}.");
                    }
                }
            }
        }

        /// <summary>
        ///   Invoked from the hub when a user wants to send a message.
        /// </summary>
        /// <param name="user">The sender.</param>
        /// <param name="channel">The channel it was sent in.</param>
        /// <param name="text">The text to send.</param>
        public async Task OnMessageAsync(User user, Channel channel, string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            if (!ChannelStatus.TryGetValue(channel.Id, out channel))
                throw new Exception("User is not a member of this channel."); // JS injection

            var message = await MessageService.CreateAsync(text, user, channel);

            if (!ChannelStatus.TryGetValue(channel.Id, out var watchedChannel))
                throw new Exception($"{channel.Name} was not being watched!");

            watchedChannel.Messages.Add(message);
            await HubContext.Clients.Group($"{watchedChannel.Id}")
                .InvokeAsync("onMessageReceived", message, user, channel, channel.Server, channel.Server.University);
        }
    }
}
