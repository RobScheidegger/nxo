using NXO.Shared.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NXO.UnitTests.TicTacToe
{
    public static class TicTacToeTestUtilities
    {
        /// <summary>
        /// Constructor used as a testing utility for instantiating a board.
        /// </summary>
        /// <param name="array"></param>
        public static TicTacToeBoard Get2DBoard(char?[,] array)
        {
            var boards = new List<TicTacToeBoard>();
            for(int i = 0; i < array.GetLength(0); i++)
            {
                
                var endBoards = new List<TicTacToeBoard>();
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    endBoards.Add(new TicTacToeBoard()
                    {
                        Cell = array[i,j],
                        Dimension = 0,
                        Endpoint = true,
                        Position = j
                    });
                }
                boards.Add(new TicTacToeBoard()
                {
                    Boards = endBoards,
                    Dimension = 1,
                    Endpoint = false,
                    Position = i
                });
            }
            return new TicTacToeBoard()
            {
                Endpoint = false,
                Boards = boards,
                Position = 0,
                Dimension = 2
            };
        }
        /// <summary>
        /// Constructor used as a testing utility for instantiating a board.
        /// </summary>
        /// <param name="array"></param>
        public static TicTacToeBoard Get3DBoard(char?[,,] array)
        {
            throw new NotImplementedException();
        }
    }
}
