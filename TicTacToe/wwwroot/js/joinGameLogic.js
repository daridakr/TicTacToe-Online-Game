"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

connection.on("GameConfirmed", function (roomName) {
    window.location.href = '/Game?roomName=' + roomName + '&userName=' + document.getElementById('userName').innerHTML + '&circlePlayer=0';
});

connection.on("CirclePlayer", function (roomName) {
    window.location.href = '/Game?roomName=' + roomName + '&userName=' + document.getElementById('userName').innerHTML + '&circlePlayer=1';
});
var globalOpenRooms;
connection.on("UpdateOpenRooms", function (OpenRooms) {
    globalOpenRooms = OpenRooms;
    updateOpenRooms();
});

function updateOpenRooms() {
    var roomsList = document.querySelector("#roomsList");
    var roomsDiv = document.createElement("div");
    roomsDiv.id = "roomsList";
    roomsDiv.innerHTML = "";
    globalOpenRooms.forEach(roomName => {
        if (roomName != document.getElementById("roomName").innerHTML) {
            var p = document.createElement("p");
            p.innerHTML = "Room name: " + roomName;
            p.className = "testOpenRoom";
            var btn = document.createElement("button");
            btn.textContent = "Join Room";
            btn.addEventListener('click', function () {
                // Create group in Hub
                connection.invoke("JoinRoom", roomName).catch(function (err) {
                    return console.error(err.toString());
                });
                // Go to game view
                window.location.href = '/Game?circlePlayer=1'; 
            }, false);
            p.appendChild(btn);
            roomsDiv.appendChild(p);
        }
    });
    roomsList.replaceWith(roomsDiv);
}

connection.start().then(function () {

});

var openRoomsSelection = document.getElementsByClassName('openRooms');
for (var i = 0; i < openRoomsSelection.length; i++) {
    (function (index) {
        openRoomsSelection[index].addEventListener("click", function (event) {
            // Hitta nuvarande rumsnamn
            var roomName = openRoomsSelection[index].name;
            connection.invoke("JoinRoom", roomName).catch(function (err) {
                return console.error(err.toString());
            });
            event.preventDefault();
        })
    })(i);
}

var userSelection = document.getElementsByClassName('userUnit');
for (var i = 0; i < userSelection.length; i++) {
    (function (index) {
        userSelection[index].addEventListener("click", function (event) {
            // Hitta nuvarande användares userNumber
            var user = userSelection[index].name;
            // var message = document.getElementById("messageInput").value;
            connection.invoke("AskGame", user).catch(function (err) {
                return console.error(err.toString());
            });
            event.preventDefault();
        })
    })(i);
}

document.getElementById("joinGameHead").addEventListener("click", function (event) {
    window.location.href = "/JoinGame";
});

document.getElementById("createGameHead").addEventListener("click", function (event) {
    window.location.href = "/NewGame";
});