using NXO.Shared.Modules;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Modules.TicTacToe
{
    public class TicTacToeGameLogicHandler
    {
        private static readonly int[] Primes = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37 };
        private static readonly ConcurrentDictionary<(int bas, int pow), int> exponentCache = new();
        private Dictionary<int, IEnumerable<List<int>>> VectorCache { get; set; } 
        public TicTacToeGameLogicHandler()
        {
            VectorCache = new Dictionary<int, IEnumerable<List<int>>>();   
        }
        public IEnumerable<List<int>> GetVectorsForDimension(int dimension, bool subCall = false)
        {
            if (VectorCache.ContainsKey(dimension))
            {
                return VectorCache[dimension];
            }
            else if (dimension == 1)
                return new List<List<int>>{
                    new List<int>() { 1 },
                    new List<int>() { 0 },
                    new List<int>() { -1 }
                };
            else
            {
                var result = GetVectorsForDimension(dimension - 1, true).SelectMany(vector =>
                    new List<List<int>>()
                    {
                        vector.Prepend(0).ToList(),
                        vector.Prepend(1).ToList(),
                        vector.Prepend(-1).ToList()
                    }
                );
                if(!subCall)
                {
                    //Sanitize and remove linearly dependent terms
                    var resultList = result.ToList();

                    int i = 0;
                    while(i < resultList.Count)
                    {
                        var value = resultList[i];
                        var negative = value.Select(i => -i).ToList();
                        var negativeHash = GetHash(negative);
                        var foundNegative = resultList.Where(q => GetHash(q) == negativeHash).LastOrDefault();
                        if (value.All(q => q == 0))
                        {
                            resultList.RemoveAt(i);
                        }
                        else
                            i++;
                    }
                    result = resultList;
                    VectorCache[dimension] = result;
                }
                return result; 
            }
        }
        public bool HasPlayerWon(char playerToken, TicTacToeBoard board)
        {
            var arrayBoard = GetArrayFromBoard(board);
            return HasPlayerWon(playerToken, arrayBoard);
        }
        public bool HasPlayerWon(char playerToken, Array arrayBoard)
        {
            var playerMoves = GetPositionFromBoardWhere(arrayBoard, (i, arr) => arr.GetValue(i) as char? == playerToken, arrayBoard.Rank);
            return HasPlayerWon(playerMoves, arrayBoard.Rank, arrayBoard.GetLength(0));   
        }
        public bool HasPlayerWon(IEnumerable<List<int>> playerMoves, int dimension, int boardSize)
        {
            var vectors = GetVectorsForDimension(dimension);
            var playerEdgeMoves = playerMoves.Where(move =>
                Enumerable.Range(0, dimension).Select(i => move[i]).Any(i => i == 0));
            var playerMovesHash = HashMoves(playerMoves);

            return playerEdgeMoves
                .Any(move => vectors
                    .Any(vector =>
                    {
                        var moveCheck = Enumerable.Range(0, boardSize).Select(n => MultiplyThenAdd(move, n, vector));

                        var hashes = moveCheck.Select(GetHash);

                        return hashes.All(playerMovesHash.Contains);
                    }));
        }
        public List<int> MultiplyThenAdd(List<int> array1, int scalar, List<int> array2)
        {
            List<int> result = new(array1.Count);
            for (int i = 0; i < array1.Count; i++)
            {
                result.Add(array1[i] + scalar * array2[i]);
            }
            return result;
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

        internal bool IsDraw(TicTacToeBoard board)
        {
            var available_moves = GetPositionFromBoardWhere(GetArrayFromBoard(board), (path, arr) => arr.GetValue(path) is null, board.Dimension);
            return !available_moves.Any();
        }

        public int GetHash(List<int> array)
        {
            int sum = 1;
            for(int i = 0; i < array.Count; i++)
            {
                sum *= Pow(Primes[i], array[i] + 1); 
            }
            return sum;
            static int Pow(int bas, int exp)
            {
                if(!exponentCache.ContainsKey((bas, exp)))
                {
                    exponentCache[(bas, exp)] = Enumerable
                                                  .Repeat(bas, exp)
                                                  .Aggregate(1, (a, b) => a * b);
                }
                return exponentCache[(bas, exp)];
            }
        }

        public bool InBounds(List<int> vector, int boardSize)
        {
            return vector.All(i => i >= 0 && i < boardSize);
        }

        public HashSet<int> HashMoves(IEnumerable<List<int>> playerMoves)
        {
            return new HashSet<int>(playerMoves.Select(GetHash));
        }
    }
}
