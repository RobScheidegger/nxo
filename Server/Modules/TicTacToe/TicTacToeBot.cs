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
        private readonly Random random = new();
        private readonly int A = -1000;
        private readonly int B = 1000;

        public TicTacToeBot(TicTacToeGameLogicHandler logic)
        {
            this.logic = logic;
        }
        public Task<TicTacToeMove> GetNextMove(TicTacToeGameStatus GameStatus)
        {
            List<int> scores = new();
            List<List<int>> moves = new();
            TicTacToeBoard board = GameStatus.Board;
            Array a = logic.GetArrayFromBoard(board);
            TicTacToePlayer currentPlayer = GameStatus.Players.Where(p => p.PlayerId == GameStatus.CurrentPlayerId).First();
            TicTacToePlayer opp = GameStatus.Players.Where(p => p.PlayerId != currentPlayer.PlayerId).First();
            var available_moves = logic.GetPositionFromBoardWhere(a, (path, arr) => arr.GetValue(path) is null, board.Dimension);
            if (!available_moves.Any())
            {
                return null;
            }
            foreach (var move in available_moves)
            {
                moves.Add(move);
                Array testBoard = logic.CloneBoard(a);
                testBoard.SetValue(currentPlayer.Token, move.ToArray());
                scores.Add(Minimax(testBoard, 0, GameStatus, opp, A, B));
            }

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

        public int Minimax(Array board, int depth, TicTacToeGameStatus GameStatus, TicTacToePlayer currentPlayer, int alpha, int beta)
        {
            if (depth >= 10)
            {
                return 0;
            }
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
                bestVal = A;
                foreach (var move in available_moves)
                {
                    Array testBoard = logic.CloneBoard(board);
                    testBoard.SetValue(currentToken, move.ToArray());
                    bestVal = Math.Max(bestVal, Minimax(testBoard, depth + 1, GameStatus, opp, alpha, beta)); ;
                    alpha = Math.Max(alpha, bestVal);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }

            }
            else // MIN
            {
                bestVal = B;
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
    }
}
