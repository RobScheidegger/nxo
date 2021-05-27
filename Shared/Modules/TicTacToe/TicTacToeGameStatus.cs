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
    }
    public class TicTacToeBoard
    {
        public IEnumerable<TicTacToeBoard> Boards { get; set; }
        public bool Endpoint { get { return Dimension == 0;  } }
        public char? Cell { get; set; }
        public int Dimension { get; set; }
        public int Position { get; set; }
        public static TicTacToeBoard Construct(int Dimensions, int BoardSize, int? Position = null)
        {
            if (Dimensions >= 0)
            {
                var board = new TicTacToeBoard()
                {
                    Boards = Dimensions == 0 ? null : Enumerable.Range(0, BoardSize).Select(i => Construct(Dimensions - 1, BoardSize, i)).ToList(),
                    Dimension = Dimensions,
                    Position = Position??0
                };
                return board;
            }
            else
                return null;
        }
        public char? GetForPosition(IEnumerable<int> Path)
        {
            if(Path.Count() == 0)
            {
                return Cell;
            }
            else
            {
                return Boards.ElementAt(Path.First()).GetForPosition(Path.Skip(1));
            }
        }
        public void Place(char playerToken, IEnumerable<int> Path)
        {
            if(Path.Count() == 0)
            {
                Cell = playerToken;
            }
            else
            {
                Boards.ElementAt(Path.First()).Place(playerToken, Path.Skip(1));
            }
        }
    }
}
