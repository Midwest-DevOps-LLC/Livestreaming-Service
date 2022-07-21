"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.start().then(function () {
    //document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    li.innerHTML = `${user}: ${message}`;

    //Todo if scroll not at latest then don't move scrollbar
    $('#messageColumn').scrollTop($('#messageColumn')[0].scrollHeight);
});

$("#sendButton").click(function (event) {
    //var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    var auth = document.getElementById("userAuth").value;
    connection.invoke("SendMessage", auth, message).catch(function (err) {
        return console.error(err.toString());
    });

    $('#messageInput').val('');
    event.preventDefault();
});

var streamHeight = 0;

function RecalcTextBoxHeight() {
    var screen = $('#my-video_html5_api')

    streamHeight = screen.height();

    var inputSendMessage = $('#test')

    streamHeight -= inputSendMessage.height();
    //streamHeight -= 10;

    $('#messageColumn').css("max-height", streamHeight);
    $('#messageColumn').css("height", streamHeight);
}

$('#messageInput').keypress(function (e) {
    var key = e.which;
    if (key == 13)  // the enter key code
    {
        $('#sendButton').click();
        return false;
    }
});   

$(window).resize(function () {
    RecalcTextBoxHeight();
});

videojs("my-video").ready(function () {
    RecalcTextBoxHeight();
});