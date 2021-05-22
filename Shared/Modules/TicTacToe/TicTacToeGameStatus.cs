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
     }
    public class TicTacToeBoard
    {
        public IEnumerable<TicTacToeBoard> Boards { get; set; }
        public bool Endpoint { get; set; } = false;
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
                    Endpoint = Dimensions == 0,
                    Dimension = Dimensions,
                    Position = Position??0
                };
                return board;
            }
            else
                return null;
        }
    }
}
