using NXO.Shared.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NXO.Server.Modules.TicTacToe
{
    public class TicTacToeMinimaxBot : ITicTacToeBot
    {
        private readonly TicTacToeGameLogicHandler logic;
        private readonly Random random = new();
        private const int defaultAlpha = -5000;
        private const int defaultBeta = 5000;
        private const int defaultMaxDepth = 7;
        private const int timeout = 30;
        private const int winningScore = 1000;

        public string Type => "Minimax";

        public TicTacToeMinimaxBot(TicTacToeGameLogicHandler logic)
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

            var available_moves = moveOrder(logic.GetPositionFromBoardWhere(startingBoard, (path, arr) => arr.GetValue(path) is null, board.Dimension),currentPlayerMoves,oppositePlayerMoves,GameStatus.Dimensions,GameStatus.BoardSize);
            bool hasMoved = logic.GetPositionFromBoardWhere(startingBoard, (path, arr) => arr.GetValue(path) as char? == currentPlayer.Token, board.Dimension).Any();
            if (!available_moves.Any())
            {
                return null;
            }

            List<MinimaxResult> minimaxMoves = available_moves.Select((v, i) => new MinimaxResult()
            {
                DepthReached = 0,
                Score = -5000,
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
                    foreach (var move in minimaxMoves.OrderBy(i => i.Index))
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
                    forcedMove = minimaxMoves.Where(i => i.Score == maxScore).Count() == 1 || maxScore >= 300;
                    if (cancelToken.Token.IsCancellationRequested || forcedMove)
                    {
                        break;
                    }
                    
                    if (d == 1)
                    {
                        minimaxMoves = minimaxMoves.OrderBy(i => i.Score).ToList();
                        minimaxMoves.RemoveRange(0, minimaxMoves.Count - 15);
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

        public IEnumerable<List<int>> moveOrder(IEnumerable<List<int>> available_moves, IEnumerable<List<int>> currentPlayerMoves, IEnumerable<List<int>> oppositePlayerMoves, int dimension, int boardSize)
        {
            var currentPlayerMovesHash = logic.HashMoves(currentPlayerMoves);
            var oppositePlayerMovesHash = logic.HashMoves(oppositePlayerMoves);
            IEnumerable<MoveScore> moveScores = available_moves.Select(move => new MoveScore() { Move = move, Score = 0 }).ToArray();
            var vectors = logic.GetVectorsForDimension(dimension);
            foreach (var moveScore in moveScores)
            {
                int count = 0;
                foreach (var vector in vectors)
                {
                    var moveCheck = Enumerable.Range(-boardSize, 2 * boardSize).Select(n => logic.MultiplyThenAdd(moveScore.Move, n, vector));
                    var hashes = moveCheck.Where(i => logic.InBounds(i, boardSize)).Select(logic.GetHash);
                    count += hashes.Count(hash => currentPlayerMovesHash.Contains(hash) || oppositePlayerMovesHash.Contains(hash));
                }
                moveScore.Score = count;
            }

            
            return moveScores.OrderByDescending((moveScore) => moveScore.Score).Select(moveScore => moveScore.Move);
        }

        public int IterativeDeepening(ref List<TrackedMove> availableMoves, IEnumerable<List<int>> currentPlayerMoves, IEnumerable<List<int>> oppositePlayerMoves, 
            int depth, int maxDepth, TicTacToeGameStatus gameStatus, TicTacToePlayer currentPlayer, int alpha, int beta, TrackedMove firstMove = null)
        {
            bool maximizing = currentPlayer.PlayerId == gameStatus.CurrentPlayerId;

            int bestVal = maximizing ? defaultAlpha : defaultBeta;
            char currentToken = currentPlayer.Token;
            TicTacToePlayer oppositionPlayer = gameStatus.Players.Where(p => p.PlayerId != currentPlayer.PlayerId).First();
            int minmaxInt = maximizing ? winningScore : -winningScore;
            int score = logic.HasPlayerWon(currentPlayerMoves, gameStatus.Dimensions, gameStatus.BoardSize)
                ? minmaxInt : (logic.HasPlayerWon(oppositePlayerMoves, gameStatus.Dimensions, gameStatus.BoardSize) ? (minmaxInt * -1) : 0);

            if (score == winningScore)
            {
                return score - depth*100;
            }
            else if (score == -winningScore)
            {
                return score + depth * 100;
            }
            else if (depth >= maxDepth)
            {
                return (maximizing ? 1 : -1) * StaticEvaluationScore(currentPlayerMoves, oppositePlayerMoves, gameStatus.Dimensions, gameStatus.BoardSize);
            }
            else 
            {
                if (maximizing) // MAX
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

        private int StaticEvaluationScore(IEnumerable<List<int>> currentPlayerMoves, IEnumerable<List<int>> oppositePlayerMoves, int dimension, int boardSize)
        {
            var currentPlayerMovesHash = logic.HashMoves(currentPlayerMoves);
            var oppositePlayerMovesHash = logic.HashMoves(oppositePlayerMoves);
            var vectors = logic.GetVectorsForDimension(dimension);


            //From each move for the current player, check each vector to see if there is a winning path
            int playerPaths = currentPlayerMoves.Select(move =>
            {
                return vectors.Count(vector =>
                {
                    var moveCheck = Enumerable.Range(-boardSize, 2 * boardSize).Select(n => logic.MultiplyThenAdd(move, n, vector));

                    var hashes = moveCheck.Where(i => logic.InBounds(i, boardSize)).Select(logic.GetHash);

                    return !hashes.Any(oppositePlayerMovesHash.Contains) && (hashes.Count() == boardSize);
                });
            }).Sum();

            int count = 0;
            //From each move for the opposite player, check if there is a winning path
            int oppositePaths = oppositePlayerMoves.Select(move =>
            {
                return vectors.Count(vector =>
                {
                    var moveCheck = Enumerable.Range(-boardSize, 2 * boardSize).Select(n => logic.MultiplyThenAdd(move, n, vector));

                    var hashes = moveCheck.Where(i => logic.InBounds(i, boardSize)).Select(logic.GetHash);

                    count += hashes.Where(oppositePlayerMovesHash.Contains).Count() >= boardSize - 1 && !hashes.Any(currentPlayerMovesHash.Contains) ? 1 : 0;
                    return !hashes.Any(currentPlayerMovesHash.Contains) && (hashes.Count() == boardSize);
                });
            }).Sum();
            if (count >= 2)
            {
                oppositePaths += 100;
            }
            return playerPaths - oppositePaths;
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

    public class MoveScore
    {
        public List<int> Move { get; set; }
        public int Score { get; set; }
    }
}
