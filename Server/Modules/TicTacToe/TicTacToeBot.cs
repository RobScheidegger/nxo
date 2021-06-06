using NXO.Shared.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NXO.Server.Modules.TicTacToe
{
    public class TicTacToeBot
    {
        private readonly TicTacToeGameLogicHandler logic;
        private readonly Random random = new();
        private readonly int defaultAlpha = -1000;
        private readonly int defaultBeta = 1000;
        private readonly int defaultMaxDepth = 5;

        public TicTacToeBot(TicTacToeGameLogicHandler logic)
        {
            this.logic = logic;
        }

        public Task<TicTacToeMove> GetNextMove(TicTacToeGameStatus GameStatus)
        {
            List<int> depthReached = new();
            List<int> scores = new();
            List<List<int>> moves = new();
            TicTacToeBoard board = GameStatus.Board;
            Array a = logic.GetArrayFromBoard(board);
            TicTacToePlayer currentPlayer = GameStatus.Players.Where(p => p.PlayerId == GameStatus.CurrentPlayerId).First();
            TicTacToePlayer opp = GameStatus.Players.Where(p => p.PlayerId != currentPlayer.PlayerId).First();
            var available_moves = logic.GetPositionFromBoardWhere(a, (path, arr) => arr.GetValue(path) is null, board.Dimension);
            bool hasMoved = logic.GetPositionFromBoardWhere(a, (path, arr) => arr.GetValue(path) as char? == currentPlayer.Token, board.Dimension).Any();
            if (!available_moves.Any())
            {
                return null;
            }
            foreach (var move in available_moves)
            {
                moves.Add(move);
                scores.Add(-1000);
                depthReached.Add(0);
            }

            var cancelToken = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                var forcedMove = false;
                for (int d = 0; d < defaultMaxDepth + 1; d++)
                {
                    for (int i = 0; i < moves.Count; i++)
                    {
                        for (int j = 0; j < d + 1; j++)
                        {
                            scores[i] = IterativeDeepening(a, 0, j, GameStatus, currentPlayer, defaultAlpha, defaultBeta, moves[i]);
                            depthReached[i] = j;
                            if (cancelToken.Token.IsCancellationRequested || forcedMove)
                            {
                                break;
                            }
                        }
                        if (cancelToken.Token.IsCancellationRequested || forcedMove)
                        {
                            break;
                        }
                    }
                    forcedMove = scores.Select((x, i) => new { Index = i, Value = x })
                    .Where(x => x.Value == scores.Max())
                    .Select(x => x.Index).Count() == 1;
                    if (cancelToken.Token.IsCancellationRequested || forcedMove || !hasMoved)
                    {
                        break;
                    }
                }
            }, cancelToken.Token);

            bool isCompletedSuccessfully = task.Wait(TimeSpan.FromSeconds(30));
            if (!isCompletedSuccessfully)
            {
                cancelToken.Cancel();
            }

            /*foreach (var move in available_moves)
            {
                moves.Add(move);
                scores.Add(Minimax(a, 0, GameStatus, currentPlayer, defaultAlpha, defaultBeta, move));
            }*/


            var searchIndex = random.Next(0, scores.LastIndexOf(scores.Max()));
            if (searchIndex == -1)
            {
                searchIndex = 0;
            }
            TicTacToeMove bestMove = new()
            {
                PlayerId = GameStatus.CurrentPlayerId,
                LobbyCode = GameStatus.LobbyCode,
                Path = moves[scores.IndexOf(scores.Max(), searchIndex)]
            };
            return Task.FromResult(bestMove);
        }

        public int Minimax(Array board, int depth, TicTacToeGameStatus GameStatus, TicTacToePlayer currentPlayer, int alpha, int beta, List<int> firstMove = null)
        {

            int bestVal;
            char currentToken = currentPlayer.Token;
            TicTacToePlayer opp = GameStatus.Players.Where(p => p.PlayerId != currentPlayer.PlayerId).First();
            int minmaxInt = currentPlayer.Bot ? 10 : -10;
            int score = logic.HasPlayerWon(currentToken, board) ? minmaxInt : logic.HasPlayerWon(opp.Token, board) ? (minmaxInt * -1) : 0;

            if (score == 10)
            {
                return score - depth;
            }
            else if (score == -10)
            {
                return score + depth;
            }


            var available_moves = firstMove == null ? logic.GetPositionFromBoardWhere(board, (path, arr) => arr.GetValue(path) is null, board.Rank) : new List<List<int>>() { firstMove };
            if (!available_moves.Any())
            {
                return 0;
            }

            if (depth >= 3)
            {
                return currentPlayer.Bot ? defaultAlpha : defaultBeta;
            }

            if (currentPlayer.Bot) // MAX
            {
                bestVal = defaultAlpha;
                foreach (var move in available_moves)
                {

                    Array testBoard = logic.CloneBoard(board);
                    testBoard.SetValue(currentToken, move.ToArray());
                    bestVal = Math.Max(bestVal, Minimax(testBoard, depth + 1, GameStatus, opp, alpha, beta));

                    alpha = Math.Max(alpha, bestVal);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }

            }
            else // MIN
            {
                bestVal = defaultBeta;
                foreach (var move in available_moves)
                {
                    Array testBoard = logic.CloneBoard(board);
                    testBoard.SetValue(currentToken, move.ToArray());
                    bestVal = Math.Min(bestVal, Minimax(testBoard, depth + 1, GameStatus, opp, alpha, beta));
                    beta = Math.Min(beta, bestVal);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
            }
            return bestVal;
        }
        public int IterativeDeepening(Array board, int depth, int maxDepth, TicTacToeGameStatus GameStatus, TicTacToePlayer currentPlayer, int alpha, int beta, List<int> firstMove = null)
        {
            int bestVal = currentPlayer.Bot ? defaultAlpha : defaultBeta;
            char currentToken = currentPlayer.Token;
            TicTacToePlayer opp = GameStatus.Players.Where(p => p.PlayerId != currentPlayer.PlayerId).First();
            int minmaxInt = currentPlayer.Bot ? 10 : -10;
            int score = logic.HasPlayerWon(currentToken, board) ? minmaxInt : logic.HasPlayerWon(opp.Token, board) ? (minmaxInt * -1) : 0;

            if (score == 10)
            {
                return score - depth;
            }
            else if (score == -10)
            {
                return score + depth;
            }
            if (depth > maxDepth)
            {
                return score;
            } else 
            {
                var available_moves = firstMove == null ? logic.GetPositionFromBoardWhere(board, (path, arr) => arr.GetValue(path) is null, board.Rank) : new List<List<int>>() { firstMove };
                if (currentPlayer.Bot) // MAX
                {
                    foreach (var move in available_moves)
                    {

                        Array testBoard = logic.CloneBoard(board);
                        testBoard.SetValue(currentToken, move.ToArray());
                        bestVal = Math.Max(bestVal, IterativeDeepening(testBoard, depth + 1, maxDepth, GameStatus, opp, alpha, beta));

                        alpha = Math.Max(alpha, bestVal);
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }

                }
                else // MIN
                {
                    foreach (var move in available_moves)
                    {
                        Array testBoard = logic.CloneBoard(board);
                        testBoard.SetValue(currentToken, move.ToArray());
                        bestVal = Math.Min(bestVal, IterativeDeepening(testBoard, depth + 1, maxDepth, GameStatus, opp, alpha, beta));
                        beta = Math.Min(beta, bestVal);
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }
            }

            return bestVal;
        }
    }
}
