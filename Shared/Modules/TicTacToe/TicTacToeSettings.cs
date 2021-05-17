using NXO.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NXO.Shared.Modules
{
    public class TicTacToeSettings : IGameSettings
    {
        public int BoardSize { get; set; }
        public int Dimensions { get; set; }
        public string LobbyCode { get; set; }
        public int MaximumPlayers { get; set; }
    }
}
