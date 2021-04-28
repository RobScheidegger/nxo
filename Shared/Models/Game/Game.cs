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
        public LobbyStatus LobbyStatus { get; set; }
    }
}
