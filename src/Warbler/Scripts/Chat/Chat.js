/* global ko */
var Chat = Chat || {};

$(function() {
  Chat.connection = new signalR.HubConnection("/ChatHub");
  Chat.dataModel = ko.observable({}); // Holds all the actual data that the viewmodel references
  Chat.server = new Chat.Server(); // Handles interaction with SignalR
  Chat.viewModel = new Chat.ViewModel(); // Handles UI and user called functions
  ko.applyBindings(Chat.viewModel);
});

Chat.Server = function() {
  const self = this;

  // Sent on connection
  Chat.connection.on("receiveInitialPayload", (payload) => {
    console.info("Initial payload", JSON.parse(JSON.stringify(payload)));
    // TODO: Error check payload
    // TODO: Maybe use predefined classes :thinking:
    payload
      .map(uni => {
        Object.assign(uni, uni.server); // merge uni with its server for convenience
        delete uni.server;

        uni.channels = ko.observableArray(uni.channels);
        return uni.channels();
      })
      .reduce((channels, currentChannelArray) => channels.concat(currentChannelArray), []) // flatten all channels
      .map(chan => {
        chan.lastUsed = ko.observable(chan.lastUsed);
        chan.state    = ko.observable(chan.state);
        chan.messages = ko.observableArray(chan.messages);
        chan.users    = ko.observableArray(chan.users);
        return chan.users();
      })
      .reduce((users, currentUserArray) => users.concat(currentUserArray), []) // flatten all users
      .map(user => {
        user.isOnline = ko.observable(user.isOnline);
      });

    Chat.dataModel().servers = ko.observableArray(payload);
    console.log("ayy2");
    Chat.viewModel.initializeView();
  });

  // Sent on updates to user's content
  Chat.connection.on("messageReceived", (messagePayload) => {
    messagePayload.forEach(function(uni) {
      var server = ko.utils.arrayFilter(Chat.dataModel().servers(), s => {
        console.log(uni, s);
        return uni["Id"] === s.id;
      });
      uni["server"]["channels"].forEach(function(chan) {
        var channel = ko.utils.arrayFilter(server.channels(), c => {
          console.log(chan, c);
          return chan["Id"] === c.id;
        });
        chan["messages"].forEach(m => {
          var message = {};
          message.id        = m["id"];
          message.channelId = m["channelId"];
          message.userId    = m["userId"];
          message.text      = m["text"];
          message.sendDate  = m["sendDate"];

          channel.messages.push(message);
        });
      });
    });
  });

  //Called on connection of another user
  Chat.connection.on("onJoin", (user, channel) => {
    Chat.dataModel()
      .servers()
      .find((server) => server.id == channel.serverId)
      .channels()
      .find((chan) => chan.id == channel.id)
      .users.push((u => {
        console.log(u);
        u.isOnline = ko.observable(u.isOnline);
        return u;
      })(user));

    console.log(Chat.dataModel().servers()[0].channels()[0].users());
  });

  //Called on disconnection of another user
  //TODO: MODIFY BELOW TO REMOVE USER INSTEAD OF ADD
  //Chat.connection.on("onLeave", (user, channel) => {
  //  Chat.dataModel()
  //    .servers()
  //    .find((server) => server.id == channel.serverId)
  //    .channels()
  //    .find((chan) => chan.id == channel.id)
  //    .users.push((u => {
  //      console.log(u);
  //      u.isOnline = ko.observable(u.isOnline);
  //      return u;
  //    })(user));

  //  console.log(Chat.dataModel().servers()[0].channels()[0].users());
  //});

  Chat.connection.start()
    .then(() => console.log("connected"))
    .catch(() => console.error("Error connecting to server. Please refresh the page and try again."));

  Chat.connection.onConnectionClosed(() => {
    console.error("Lost connection to server. Please refresh the page.");
  });
};

Chat.ViewModel = function () {
  const self = this;

  self.visibleServers = ko.observable();  // What servers the user has joined and can see
  self.currentServer = ko.observable();   // Server that user is currently viewing
  self.visibleChannels = ko.observable(); // What channels on the current server the user has joined and can see
  self.currentChannel = ko.observable();  // Channel that user is currently viewing

  self.initializeView = function () {     // Set an initial server and channel to view
    self.visibleServers(Chat.dataModel().servers());
    console.log('self.visibleServers()', self.visibleServers());
    if (self.visibleServers().length < 1) return;

    self.currentServer(self.visibleServers()[0]);

    self.visibleChannels(self.currentServer().channels());
    if (self.visibleChannels().length < 1) return;

    self.currentChannel(self.visibleChannels()[0]);
    console.log("ayy");
  };

  self.setServer = function(item) { // Called on click of a server icon
  };

  self.setChannel = function (item) { // Called on click of a channel
    self.currentChannel(item);
  };

  self.composedMessage = ko.observable();

  self.sendMessage = function(item) {    // Currently relies on messagePayload to actually add message to screen because it needs the global message id
    console.log(self.currentServer().id, self.currentChannel().id, self.composedMessage());
    Chat.connection.invoke("sendMessage", self.currentServer().id, self.currentChannel().id, self.composedMessage());
    self.composedMessage("");
  };

  // TODO: MAKE SEND BUTTON DISABLED WHEN COMPOSED MESSAGE IS EMPTY
};
