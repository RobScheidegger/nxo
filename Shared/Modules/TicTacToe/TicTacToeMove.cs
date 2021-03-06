using NXO.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NXO.Shared.Modules
{
    public class TicTacToeMove : IGameMove
    {
        public string PlayerId { get; set; }
        public string LobbyCode { get; set; }
        public IEnumerable<int> Path { get; set; }
    }
}
