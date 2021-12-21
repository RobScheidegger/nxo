using NXO.Shared.Modules;
using NXO.Shared.Modules.TicTacToe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Modules.TicTacToe
{
    public class TicTacToeStaticBot : ITicTacToeBot
    {
        private readonly TicTacToeGameLogicHandler logic;
        private readonly Random random = new();
        private readonly Func<HashSet<TicTacToePosition>, TicTacToeGameStatus, TicTacToeMove>[] bestMoveFinders;
        public TicTacToeStaticBot(TicTacToeGameLogicHandler logic)
        {
            this.logic = logic;
            this.bestMoveFinders = new Func<HashSet<TicTacToePosition>, TicTacToeGameStatus, TicTacToeMove>[]{
                FindWinningMove,
                FindBlockingMove,
                FindForkMove,
                FindBlockingForkMove
            };
        }

        public string Type => "Static";

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

            if (!available_moves.Any())
            {
                return null;
            }

            foreach(var function in bestMoveFinders)
            {
                var foundMove = function(available_moves, GameStatus);
                if (foundMove != null)
                {
                    return Task.FromResult(foundMove);
                }
            }

            return Task.FromResult(FindOptimalMove(available_moves, GameStatus));
        }

        public TicTacToeMove FindWinningMove(HashSet<TicTacToePosition> available_moves, TicTacToeGameStatus GameStatus)
        {
            TicTacToeBoard board = GameStatus.Board;
            Array startingBoard = logic.GetArrayFromBoard(board);
            TicTacToePlayer currentPlayer = GameStatus.Players.Where(p => p.PlayerId == GameStatus.CurrentPlayerId).First();
            TicTacToePlayer oppositePlayer = GameStatus.Players.Where(p => p.PlayerId != currentPlayer.PlayerId).First();

            var currentPlayerMoves = logic.GetPositionFromBoardWhere(startingBoard, (path, arr) =>
                arr.GetValue(path) as char? == currentPlayer.Token, board.Dimension);
            var oppositePlayerMoves = logic.GetPositionFromBoardWhere(startingBoard, (path, arr) =>
                arr.GetValue(path) as char? == oppositePlayer.Token, board.Dimension);
            foreach (var move in available_moves)
            {
                currentPlayerMoves.Add(move);
                if (logic.HasPlayerWon(currentPlayerMoves, GameStatus.Dimensions, GameStatus.BoardSize))
                {
                    return new TicTacToeMove()
                    {
                        PlayerId = GameStatus.CurrentPlayerId,
                        LobbyCode = GameStatus.LobbyCode,
                        Path = move.Location
                    };
                }
                currentPlayerMoves.Remove(move);
            }

            return null;
        }

        public TicTacToeMove FindBlockingMove(HashSet<TicTacToePosition> available_moves, TicTacToeGameStatus GameStatus)
        {
            TicTacToeBoard board = GameStatus.Board;
            Array startingBoard = logic.GetArrayFromBoard(board);
            TicTacToePlayer currentPlayer = GameStatus.Players.Where(p => p.PlayerId == GameStatus.CurrentPlayerId).First();
            TicTacToePlayer oppositePlayer = GameStatus.Players.Where(p => p.PlayerId != currentPlayer.PlayerId).First();

            var currentPlayerMoves = logic.GetPositionFromBoardWhere(startingBoard, (path, arr) =>
                arr.GetValue(path) as char? == currentPlayer.Token, board.Dimension);
            var oppositePlayerMoves = logic.GetPositionFromBoardWhere(startingBoard, (path, arr) =>
                arr.GetValue(path) as char? == oppositePlayer.Token, board.Dimension);
            foreach (var move in available_moves)
            {
                oppositePlayerMoves.Add(move);
                if (logic.HasPlayerWon(oppositePlayerMoves, GameStatus.Dimensions, GameStatus.BoardSize))
                {
                    return new TicTacToeMove()
                    {
                        PlayerId = GameStatus.CurrentPlayerId,
                        LobbyCode = GameStatus.LobbyCode,
                        Path = move.Location
                    };
                }
                oppositePlayerMoves.Remove(move);
            }

            return null;
        }
        public TicTacToeMove FindForkMove(HashSet<TicTacToePosition> available_moves, TicTacToeGameStatus GameStatus)
        {
            TicTacToeBoard board = GameStatus.Board;
            Array startingBoard = logic.GetArrayFromBoard(board);
            TicTacToePlayer currentPlayer = GameStatus.Players.Where(p => p.PlayerId == GameStatus.CurrentPlayerId).First();
            TicTacToePlayer oppositePlayer = GameStatus.Players.Where(p => p.PlayerId != currentPlayer.PlayerId).First();

            var currentPlayerMoves = logic.GetPositionFromBoardWhere(startingBoard, (path, arr) =>
                arr.GetValue(path) as char? == currentPlayer.Token, board.Dimension);
            var oppositePlayerMoves = logic.GetPositionFromBoardWhere(startingBoard, (path, arr) =>
                arr.GetValue(path) as char? == oppositePlayer.Token, board.Dimension);

            var boardSize = GameStatus.BoardSize;
            var dimension = GameStatus.Dimensions;

            foreach (var move in available_moves)
            {
                var vectors = logic.GetVectorsForDimension(dimension);
                int count = 0;
                currentPlayerMoves.Add(move);
                foreach (var winMove in available_moves)
                {
                    currentPlayerMoves.Add(winMove);
                    if (logic.HasPlayerWon(currentPlayerMoves, dimension, boardSize))
                    {
                        count++;
                    }
                    currentPlayerMoves.Remove(winMove);
                }
                /*int playerPaths = currentPlayerMoves.Select(move =>
                {
                    return vectors.Count(vector =>
                    {
                        var moveCheck = Enumerable.Range(-boardSize, 2 * boardSize).Select(n => logic.MultiplyThenAdd(move, n, vector));

                        var hashes = moveCheck.Where(i => logic.InBounds(i, boardSize)).Select(logic.GetHash);
                        return hashes.Where(currentPlayerMovesHash.Contains).Count() >= boardSize - 1 && !hashes.Any(oppositePlayerMovesHash.Contains);
                    });
                }).Sum();
                */
                if (count >= 2)
                {
                    return new TicTacToeMove()
                    {
                        PlayerId = GameStatus.CurrentPlayerId,
                        LobbyCode = GameStatus.LobbyCode,
                        Path = move.Location
                    };
                }
                currentPlayerMoves.Remove(move);


            }
            
            return null;
        }

        public TicTacToeMove FindBlockingForkMove(HashSet<TicTacToePosition> available_moves, TicTacToeGameStatus GameStatus)
        {
            TicTacToeBoard board = GameStatus.Board;
            Array startingBoard = logic.GetArrayFromBoard(board);
            TicTacToePlayer currentPlayer = GameStatus.Players.Where(p => p.PlayerId == GameStatus.CurrentPlayerId).First();
            TicTacToePlayer oppositePlayer = GameStatus.Players.Where(p => p.PlayerId != currentPlayer.PlayerId).First();

            var currentPlayerMoves = logic.GetPositionFromBoardWhere(startingBoard, (path, arr) =>
                arr.GetValue(path) as char? == currentPlayer.Token, board.Dimension);
            var oppositePlayerMoves = logic.GetPositionFromBoardWhere(startingBoard, (path, arr) =>
                arr.GetValue(path) as char? == oppositePlayer.Token, board.Dimension);

            var boardSize = GameStatus.BoardSize;
            var dimension = GameStatus.Dimensions;

            foreach (var move in available_moves)
            {
                var vectors = logic.GetVectorsForDimension(dimension);
                int count = 0;
                oppositePlayerMoves.Add(move);
                foreach (var winMove in available_moves)
                {
                    oppositePlayerMoves.Add(winMove);
                    if (logic.HasPlayerWon(oppositePlayerMoves, dimension, boardSize))
                    {
                        count++;
                    }
                    oppositePlayerMoves.Remove(winMove);
                }
                /*
                int playerPaths = oppositePlayerMoves.Select(move =>
                {
                    return vectors.Count(vector =>
                    {
                        var moveCheck = Enumerable.Range(-boardSize, 2 * boardSize).Select(n => logic.MultiplyThenAdd(move, n, vector));

                        var hashes = moveCheck.Where(i => logic.InBounds(i, boardSize)).Select(logic.GetHash);
                        return hashes.Where(oppositePlayerMovesHash.Contains).Count() >= boardSize - 1 && !hashes.Any(currentPlayerMovesHash.Contains);
                    });
                }).Sum();
                */
                oppositePlayerMoves.Remove(move);
                if (count >= 2)
                {
                    return new TicTacToeMove()
                    {
                        PlayerId = GameStatus.CurrentPlayerId,
                        LobbyCode = GameStatus.LobbyCode,
                        Path = move.Location
                    };
                }
            }

            return null;
        }

        public TicTacToeMove FindOptimalMove(HashSet<TicTacToePosition> available_moves, TicTacToeGameStatus GameStatus)
        {
            TicTacToeBoard board = GameStatus.Board;
            Array startingBoard = logic.GetArrayFromBoard(board);
            TicTacToePlayer currentPlayer = GameStatus.Players.Where(p => p.PlayerId == GameStatus.CurrentPlayerId).First();
            TicTacToePlayer oppositePlayer = GameStatus.Players.Where(p => p.PlayerId != currentPlayer.PlayerId).First();

            var currentPlayerMoves = logic.GetPositionFromBoardWhere(startingBoard, (path, arr) =>
                arr.GetValue(path) as char? == currentPlayer.Token, board.Dimension);
            var oppositePlayerMoves = logic.GetPositionFromBoardWhere(startingBoard, (path, arr) =>
                arr.GetValue(path) as char? == oppositePlayer.Token, board.Dimension);

            var boardSize = GameStatus.BoardSize;
            var dimension = GameStatus.Dimensions;


            List<StaticEvalResult> evalMoves = available_moves.Select((v, i) => new StaticEvalResult()
            {
                Score = -5000,
                Move = v,
                Index = i
            }).ToList();

            foreach (var move in evalMoves)
            {
                currentPlayerMoves.Add(move.Move);
                move.Score = StaticEvaluationScore(currentPlayerMoves, oppositePlayerMoves, dimension, boardSize);
                currentPlayerMoves.Remove(move.Move);
            }

            float scoreMax = evalMoves.Max((i) => i.Score);
            return new TicTacToeMove()
            {
                PlayerId = GameStatus.CurrentPlayerId,
                LobbyCode = GameStatus.LobbyCode,
                Path = evalMoves.First((i) => i.Score == scoreMax).Move.Location
            };
           
        }

        private float StaticEvaluationScore(HashSet<TicTacToePosition> currentPlayerMoves, HashSet<TicTacToePosition> oppositePlayerMoves, int dimension, int boardSize)
        {
            var vectors = logic.GetVectorsForDimension(dimension);

            float currentPlayerScore = 0f;
            float oppositePlayerScore = 0f;
            //From each move for the current player, check each vector to see if there is a winning path
            foreach (var move in currentPlayerMoves)
            {
                foreach (var vector in vectors)
                {
                    var moveCheck = logic.GetSpanVectors(boardSize, move, vector);

                    int currentPlayerPositions = moveCheck.Count(currentPlayerMoves.Contains);
                    int oppositePlayerPositions = moveCheck.Count(oppositePlayerMoves.Contains);
                    if (currentPlayerPositions > 0 && oppositePlayerPositions == 0)
                    {
                        // Current player can win on this path
                        currentPlayerScore += currentPlayerPositions * currentPlayerPositions;
                    }
                    if (oppositePlayerPositions > 0 && currentPlayerPositions == 0)
                    {
                        // Opposite player can win on this path.
                        oppositePlayerScore += oppositePlayerPositions * oppositePlayerPositions;
                    }
                }
            }

            return currentPlayerScore / (currentPlayerScore + oppositePlayerScore + 1f);
        }
    }

    public class StaticEvalResult
    {
        public TicTacToePosition Move { get; set; }
        public float Score { get; set; }
        public int Index { get; set; }
    }
}
