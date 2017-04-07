/* global ko */
var Chat = Chat || {};
var Data;                                                           //Holds all the actual data that the viewmodel references

$(function () {
    Chat.viewModel = new Chat.ViewModel();
    ko.applyBindings(Chat.viewModel);
})

Chat.ViewModel = function () {
    var self = this;
                                                                    //TODO (WH & BC): Get data on servers and channels from signalR hub
    self.visibleServers = ko.observable();                          //What servers the user has joined and can see                                                               
    self.currentServer = ko.observable();

    self.visibleChannels = ko.computed(function () {
        return self.currentServer().serverChannels();
    });
    self.currentChannel = ko.observable();

    self.messages = ko.computed(function () {
        return self.currentChannel().channelMessages();
    });

    self.setServer = function (item) {                              //Called on click of a server icon
        self.currentServer(item);
    }

    self.setChannel = function (item) {
        self.currentChannel(item);
    }
}

var ServerModel = function (id, name, uni) {
    var serverId = id;
    var serverName = name;
    var organization = uni;

    var serverChannels = ko.observableArray([]);
    //var serverUsers = ko.observableArray([]);                     //TODO (WH & BC): Determine if this is needed
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