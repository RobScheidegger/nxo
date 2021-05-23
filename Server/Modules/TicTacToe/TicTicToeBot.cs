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

        public Array GetArrayFromBoard(TicTacToeBoard board)
        {
            var output = Array.CreateInstance(typeof(char?), 
                Enumerable.Range(0, board.Dimension).Select(i => board.Boards.Count()).ToArray());
            ParseBoardTree(ref output, board, Enumerable.Empty<int>());
            return output;
        }

        public void ParseBoardTree(ref Array array, TicTacToeBoard board, IEnumerable<int> path)
        {
            if (board.Dimension == 0)
            {
                array.SetValue(board.Cell, path.ToArray());
            }
            else
            {
                foreach (TicTacToeBoard b in board.Boards)
                {
                    ParseBoardTree(ref array, b, path.Append(b.Position));
                }
            }
        }
        public IEnumerable<int[]> GetPositionFromBoardWhere(Array board, Func<int[], Array, bool> selector, int currentDimension, int[] path = null)
        {
            if (path == null)
                path = new int[board.Rank];
            if (currentDimension == 1)
            {
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    path[board.Rank - currentDimension] = i;
                    if (selector(path, board))
                    {
                        var tempPath = new List<int>(path).ToArray();
                        yield return tempPath;
                    }
                }
            }
            else
            {
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    path[board.Rank - currentDimension] = i;
                    foreach(var result in GetPositionFromBoardWhere(board, selector, currentDimension - 1, path))
                    {
                        yield return result;
                    }
                }
            }
        }
    }
}
