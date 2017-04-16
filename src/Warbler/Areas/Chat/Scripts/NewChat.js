/* global ko */
var Chat = Chat || {};
var Data = ko.observable();                                         //Holds all the actual data that the viewmodel references

$(function () {
    Chat.viewModel = new Chat.ViewModel();
    ko.applyBindings(Chat.viewModel);
})

Chat.Server = function () {
    var self = this;

    self.hub = $.connection.chatHub;                                //ChatHub.cs

    self.hub.client.onConnection = function (model) {
        var temp = [];
        model.Servers
    }

    self.hub.client.messageReceived = function (messageObject) {

        var server = Data.Servers().forEach(function (server) {
            return server.serverId = messageObject.serverId;
        });
        if (server.length !== 1) {/*fuck shit up*/}                 //TODO: Make a errorHandler utility function
        
        var channel = server[0].serverChannels.forEach(function (channel) {
            return channel.channelId = message.channelId;
        });
        if (channel.length !== 1) {/*fuck shit up*/}

        channel[0].channelMessages.push(message);

    }

    $.connection.hub.start()
        .done(/* do nothing for now */)                             //TODO: Maybe use this to replace onConnection()
        .fail(function () {
            console.error("Error connecting to server. Please refresh the page and try again.");
        });

    $.connection.hub.disconnected(function () {
        console.error("Lost connection to server. Please refresh the page.");
    });
}

Chat.ViewModel = function () {
    var self = this;

    self.visibleServers = ko.observable();                          //What servers the user has joined and can see
    self.currentServer = ko.observable();

    self.visibleChannels = ko.computed(function () {                //The channels that are a part of current server
        return self.currentServer().serverChannels();
    });
    self.currentChannel = ko.observable();

    self.messages = ko.computed(function () {
        return self.currentChannel().channelMessages();
    });

    self.setServer = function (item) {                              //Called on click of a server icon
        self.currentServer(item);
    }

    self.setChannel = function (item) {                             //Called on click of a channel
        self.currentChannel(item);
    }
}

var ServerModel = function (id, name, uni) {
    var serverId = id;
    var serverName = name;
    var organization = uni;

    var serverChannels = ko.observableArray([]);
}

var ChannelModel = function (server, id, name, desc, state) {
    var serverId = server;
    var channelId = id;
    var channelName = name;
    var channelDesc = desc;
    var channelState = state;

    var channelUsers = ko.obserableArray([]);
    var channelMessages = ko.observableArray([]);
}

var MessageModel = function (server, channel, id, user, time, content) {
    var serverId = server;
    var channelId = channel;
    var messageId = id,
    var userId = user;
    var timeSent = time;
    var messageContent = content;
}
