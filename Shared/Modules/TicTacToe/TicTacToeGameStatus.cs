using NXO.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NXO.Shared.Modules
{
    public class TicTacToeGameStatus : IGameStatus
    {
        /// <summary>
        /// Dictionary of player moves by player Id
        /// </summary>
        public Dictionary<string, List<int[]>> PlayerMoves { get; set; }
    }
}
