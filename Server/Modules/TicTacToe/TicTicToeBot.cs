using NXO.Shared.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Modules.TicTacToe
{
    public class TicTicToeBot
    {
        public async Task<TicTacToeMove> GetNextMove(char MoveIdentifier, TicTacToeGameStatus GameStatus, TicTacToeSettings GameSettings)
        {
            List<int> scores = new();
            List<int[]> moves = new();
            TicTacToeBoard board = GameStatus.Board;
            Array a = GetArrayFromBoard(board);
            char t = GameSettings.Players.Where(p => p.PlayerId == GameStatus.CurrentPlayerId).First().Token;
            var available_moves = GetPositionFromBoardWhere(a, (path, arr) => arr.GetValue(path) is null, 2);
            if (!available_moves.Any())
            {
                return null;
            }
            foreach (var move in available_moves)
            {
                moves.Add(move);
                Array testBoard = CloneBoard(a);
                testBoard.SetValue(t, move);
                scores.Add(Minimax(a, 0, GameStatus.CurrentPlayerId, t));
            }
            TicTacToeMove bestMove = new()
            {
                PlayerId = GameStatus.CurrentPlayerId,
                LobbyCode = GameSettings.LobbyCode,
                Path = moves[scores.IndexOf(scores.Max())]
            };
            return bestMove;
        }

        public int Minimax(Array board, int depth, string currentPlayer, char currentToken)
        {
            int bestVal;
            if (depth == 1)
            {
                return 0;
            }
            int score = EvaluateWin(board);
            if (score == 10)
            {
                return score - depth;
            }
            else if (score == -10)
            {
                return score + depth;
            }
            var available_moves = GetPositionFromBoardWhere(board, (path, arr) => arr.GetValue(path) is null, 2);
            if (!available_moves.Any())
            {
                return 0;
            }
            if (currentPlayer.Equals("bot")) // MAX
            {
                bestVal = -1000;
                foreach (var move in available_moves)
                {
                    Array testBoard = CloneBoard(board);
                    testBoard.SetValue(currentToken, move);
                    bestVal = Math.Max(bestVal,Minimax(testBoard, depth+1, currentPlayer, currentToken));
                }

            } else // MIN
            {
                bestVal = 1000;
                foreach (var move in available_moves)
                {
                    Array testBoard = CloneBoard(board);
                    testBoard.SetValue(currentToken, move);
                    bestVal = Math.Min(bestVal, Minimax(testBoard, depth + 1, currentPlayer, currentToken));
                }
            }
            return bestVal;
        }

        public int EvaluateWin(Array board, int[] move = null, char currentPlayer = '_')
        {
            int dim = board.Rank;
            int size = board.GetLength(0);
            if (move != null)
            {
                Array mov = Array.CreateInstance(typeof(int), dim);
                for (int d = 0; d < dim; d++)
                {

                }

                var available_moves = GetPositionFromBoardWhere(board, (path, arr) => arr.GetValue(path) as char? == currentPlayer, 2);


            } else
            {

                for (int s = 0; s < size; s++)
                {
                    for (int d = 0; d < dim; d++)
                    {

                    }
                }
            }
            return 0;
        }

        public static Array CloneBoard(Array originalBoard)
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
