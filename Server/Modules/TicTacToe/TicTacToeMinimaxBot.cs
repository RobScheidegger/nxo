using NXO.Shared.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NXO.Server.Modules.TicTacToe;

public class TicTacToeMinimaxBot : ITicTacToeBot
{
    private readonly TicTacToeGameLogicHandler logic;
    private readonly Random random = new();
    private const int timeout = 10;

    private const float POSITIVE_INFINITY = float.MaxValue;
    private const float NEGATIVE_INFINITY = -float.MaxValue;
    private const int MAX_DEPTH = 2;

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

        var available_moves = logic.GetPositionFromBoardWhere(startingBoard, (path, arr) => arr.GetValue(path) is null, board.Dimension);
        bool hasMoved = logic.GetPositionFromBoardWhere(startingBoard, (path, arr) => arr.GetValue(path) as char? == currentPlayer.Token, board.Dimension).Any();
        if (!available_moves.Any())
        {
            return null;
        }

        var trackedAvailableMoves = available_moves.Select(i => new TrackedMove()
            {
                Available = true,
                Move = i
            }).ToList();

        var cancelToken = new CancellationTokenSource();
        var task = Task.Run(() => Minimax(
                    availableMoves: ref trackedAvailableMoves,
                    currentPlayerMoves: currentPlayerMoves,
                    oppositePlayerMoves: oppositePlayerMoves,
                    depth: 0,
                    maxDepth: MAX_DEPTH,
                    gameStatus: GameStatus,
                    currentPlayer: currentPlayer,
                    alpha: NEGATIVE_INFINITY,
                    beta: POSITIVE_INFINITY), cancelToken.Token);

        bool isCompletedSuccessfully = task.Wait(TimeSpan.FromSeconds(timeout));
        var resultMove = task.Result;
        if (!isCompletedSuccessfully)
        {
            cancelToken.Cancel();
            resultMove = null; // TODO: Select random move
        }
        TicTacToeMove bestMove = new()
        {
            PlayerId = GameStatus.CurrentPlayerId,
            LobbyCode = GameStatus.LobbyCode,
            Path = resultMove.Move
        };
        return Task.FromResult(bestMove);
    }

    public MoveScore Minimax(ref List<TrackedMove> availableMoves, IEnumerable<List<int>> currentPlayerMoves, IEnumerable<List<int>> oppositePlayerMoves,
        int depth, int maxDepth, TicTacToeGameStatus gameStatus, TicTacToePlayer currentPlayer, float alpha, float beta)
    {
        bool maximizing = currentPlayer.PlayerId == gameStatus.CurrentPlayerId;
        if(logic.HasPlayerWon(currentPlayerMoves, gameStatus.Dimensions, gameStatus.BoardSize))
        {
            return new(maximizing ? 1 : -1, null);
        }
        else if (logic.HasPlayerWon(oppositePlayerMoves, gameStatus.Dimensions, gameStatus.BoardSize))
        {
            return new(maximizing ? -1 : 1, null);
        }
        if(depth >= maxDepth)
        {
            return new(StaticEvaluationScore(currentPlayerMoves, oppositePlayerMoves, gameStatus.Dimensions, gameStatus.BoardSize), null);
        }
        
        TicTacToePlayer oppositionPlayer = gameStatus.Players.Where(p => p.PlayerId != currentPlayer.PlayerId).First();

        MoveScore bestMove = new(maximizing ? NEGATIVE_INFINITY : POSITIVE_INFINITY, null);
        foreach (var move in availableMoves.Where(i => i.Available))
        {
            move.Available = false;
            var subtreeBest = Minimax(
                availableMoves: ref availableMoves,
                currentPlayerMoves: oppositePlayerMoves,
                oppositePlayerMoves: currentPlayerMoves.Append(move.Move),
                depth: depth + 1,
                maxDepth: maxDepth,
                gameStatus: gameStatus,
                currentPlayer: oppositionPlayer,
                alpha: alpha,
                beta: beta);
            move.Available = true;
            if (maximizing)
            {
                if(subtreeBest.Score > bestMove.Score)
                {
                    bestMove.Score = subtreeBest.Score;
                    bestMove.Move = move.Move;
                    alpha = Math.Max(alpha, bestMove.Score);
                }
                if(bestMove.Score >= beta)
                {
                    return bestMove;
                }
            }
            else
            {
                if (subtreeBest.Score < bestMove.Score)
                {
                    bestMove.Score = subtreeBest.Score;
                    bestMove.Move = move.Move;
                    beta = Math.Min(beta, bestMove.Score);
                }
                if (bestMove.Score <= alpha)
                {
                    return bestMove;
                }
            }
        }
        return bestMove;
    }

    private float StaticEvaluationScore(IEnumerable<List<int>> currentPlayerMoves, IEnumerable<List<int>> oppositePlayerMoves, int dimension, int boardSize)
    {
        var currentPlayerMovesHash = logic.HashMoves(currentPlayerMoves);
        var oppositePlayerMovesHash = logic.HashMoves(oppositePlayerMoves);
        var vectors = logic.GetVectorsForDimension(dimension);

        float currentPlayerScore = 0f;
        float oppositePlayerScore = 0f;
        //From each move for the current player, check each vector to see if there is a winning path
        foreach(var move in currentPlayerMoves)
        {
            foreach(var vector in vectors)
            {
                var moveCheck = Enumerable.Range(-boardSize, 2 * boardSize).Select(n => logic.MultiplyThenAdd(move, n, vector));
                var hashes = moveCheck.Where(i => logic.InBounds(i, boardSize)).Select(logic.GetHash);

                int currentPlayerPositions = hashes.Count(currentPlayerMovesHash.Contains);
                int oppositePlayerPositions = hashes.Count(oppositePlayerMovesHash.Contains);
                if(currentPlayerPositions > 0 && oppositePlayerPositions == 0)
                {
                    // Current player can win on this path
                    currentPlayerScore += currentPlayerPositions * currentPlayerPositions;
                }
                if(oppositePlayerPositions > 0 && currentPlayerPositions == 0)
                {
                    // Opposite player can win on this path.
                    oppositePlayerScore += oppositePlayerPositions * oppositePlayerPositions;
                }
            }
        }

        return currentPlayerScore / (currentPlayerScore + oppositePlayerScore + 1f);
    }
}
public class MinimaxResult
{
    public int DepthReached { get; set; }
    public List<int> Move { get; set; }
    public float Score { get; set; }
    public int Index { get; set; }
}
public class TrackedMove
{
    public bool Available { get; set; }
    public List<int> Move { get; set; }
}

public class MoveScore
{
    public MoveScore(float score, List<int> move)
    {
        Move = move;
        Score = score; 
    }
    public List<int> Move { get; set; }
    public float Score { get; set; }
}
