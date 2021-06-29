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
                FindBlockingForkMove,
                FindCenterMove,
                FindOppositeCornerMove,
                FindEmptyCornerMove,
                FindEmptySideMove
            };
        }

        public string Type => "Static";

        public async Task<TicTacToeMove> GetNextMove(TicTacToeGameStatus GameStatus)
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
                    return foundMove;
                }
            }

            return new TicTacToeMove()
            {
                PlayerId = GameStatus.CurrentPlayerId,
                LobbyCode = GameStatus.LobbyCode,
                Path = available_moves.ElementAt(random.Next(available_moves.Count()))
            }; 
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
            return null;
        }
        public TicTacToeMove FindForkMove(IEnumerable<List<int>> available_moves, TicTacToeGameStatus GameStatus)
        {
            return null;
        }

        public TicTacToeMove FindBlockingForkMove(IEnumerable<List<int>> available_moves, TicTacToeGameStatus GameStatus)
        {
            return null;
        }

        public TicTacToeMove FindCenterMove(IEnumerable<List<int>> available_moves, TicTacToeGameStatus GameStatus)
        {
            return null;
        }

        public TicTacToeMove FindOppositeCornerMove(IEnumerable<List<int>> available_moves, TicTacToeGameStatus GameStatus)
        {
            return null;
        }

        public TicTacToeMove FindEmptyCornerMove(IEnumerable<List<int>> available_moves, TicTacToeGameStatus GameStatus)
        {
            return null;
        }

        public TicTacToeMove FindEmptySideMove(IEnumerable<List<int>> available_moves, TicTacToeGameStatus GameStatus)
        {
            return null;
        }
    }
}
