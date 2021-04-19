using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace TicTacToe.Models
{
    public class GroupUnit
    {
        public string _Name;
        public List<int> Circles { get; set; }
        public List<int> Crosses { get; set; }

        public GroupUnit(string Name)
        {
            _Name = Name;
            Circles = new List<int>();
            Crosses = new List<int>();
        }
    }

    public static class GroupHandler
    {
        public static List<string> GroupNames = new List<string>();

        public static List<string> OpenRooms = new List<string>();
        public static List<string> OngoingGames = new List<string>();

        public static Dictionary<string, string> GroupStats = new Dictionary<string, string>();
        public static Dictionary<string, int> GroupCircleWins = new Dictionary<string, int>();
        public static Dictionary<string, int> GroupCrossWins = new Dictionary<string, int>();
        public static Dictionary<string, int> GroupDrawWins = new Dictionary<string, int>();
        public static Dictionary<string, int> GroupRoundsPlayed = new Dictionary<string, int>();
        public static List<GroupUnit> GroupUnits = new List<GroupUnit>();
    }
}
