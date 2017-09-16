/* global ko */
var Chat = Chat || {};                       

$(function () {
    $('#bodyContainer').removeClass('container');
    $('#bodyContainer').addClass('container-fluid');
    Chat.dataModel = ko.observable({});   //Holds all the actual data that the viewmodel references
    Chat.server = new Chat.Server();    //Handles interaction with SignalR
    Chat.viewModel = new Chat.ViewModel();  //Handles UI and user called functions
    ko.applyBindings(Chat.viewModel);
    //ko.applyBindings(Chat.dataModel);
})

Chat.Server = function () {
    var self = this;

    self.hub = $.connection.chatHub;
    Chat.dataModel().servers = ko.observableArray([]);

    //Sent on connection
    self.hub.client.receiveInitialPayload = function (payload) {
        console.info("Initial payload", payload);
        //TODO: Error check payload
        //TODO: Maybe use predefined classes :thinking:
        /* Probably going to need to change all of the initial payload code below to break it into functions that handle
           the creation of new servers/channels/etc but im still not sure how I want to handle that since it would end up
           with functions in loops calling functions inside deeper loops up to like 4 levels deep which just doesnt seem
           right to me */
        payload.forEach(function (u) {
            var uni = {};
            uni.address = u["Address"] || null;
            uni.id = u["Id"] || null;
            uni.lat = u["Lat"] || null;
            uni.lng = u["Lng"] || null;
            uni.name = u["Name"] || null;
            uni.placeId = u["PlaceId"] || null;

            uni.channels = ko.observableArray([]);
            u["Server"]["Channels"].forEach(function (c) {
                var chan = {};
                chan.description = c["Description"];
                chan.id = c["Id"];
                chan.name = c["Name"];
                chan.serverId = c["ServerId"];
                chan.type = c["Type"];
                chan.lastUsed = ko.observable(c["LastUsed"]);
                chan.state = ko.observable(c["State"]);

                chan.messages = ko.observableArray([]);
                c["Messages"].forEach(function (m) {
                    var message = {};
                    message.id = m["Id"];
                    message.channelId = m["ChannelId"];
                    message.userId = m["UserId"];
                    message.text = m["Text"];
                    message.sendDate = m["SendDate"];
                });

                chan.users = ko.observableArray([]);
                c["Users"].forEach(function (u) {
                    var user = {};
                    user.userName = u["UserName"];
                    user.avatarId = u["AvatarId"];
                    user.isOnline = ko.observable(u["IsOnline"]);
                    chan.users.push(user);
                });

                uni.channels.push(chan);
            });
            Chat.dataModel().servers.push(uni);
        });
        Chat.viewModel.initializeView();
    }

    //Sent on updates to user's content
    self.hub.client.messageReceived = function (messagePayload) {
        messagePayload.forEach(function (uni) {
            var server = ko.utils.arrayFilter(Chat.dataModel().servers(), function (s) {
                console.log(uni, s);
                return uni["Id"] == s.id;
            });
            uni["Server"]["Channels"].forEach(function (chan) {
                var channel = ko.utils.arrayFilter(server.channels(), function (c) {
                    console.log(chan, c);
                    return chan["Id"] == c.id;
                });
                chan["Messages"].forEach(function (mess) {
                    var message = {};
                    message.id = m["Id"];
                    message.channelId = m["ChannelId"];
                    message.userId = m["UserId"];
                    message.text = m["Text"];
                    message.sendDate = m["SendDate"];

                    channel.messages.push(message);
                })
            })
        })
    }

    $.connection.hub.start()
        .done(function () { console.log("connected"); })
        .fail(function () {
            console.error("Error connecting to server. Please refresh the page and try again.");
        });

    $.connection.hub.disconnected(function () {
        console.error("Lost connection to server. Please refresh the page.");
    });
}

Chat.ViewModel = function () {
    var self = this;

    self.visibleServers = ko.observable();  //What servers the user has joined and can see
    self.currentServer = ko.observable();   //Server that user is currently viewing
    self.visibleChannels = ko.observable(); //What channels on the current server the user has joined and can see
    self.currentChannel = ko.observable();  //Channel that user is currently viewing

    self.initializeView = function () {     //Set an initial server and channel to view
        self.visibleServers(Chat.dataModel().servers());
        console.log(self.visibleServers());
        if (self.visibleServers().length < 1) return;

        self.currentServer(self.visibleServers()[0]);

        self.visibleChannels(self.currentServer().channels());
        if (self.visibleChannels().length < 1) return;

        self.currentChannel(self.visibleChannels()[0]);
    };

    self.setServer = function (item) {                              //Called on click of a server icon
    }

    self.setChannel = function (item) {                             //Called on click of a channel
    }

    self.composedMessage = ko.observable();

    self.sendMessage = function (item) {    //Currently relies on messagePayload to actually add message to screen because it needs the global message id
        console.log(self.currentServer().id, self.currentChannel().id, self.composedMessage());
        Chat.server.hub.server.sendMessage(self.currentServer().id, self.currentChannel().id, self.composedMessage());
        self.composedMessage("");
    };

    //TODO: MAKE SEND BUTTON DISABLED WHEN COMPOSED MESSAGE IS EMPTY
}

//var ServerModel = function (id, name) {
//    var serverId = id;
//    var serverName = name;

//    var serverChannels = ko.observableArray([]);
//}

//var ChannelModel = function (id, name, desc, state) {
//    var channelId = id;
//    var channelName = name;
//    var channelDesc = desc;
//    var channelState = state;

//    var channelUsers = ko.obserableArray([]);
//    var channelMessages = ko.observableArray([]);
//}

//var MessageModel = function (id, user, time, content) {
//    var messageId = id;
//    var userId = user;
//    var timeSent = time;
//    var messageContent = content;
//}
