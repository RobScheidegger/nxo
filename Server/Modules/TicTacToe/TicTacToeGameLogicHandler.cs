using NXO.Shared.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Modules.TicTacToe
{
    public class TicTacToeGameLogicHandler
    {
        public TicTacToeGameLogicHandler()
        {
             
        }
        private IEnumerable<List<int>> GetVectorsForDimension(int dimension)
        {
            if (dimension == 1)
                return new List<List<int>>{
                    new List<int>() { 1 },
                    new List<int>() { 0 },
                    new List<int>() { -1 }
                };
            else
            {
                return GetVectorsForDimension(dimension - 1).SelectMany(vector =>
                    new List<List<int>>()
                    {
                        vector.Prepend(0).ToList(),
                        vector.Prepend(1).ToList(),
                        vector.Prepend(-1).ToList()
                    }
                );
            }
        }
        public bool HasPlayerWon(char playerToken, TicTacToeBoard board)
        {
            var arrayBoard = GetArrayFromBoard(board);
            return HasPlayerWon(playerToken, arrayBoard);
        }
        public bool HasPlayerWon(char playerToken, Array arrayBoard)
        {
            var vectors = GetVectorsForDimension(arrayBoard.Rank);
            var playerMoves = GetPositionFromBoardWhere(arrayBoard, (i, arr) => arr.GetValue(i) as char? == playerToken, arrayBoard.Rank);
            var playerEdgeMoves = playerMoves.Where(move => move.Contains(0));
            var playerMovesHash = new HashSet<int>(playerMoves.Select(GetHash));
            var boardSize = arrayBoard.GetLength(0);

            return playerEdgeMoves
                .Any(move => vectors
                .Where(i => !i.All(j => j == 0))
                .Any(vector =>
                {
                    var moveCheck = Enumerable.Range(0, boardSize).Select(n => Add(move, Multiply(n, vector)));

                    return moveCheck.Select(GetHash).All(playerMovesHash.Contains);
                }));

            List<int> Add(IEnumerable<int> firstArray, IEnumerable<int> secondArray)
            {
                return firstArray.Zip(secondArray, (x, y) => x + y).ToList();
            }

            List<int> Multiply(int scalar, IEnumerable<int> array)
            {
                return array.Select(i => i * scalar).ToList();
            }
        }
        public Array CloneBoard(Array originalBoard)
        {
            var output = Array.CreateInstance(typeof(char?),
                Enumerable.Range(0, originalBoard.Rank).Select(i => originalBoard.GetLength(0)).ToArray());
            Array.Copy(originalBoard, output, originalBoard.Length);
            return output;
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
        public IEnumerable<List<int>> GetPositionFromBoardWhere(Array board, Func<int[], Array, bool> selector, int currentDimension, int[] path = null)
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
                        var tempPath = new List<int>(path).ToList();
                        yield return tempPath;
                    }
                }
            }
            else
            {
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    path[board.Rank - currentDimension] = i;
                    foreach (var result in GetPositionFromBoardWhere(board, selector, currentDimension - 1, path))
                    {
                        yield return result;
                    }
                }
            }
        }
        public int GetHash(IEnumerable<int> array)
        {
            return array.Aggregate(0, (i, j) => HashCode.Combine(i, j));
        }
    }
}
