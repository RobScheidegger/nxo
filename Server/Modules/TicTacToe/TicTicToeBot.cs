using NXO.Shared.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Modules.TicTacToe
{
    public class TicTicToeBot
    {
        public async Task<TicTacToeMove> GetNextMove(char MoveIdentifier, TicTacToeGameStatus GameStatus)
        {
            int[] scores;
            int[] moves;
            TicTacToeBoard board = GameStatus.Board;
            Array a = GetArrayFromBoard(board);

            throw new NotImplementedException();
        }

        public static Array GetArrayFromBoard(TicTacToeBoard board)
        {
            var output = Array.CreateInstance(typeof(char?), Enumerable.Range(0, board.Dimension).Select(i => board.Boards.Count()).ToArray());
            correctBoardfarts(ref output, board, Enumerable.Empty<int>());
            return output;
        }

        public static void correctBoardfarts(ref Array array, TicTacToeBoard board, IEnumerable<int> path)
        {
            if (board.Dimension == 0)
            {
                array.SetValue(board.Cell, path.ToArray());
            }
            else
            {
                foreach (TicTacToeBoard b in board.Boards)
                {
                    correctBoardfarts(ref array, b, path.Append(b.Position));
                }
            }
        }
    }
}
