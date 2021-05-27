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
            char t = GameStatus.Players.Where(p => p.PlayerId == GameStatus.CurrentPlayerId).First().Token;
            var available_moves = logic.GetPositionFromBoardWhere(a, (path, arr) => arr.GetValue(path) is null, 2);
            if (!available_moves.Any())
            {
                return null;
            }
            foreach (var move in available_moves)
            {
                moves.Add(move);
                Array testBoard = logic.CloneBoard(a);
                testBoard.SetValue(t, move.ToArray());
                scores.Add(Minimax(a, 0, GameStatus.CurrentPlayerId, t));
            }
            TicTacToeMove bestMove = new()
            {
                PlayerId = GameStatus.CurrentPlayerId,
                LobbyCode = GameStatus.LobbyCode,
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
            var available_moves = logic.GetPositionFromBoardWhere(board, (path, arr) => arr.GetValue(path) is null, 2);
            if (!available_moves.Any())
            {
                return 0;
            }
            if (currentPlayer.Equals("bot")) // MAX
            {
                bestVal = -1000;
                foreach (var move in available_moves)
                {
                    Array testBoard = logic.CloneBoard(board);
                    testBoard.SetValue(currentToken, move.ToArray());
                    bestVal = Math.Max(bestVal,Minimax(testBoard, depth+1, currentPlayer, currentToken));
                }

            } else // MIN
            {
                bestVal = 1000;
                foreach (var move in available_moves)
                {
                    Array testBoard = logic.CloneBoard(board);
                    testBoard.SetValue(currentToken, move.ToArray());
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

                var available_moves = logic.GetPositionFromBoardWhere(board, (path, arr) => arr.GetValue(path) as char? == currentPlayer, 2);


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

        
    }
}
