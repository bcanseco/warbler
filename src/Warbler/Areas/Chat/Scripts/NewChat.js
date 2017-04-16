/* global ko */
var Chat = Chat || {};                       

$(function () {
    Chat.server = new Chat.Server();
    Chat.data = ko.observable();                                    //Holds all the actual data that the viewmodel references
    Chat.viewModel = new Chat.ViewModel();
    ko.applyBindings(Chat.viewModel);
    ko.applyBindings(Chat.data);
})

Chat.Server = function () {
    var self = this;

    self.hub = $.connection.chatHub;

    self.hub.client.onConnection = function (universities) {
        var temp = [];

        //Server
        universities.forEach(function (uni) {

            var _server = new ServerModel(uni.ServerId, uni.ServerName);

            //Channel
            uni.Server.Channels.forEach(function (channel) {

                var _channel = new ChannelModel(channel.Id, channel.Name, channel.Description, channel.State);

                //Message
                channel.Messages.forEach(function (message) {

                    var _message = new MessageModel(message.Id, message.UserId, message.SendDate, message.Text);

                    _channel.channelMessages.push(_message);

                })

                _server.serverChannels.push(_channel);

            })

            temp.push(_server);

        })

        Chat.data(temp);
    }

    self.hub.client.messageReceived = function (messageObject) {

        var server = Chat.data().find(function (server) {
            return server.serverId() === messageObject.ServerId;
        });

        if (!server) {/*fuck shit up*/}                             //TODO: Make a errorHandler utility function
        
        var channel = server.serverChannels().find(function (channel) {
            return channel.channelId() = message.ChannelId;
        });

        if (!channel) {/*fuck shit up*/}

        channel.channelMessages.push(new MessageModel(messageObject.MessageId, messageObject.UserId, messageObject.SendDate, messageObject.Text));

    }

    self.hub.client.channelStateUpdate = function () {              //TODO: Fill in method

    }

    $.connection.hub.start()
        .done(/* literally NOTHING xdxdxd */)                       //TODO: Maybe use this to replace onConnection()
        .fail(function () {
            console.error("Error connecting to server. Please refresh the page and try again.");
        });

    $.connection.hub.disconnected(function () {
        console.error("Lost connection to server. Please refresh the page.");
    });
}

Chat.ViewModel = function () {
    var self = this;

    self.visibleServers = ko.observable(Chat.data());               //What servers the user has joined and can see
    self.currentServer = ko.observable();
    self.currentChannel = ko.observable();

    self.composedMessage = ko.observable();

    self.visibleChannels = ko.computed(function () {                //The channels that are a part of current server
        return self.currentServer().serverChannels();
    });
    
    self.visibleMessages = ko.computed(function () {
        return self.currentChannel().channelMessages();
    });

    self.setServer = function (item) {                              //Called on click of a server icon
        self.currentServer(item);
    }

    self.setChannel = function (item) {                             //Called on click of a channel
        self.currentChannel(item);
    }

    self.sendMessage = function (item) {
        Chat.server.hub.server.sendMessage(self.composedMessage());
        self.composedMessage(null);
    }
}

var ServerModel = function (id, name) {
    var serverId = id;
    var serverName = name;

    var serverChannels = ko.observableArray([]);
}

var ChannelModel = function (id, name, desc, state) {
    var channelId = id;
    var channelName = name;
    var channelDesc = desc;
    var channelState = state;

    var channelUsers = ko.obserableArray([]);
    var channelMessages = ko.observableArray([]);
}

var MessageModel = function (id, user, time, content) {
    var messageId = id;
    var userId = user;
    var timeSent = time;
    var messageContent = content;
}
