using NXO.Shared.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Modules.TicTacToe
{
    public class TicTacToeBot
    {
        private readonly TicTacToeGameLogicHandler logic;
        public TicTacToeBot(TicTacToeGameLogicHandler logic)
        {
            this.logic = logic;
        }
        public async Task<TicTacToeMove> GetNextMove(char MoveIdentifier, TicTacToeGameStatus GameStatus)
        {
            List<int> scores = new();
            List<List<int>> moves = new();
            TicTacToeBoard board = GameStatus.Board;
            Array a = logic.GetArrayFromBoard(board);
            TicTacToePlayer currentPlayer = GameStatus.Players.Where(p => p.PlayerId == GameStatus.CurrentPlayerId).First();
            var available_moves = logic.GetPositionFromBoardWhere(a, (path, arr) => arr.GetValue(path) is null, 2);
            if (!available_moves.Any())
            {
                return null;
            }
            foreach (var move in available_moves)
            {
                moves.Add(move);
                Array testBoard = logic.CloneBoard(a);
                testBoard.SetValue(currentPlayer.Token, move.ToArray());
                TicTacToePlayer opp = GameStatus.Players.Where(p => p.PlayerId != currentPlayer.PlayerId).First();
                scores.Add(Minimax(testBoard, 0, GameStatus, opp));
            }
            TicTacToeMove bestMove = new()
            {
                PlayerId = GameStatus.CurrentPlayerId,
                LobbyCode = GameStatus.LobbyCode,
                Path = moves[scores.IndexOf(scores.Max())]
            };
            return bestMove;
        }

        public int Minimax(Array board, int depth, TicTacToeGameStatus GameStatus, TicTacToePlayer currentPlayer)
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
            var available_moves = logic.GetPositionFromBoardWhere(board, (path, arr) => arr.GetValue(path) is null, 2);
            if (!available_moves.Any())
            {
                return 0;
            }

            if (currentPlayer.Bot) // MAX
            {
                bestVal = -1000;
                foreach (var move in available_moves)
                {
                    Array testBoard = logic.CloneBoard(board);
                    testBoard.SetValue(currentToken, move.ToArray());
                    bestVal = Math.Max(bestVal, Minimax(testBoard, depth + 1, GameStatus, opp));
                }

            }
            else // MIN
            {
                bestVal = 1000;
                foreach (var move in available_moves)
                {
                    Array testBoard = logic.CloneBoard(board);
                    testBoard.SetValue(currentToken, move.ToArray());
                    bestVal = Math.Min(bestVal, Minimax(testBoard, depth + 1, GameStatus, opp));
                }
            }
            return bestVal;
        }
    }
}
