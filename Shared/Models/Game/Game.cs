using System;
using System.Collections.Generic;
using System.Text;

namespace NXO.Shared.Models
{
    public class Game
    {
        public DateTime DateCreated { get; set; }
        public string LobbyCode { get; set; }
        public string GameType { get; set; }
        public GameSettings Settings { get; set; }
        public string Nickname { get; set; }
        public IEnumerable<Player> Players { get; set; }
        public string Stage { get; set; }
        public string HostPlayerId { get; set; }
    }
}
