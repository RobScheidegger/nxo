using NXO.Shared.Modules;
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
        private readonly Func<IEnumerable<List<int>>, TicTacToeGameStatus, TicTacToeMove>[] bestMoveFinders;
        public TicTacToeStaticBot(TicTacToeGameLogicHandler logic)
        {
            this.logic = logic;
            this.bestMoveFinders = new Func<IEnumerable<List<int>>, TicTacToeGameStatus, TicTacToeMove>[]{
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

        public TicTacToeMove FindWinningMove(IEnumerable<List<int>> available_moves, TicTacToeGameStatus GameStatus)
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
                if (logic.HasPlayerWon(currentPlayerMoves.Append(move), GameStatus.Dimensions, GameStatus.BoardSize))
                {
                    return new TicTacToeMove()
                    {
                        PlayerId = GameStatus.CurrentPlayerId,
                        LobbyCode = GameStatus.LobbyCode,
                        Path = move
                    };
                }
            }

            return null;
        }

        public TicTacToeMove FindBlockingMove(IEnumerable<List<int>> available_moves, TicTacToeGameStatus GameStatus)
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
                if (logic.HasPlayerWon(oppositePlayerMoves.Append(move), GameStatus.Dimensions, GameStatus.BoardSize))
                {
                    return new TicTacToeMove()
                    {
                        PlayerId = GameStatus.CurrentPlayerId,
                        LobbyCode = GameStatus.LobbyCode,
                        Path = move
                    };
                }
            }

            return null;
        }
        public TicTacToeMove FindForkMove(IEnumerable<List<int>> available_moves, TicTacToeGameStatus GameStatus)
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
                var currentPlayerMovesHash = logic.HashMoves(currentPlayerMoves.Append(move));
                var oppositePlayerMovesHash = logic.HashMoves(oppositePlayerMoves);
                var vectors = logic.GetVectorsForDimension(dimension);
                int count = 0;
                foreach (var winMove in available_moves)
                {
                    if (logic.HasPlayerWon(currentPlayerMoves.Append(move).Append(winMove), dimension, boardSize))
                    {
                        count++;
                    }
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
                        Path = move
                    };
                }


            }
            
            return null;
        }

        public TicTacToeMove FindBlockingForkMove(IEnumerable<List<int>> available_moves, TicTacToeGameStatus GameStatus)
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
                var currentPlayerMovesHash = logic.HashMoves(currentPlayerMoves);
                var oppositePlayerMovesHash = logic.HashMoves(oppositePlayerMoves.Append(move));
                var vectors = logic.GetVectorsForDimension(dimension);
                int count = 0;
                foreach (var winMove in available_moves)
                {
                    if (logic.HasPlayerWon(oppositePlayerMoves.Append(move).Append(winMove), dimension, boardSize))
                    {
                        count++;
                    }
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
                if (count >= 2)
                {
                    return new TicTacToeMove()
                    {
                        PlayerId = GameStatus.CurrentPlayerId,
                        LobbyCode = GameStatus.LobbyCode,
                        Path = move
                    };
                }
            }

            return null;
        }

        public TicTacToeMove FindOptimalMove(IEnumerable<List<int>> available_moves, TicTacToeGameStatus GameStatus)
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
                move.Score = StaticEvaluationScore(currentPlayerMoves.Append(move.Move), oppositePlayerMoves, dimension, boardSize);
            }

            int scoreMax = evalMoves.Max((i) => i.Score);
            return new TicTacToeMove()
            {
                PlayerId = GameStatus.CurrentPlayerId,
                LobbyCode = GameStatus.LobbyCode,
                Path = evalMoves.First((i) => i.Score == scoreMax).Move
            };
           
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

    public class StaticEvalResult
    {
        public List<int> Move { get; set; }
        public int Score { get; set; }
        public int Index { get; set; }
    }
}
