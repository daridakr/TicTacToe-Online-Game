using System.Collections;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using TicTacToe.Models;

namespace TicTacToe.Hubs
{
    public class GameHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            UserHandler.ConnectedIds.Add(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task CreateUser(string userName)
        {
            UserHandler.Users.Add(new User(userName));
            await Clients.Client(Context.ConnectionId).SendAsync("UserCreated", userName);
        }
        public async Task CreateRoom(string roomName)
        {
            await Clients.Client(Context.ConnectionId).SendAsync("RoomCreated", roomName);
            GroupHandler.GroupNames.Add(roomName);
            GroupHandler.OpenRooms.Add(roomName);
            await Clients.All.SendAsync("UpdateOpenRooms", GroupHandler.OpenRooms);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

    	public async Task JoinRoom(string roomName)
        {
            GroupHandler.OngoingGames.Add(roomName);
            GroupHandler.GroupUnits.Add(new GroupUnit(roomName));
            GroupHandler.GroupCircleWins.Add(roomName, 0);
            GroupHandler.GroupCrossWins.Add(roomName, 0);
            GroupHandler.GroupDrawWins.Add(roomName, 0);
            GroupHandler.GroupRoundsPlayed.Add(roomName, 0);
            
            GroupHandler.OpenRooms.Remove(roomName);
            await Clients.All.SendAsync("UpdateOngoingGames", GroupHandler.OngoingGames);
            await Clients.All.SendAsync("UpdateOpenRooms", GroupHandler.OpenRooms);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).SendAsync("GameConfirmed", roomName);
            await Clients.Client(Context.ConnectionId).SendAsync("CirclePlayer", roomName);
        }

        public async Task EnterRoom(string roomName, string userName, int circlePlayer)
        {
            UserHandler.Users.Find(x => x._Name == userName).Room = roomName;
            if (circlePlayer == 1) {
                UserHandler.Users.Find(x => x._Name == userName).circlePlayer = true;
            } else {
                UserHandler.Users.Find(x => x._Name == userName).circlePlayer = false;
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }
        
        public async Task MakeMove(string roomName, int squareNumber, int circlePlayer)
        {
            await Clients.Group(roomName).SendAsync("MoveMade", squareNumber);
            if (circlePlayer==1) {
                GroupHandler.GroupUnits.Find(x => x._Name == roomName).Circles.Add(squareNumber);
            } else {
                GroupHandler.GroupUnits.Find(x => x._Name == roomName).Crosses.Add(squareNumber);
            }
            await Clients.Group(roomName + "Watch").SendAsync("MoveMade", GroupHandler.GroupUnits.Find(x => x._Name == roomName).Circles, GroupHandler.GroupUnits.Find(x => x._Name == roomName).Crosses);
        }

        public async Task RoundFinished(string roomName, string winner)
        {
            await Clients.Group(roomName).SendAsync("RoundFinished", winner);
            UserHandler.Users.FindAll(x => x.Room == roomName).ForEach(x => {
                if (x.circlePlayer == true) {
                    if (winner == "circle") {
                        x.Wins++;
                        GroupHandler.GroupCircleWins[roomName]++;
                    }
                    if (winner == "cross") {
                        x.Losses++;
                        GroupHandler.GroupCrossWins[roomName]++;
                    }
                    if (winner == "draw") {
                        x.Draws++;
                        GroupHandler.GroupDrawWins[roomName]++;
                    }
                } else {
                    if (winner == "circle") x.Losses++;
                    if (winner == "cross") x.Wins++;
                    if (winner == "draw") x.Draws++;
                }
            });
            GroupHandler.GroupRoundsPlayed[roomName]++;
            UserHandler.GamesPlayed++;
        }
        
        public async Task AskNewGame(string roomName)
        {
            await Clients.OthersInGroup(roomName).SendAsync("YesToNewGame");
            // await Clients.All.SendAsync("MakeMove");
        }

        public async Task YesToNewGame(string roomName)
        {
            await Clients.Group(roomName).SendAsync("StartNewGame");
            await Clients.Group(roomName + "Watch").SendAsync("StartNewGame");
            GroupHandler.GroupUnits.RemoveAll(x => x._Name == roomName);
            GroupHandler.GroupUnits.Add(new GroupUnit(roomName));
        }

        public async Task LeaveGame(string roomName)
        {
            GroupHandler.OngoingGames.Remove(roomName);
            await Clients.All.SendAsync("UpdateOngoingGames", GroupHandler.OngoingGames);
            await Clients.Group(roomName).SendAsync("GameEnded");
            await Clients.Group(roomName + "Watch").SendAsync("GameEnded");
        }
        
        public async Task WatchGame(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName + "Watch");
        }
        public async Task EnterWatchGame(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName + "Watch");
        }
    }
}