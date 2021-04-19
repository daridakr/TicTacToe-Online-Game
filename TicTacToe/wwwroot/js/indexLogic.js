"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

//Disable send button until connection is established
document.getElementById("joinGame").disabled = true;
document.getElementById("createGame").disabled = true;
document.getElementById("joinGameHead").disabled = true;
document.getElementById("createGameHead").disabled = true;
var nameSet = false;

connection.on("UserCreated", function (userName) {
    var p = document.createElement("p");
    p.textContent = userName;
    p.id = "userName";
    document.getElementById("userName").replaceWith(p);
    userCreated();
});

function userCreated() {
    document.getElementById("createUser").hidden = true;
    document.getElementById("joinGame").disabled = false;
    document.getElementById("createGame").disabled = false;
    document.getElementById("joinGameHead").disabled = false;
    document.getElementById("createGameHead").disabled = false;
    nameSet = true;
    //updateOpenRooms();
}


connection.start().then(function () {
    if(typeof ifNameFromOtherGame !== 'undefined') userCreated();
}).catch(function (err) {
    return console.error(err.toString());
});

var user;
document.getElementById("createUser").addEventListener("click", function (event) {
    var roomName = document.getElementById("userName").value;
    user = roomName;
    connection.invoke("CreateUser", roomName).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("joinGame").addEventListener("click", function (event) {
    window.location.href = "/JoinGame?userName=" + user;
});

document.getElementById("joinGameHead").addEventListener("click", function (event) {
    window.location.href = "/JoinGame?userName=" + user;
});

document.getElementById("createGame").addEventListener("click", function (event) {
    window.location.href = "/NewGame?userName=" + user;
});

document.getElementById("createGameHead").addEventListener("click", function (event) {
    window.location.href = "/NewGame?userName=" + user;
});

document.getElementById("MainPage").addEventListener("click", function (event) {

    window.location.href = "/Index";
});