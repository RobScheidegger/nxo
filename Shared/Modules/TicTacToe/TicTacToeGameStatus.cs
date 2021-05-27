using NXO.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NXO.Shared.Modules
{
    public class TicTacToeGameStatus : IGameStatus
    {
        /// <summary>
        /// Array representing the current state of the game board, with 
        /// </summary>
        public TicTacToeBoard Board { get; set; }
        public string CurrentPlayerId { get; set; }
        public string CurrentPlayerName { get; set; }
        public TicTacToePlayer Winner { get; set; }
        public bool Completed { get; set; }
        public int BoardSize { get; set; }
        public int Dimensions { get; set; }
        public string LobbyCode { get; set; }
        public int MaximumPlayers { get; set; }
        public IEnumerable<TicTacToePlayer> Players { get; set; }
        public DateTime DateCreated { get; set; }
        public string GameType { get; set; }
        public string Stage { get; set; }
        public string Nickname { get; set; }
        public string HostPlayerId { get; set; }

        public int PlayerCount { get {
                return Players?.Count()??0;
            } }
        public List<TicTacToeGameHistoryEntry> History { get; set; }
    }
    
}
