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
        private const int defaultAlpha = -1000;
        private const int defaultBeta = 1000;
        private const int defaultMaxDepth = 5;
        private const int timeout = 30;

        public TicTacToeBot(TicTacToeGameLogicHandler logic)
        {
            this.logic = logic;
        }

        public Task<TicTacToeMove> GetNextMove(TicTacToeGameStatus GameStatus)
        {
            TicTacToeBoard board = GameStatus.Board;
            Array startingBoard = logic.GetArrayFromBoard(board);

            TicTacToePlayer currentPlayer = GameStatus.Players.Where(p => p.PlayerId == GameStatus.CurrentPlayerId).First();
            TicTacToePlayer oppositePlayer = GameStatus.Players.Where(p => p.PlayerId != currentPlayer.PlayerId).First();

            var currentPlayerMoves = logic.GetPositionFromBoardWhere(startingBoard, (path, arr) => 
                arr.GetValue(path) as char? == currentPlayer.Token, board.Dimension);
            var oppositePlayerMoves = logic.GetPositionFromBoardWhere(startingBoard, (path, arr) =>
                arr.GetValue(path) as char? == oppositePlayer.Token, board.Dimension);

            var available_moves = logic.GetPositionFromBoardWhere(startingBoard, (path, arr) => arr.GetValue(path) is null, board.Dimension);
            bool hasMoved = logic.GetPositionFromBoardWhere(startingBoard, (path, arr) => arr.GetValue(path) as char? == currentPlayer.Token, board.Dimension).Any();
            if (!available_moves.Any())
            {
                return null;
            }

            List<MinimaxResult> minimaxMoves = available_moves.Select((v, i) => new MinimaxResult()
            {
                DepthReached = 0,
                Score = -1000,
                Move = v,
                Index = i
            }).ToList();

            var trackedAvailableMoves = available_moves.Select(i => new TrackedMove()
            {
                Available = true,
                Move = i
            }).ToList();

            var cancelToken = new CancellationTokenSource();
            var task = Task.Run(() =>
            {
                var forcedMove = false;
                for (int d = 0; d < defaultMaxDepth + 1; d++)
                {
                    foreach(var move in minimaxMoves)
                    {
                        for (int j = 0; j < d + 1; j++)
                        {
                            move.Score = IterativeDeepening(
                                availableMoves: ref trackedAvailableMoves,
                                currentPlayerMoves: oppositePlayerMoves,
                                oppositePlayerMoves: currentPlayerMoves,
                                depth: 0,
                                maxDepth: j,
                                gameStatus: GameStatus,
                                currentPlayer: currentPlayer,
                                alpha: defaultAlpha,
                                beta: defaultBeta,
                                firstMove: trackedAvailableMoves[move.Index]);
                            move.DepthReached = j;
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
                    var maxScore = minimaxMoves.Max(i => i.Score);
                    forcedMove = minimaxMoves.Where(i => i.Score == maxScore).Count() == 1;
                    if (cancelToken.Token.IsCancellationRequested || forcedMove || !hasMoved)
                    {
                        break;
                    }
                }
            }, cancelToken.Token);

            bool isCompletedSuccessfully = task.Wait(TimeSpan.FromSeconds(timeout));
            if (!isCompletedSuccessfully)
            {
                cancelToken.Cancel();
            }

            var maxScore = minimaxMoves.Max(i => i.Score);
            var maxScoreCandidates = minimaxMoves.Where(i => i.Score == maxScore);
            MinimaxResult finalResult;
            if (maxScoreCandidates.Count() == 0)
                finalResult = minimaxMoves.First();
            else
            {
                var randIndex = random.Next(0, maxScoreCandidates.Count());
                finalResult = maxScoreCandidates.ElementAt(randIndex);
            }
            TicTacToeMove bestMove = new()
            {
                PlayerId = GameStatus.CurrentPlayerId,
                LobbyCode = GameStatus.LobbyCode,
                Path = finalResult.Move
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
        public int IterativeDeepening(ref List<TrackedMove> availableMoves, IEnumerable<List<int>> currentPlayerMoves, IEnumerable<List<int>> oppositePlayerMoves, int depth, int maxDepth, TicTacToeGameStatus gameStatus, TicTacToePlayer currentPlayer, int alpha, int beta, TrackedMove firstMove = null)
        {
            int bestVal = currentPlayer.Bot ? defaultAlpha : defaultBeta;
            char currentToken = currentPlayer.Token;
            TicTacToePlayer oppositionPlayer = gameStatus.Players.Where(p => p.PlayerId != currentPlayer.PlayerId).First();
            int minmaxInt = currentPlayer.Bot ? 10 : -10;
            int score = logic.HasPlayerWon(currentPlayerMoves, gameStatus.Dimensions, gameStatus.BoardSize)
                ? minmaxInt : (logic.HasPlayerWon(oppositePlayerMoves, gameStatus.Dimensions, gameStatus.BoardSize) ? (minmaxInt * -1) : 0);

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
            }
            else 
            {
                if (currentPlayer.Bot) // MAX
                {
                    IEnumerable<TrackedMove> move_enumerable = firstMove == null ? availableMoves.Where(i => i.Available) : new List<TrackedMove> { firstMove };
                    foreach (var move in move_enumerable)
                    {
                        move.Available = false;
                        bestVal = Math.Max(bestVal, IterativeDeepening(
                            availableMoves: ref availableMoves, 
                            currentPlayerMoves: oppositePlayerMoves,
                            oppositePlayerMoves: currentPlayerMoves.Append(move.Move),
                            depth: depth + 1, 
                            maxDepth: maxDepth,
                            gameStatus: gameStatus, 
                            currentPlayer: oppositionPlayer, 
                            alpha: alpha, 
                            beta: beta));

                        alpha = Math.Max(alpha, bestVal);
                        move.Available = true;
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }

                }
                else // MIN
                {
                    IEnumerable<TrackedMove> move_enumerable = firstMove == null ? availableMoves.Where(i => i.Available) : new List<TrackedMove> { firstMove };
                    foreach (var move in move_enumerable)
                    {
                        move.Available = false;
                        bestVal = Math.Min(bestVal, IterativeDeepening(
                            availableMoves: ref availableMoves,
                            currentPlayerMoves: oppositePlayerMoves,
                            oppositePlayerMoves: currentPlayerMoves.Append(move.Move),
                            depth: depth + 1,
                            maxDepth: maxDepth,
                            gameStatus: gameStatus,
                            currentPlayer: oppositionPlayer,
                            alpha: alpha,
                            beta: beta));
                        beta = Math.Min(beta, bestVal);
                        move.Available = true;
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
    public class MinimaxResult
    {
        public int DepthReached { get; set; }
        public List<int> Move { get; set; }
        public int Score { get; set; }
        public int Index { get; set; }
    }
    public class TrackedMove
    {
        public bool Available { get; set; }
        public List<int> Move { get; set; }
    }
}
