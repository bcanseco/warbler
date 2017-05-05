var Sandbox = Sandbox || {};

$(function () {
    Sandbox.server = new Sandbox.Server();
    Sandbox.viewModel = new Sandbox.ViewModel();

    ko.bindingHandlers.returnAction = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            // Detect enter press in input fields - from http://stackoverflow.com/a/14293100
            var value = ko.utils.unwrapObservable(valueAccessor());
            $(element).keydown(function (e) {
                if (e.which === 13) {
                    value(viewModel);
                }
            });
        }
    };

    ko.applyBindings(Sandbox.viewModel);
});

Sandbox.ViewModel = function () {
    this.chatData = ko.observable(new Sandbox.ChatViewModel());
};

Sandbox.ChatViewModel = function () {
    var self = this;
    
    this.composedMessage = ko.observable();
    this.messages = ko.observableArray();

    this.send = function () {
        Sandbox.server.hub.server.sendMessage(self.composedMessage());
        self.composedMessage(null);
    };
};

Sandbox.Server = function () {
    this.hub = $.connection.chatHub; // SandboxHub.cs

    this.hub.client.receiveMessage = function (message) {
        Sandbox.viewModel.chatData().messages.push(message); // TODO: Scroll to bottom of div
    };

    $.connection.hub.logging = true;

    $.connection.hub.start()
        .done(/* do nothing for now */)
        .fail(function () {
            console.error("Error connecting to SignalR hub. Please refresh the page to try again.");
        });

    $.connection.hub.disconnected(function () {
        console.error("Lost connection to server. Please refresh the page.");
    });
};