using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace TicTacToe.Models
{
    public class User
    {
        public string _Name;
        public string Room;
        public bool circlePlayer;
        public int Wins;
        public int Losses;
        public int Draws;

        public User(string Name)
        {
            _Name = Name;
            Wins = 0;
            Losses = 0;
            Draws = 0;
            Room = "";
            circlePlayer = false;
        }
    }
    public static class UserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
        public static List<User> Users = new List<User>();
        public static int GamesPlayed = 0;
    }
}
