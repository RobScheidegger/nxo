using NXO.Server.Dependencies;
using NXO.Server.Modules.TicTacToe;
using NXO.Shared;
using NXO.Shared.Models;
using NXO.Shared.Modules;
using NXO.Shared.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Modules
{
    public class TicTacToeModuleManager : IModuleManager
    {
        private readonly IGuidProvider guid;
        private readonly IRepository<TicTacToeGameStatus> gameStatusRepository;
        private readonly TicTacToeBot bot;
        private readonly TicTacToeGameLogicHandler logic;
        public TicTacToeModuleManager(IGuidProvider guid,
            IRepository<TicTacToeGameStatus> gameStatus, TicTacToeGameLogicHandler logic, TicTacToeBot bot)
        {
            this.guid = guid;
            this.gameStatusRepository = gameStatus;
            this.logic = logic;
            this.bot = bot;
        }
        public string GameType => "tictactoe";
        private char[] Tokens = { 'x', 'o', 'q'};
        private int TokenIndex = 0;
        private char GetNextToken()
        {
            var token = Tokens[TokenIndex % Tokens.Length];
            TokenIndex++;
            return token;
        }
        public async Task<IGameStatus> CreateLobbyAsync(CreateLobbyRequest request)
        {
            var player = new TicTacToePlayer()
            {
                PlayerId = guid.New(),
                Nickname = request.Nickname
            };
            var tictactoeGame = new TicTacToeGameStatus()
            {
                DateCreated = DateTime.Now,
                LobbyCode = guid.NewLobbyCode(),
                GameType = request.GameType,
                Nickname = "New Lobby",
                Players = new TicTacToePlayer[]
                {
                    player
                },
                Stage = "Lobby",
                HostPlayerId = player.PlayerId,
                BoardSize = 3,
                Dimensions = 2,
                MaximumPlayers = 2
            };
            await gameStatusRepository.Add(tictactoeGame.LobbyCode, tictactoeGame);
            return tictactoeGame;
        }
        public  async Task<IGameStatus> GetGameStateAsync(string LobbyCode)
        {
            var status = await gameStatusRepository.Find(LobbyCode);
            return status;
        }

        public async Task<MoveResult> PerformMoveAsync(IGameMove move)
        {
            var properMove = move as TicTacToeMove;
            var gameStatus = await gameStatusRepository.Find(move.LobbyCode);
            if(gameStatus.Completed)
            {
                return new MoveResult()
                {
                    Success = false,
                    Message = "Cannot place move, game is over."
                };
            }
            if(IsValidMove(properMove, gameStatus))
            {
                var currentPlayerIndex = gameStatus.Players.Select(i => i.PlayerId).ToList().IndexOf(gameStatus.CurrentPlayerId);
                var currentPlayer = gameStatus.Players.ElementAt(currentPlayerIndex);
                var nextPlayerIndex = (currentPlayerIndex + 1) % gameStatus.Players.Count();
                var nextPlayer = gameStatus.Players.ElementAt(nextPlayerIndex);
                var moveMessage = $"{currentPlayer.Nickname} placed a '{currentPlayer.Token}' at '{string.Join(',', properMove.Path.Select(i => i + 1))}'";

                await gameStatusRepository.Update(move.LobbyCode, g =>
                {
                    g.Board.Place(currentPlayer.Token, properMove.Path);
                    g.CurrentPlayerId = nextPlayer.PlayerId;
                    g.CurrentPlayerName = nextPlayer.Nickname;
                    g.History.Add(new TicTacToeGameHistoryEntry()
                    {
                        Message = moveMessage,
                        PlayerName = currentPlayer.Nickname,
                        MovePath = properMove.Path
                    });
                });
                var updatedGame = await gameStatusRepository.Find(move.LobbyCode);
                if (logic.HasPlayerWon(currentPlayer.Token, updatedGame.Board))
                {
                    await CompleteGame(move.LobbyCode, currentPlayer);
                }
                return new MoveResult()
                {
                    Success = true,
                    Message = moveMessage
                };
            }
            else
            {
                return new MoveResult()
                {
                    Success = false,
                    Message = "Move was not valid."
                };
            }
        }

        private async Task CompleteGame(string lobbyCode, TicTacToePlayer winningPlayer)
        {
            await gameStatusRepository.Update(lobbyCode, game =>
            {
                game.Winner = winningPlayer;
                game.Completed = true;
            });
        }

        private static bool IsValidMove(TicTacToeMove properMove, TicTacToeGameStatus gameStatus)
        {
            return gameStatus.Board.GetForPosition(properMove.Path) == null
                && properMove.PlayerId == gameStatus.CurrentPlayerId;
        }

        public async Task<SaveSettingsResult> SaveSettingsAsync(IGameStatus settings)
        {
            var properSettings = settings as TicTacToeGameStatus;
            await gameStatusRepository.Update(properSettings.LobbyCode, s =>
            {
                s.BoardSize = properSettings.BoardSize;
                s.Dimensions = properSettings.Dimensions;
            });
            return new SaveSettingsResult()
            {
                Message = "Saved",
                Success = true
            };
        }

        public async Task<StartGameResult> StartGame(string LobbyCode)
        {
            var rand = new Random();
            await gameStatusRepository.Update(LobbyCode, g =>
            {
                var startingPlayer = g.Players.ElementAt(rand.Next(0, g.PlayerCount));
                g.CurrentPlayerId = startingPlayer.PlayerId;
                g.CurrentPlayerName = startingPlayer.Nickname;
                g.Board = TicTacToeBoard.Construct(g.Dimensions, g.BoardSize);
                g.History = new List<TicTacToeGameHistoryEntry>();
            });
            return new StartGameResult() { Success = true };
        }

        public async Task<bool> HasLobbyStartedAsync(string LobbyCode)
        {
            var game = await gameStatusRepository.Find(LobbyCode);
            return game.Stage == "In Progress" || game.Stage == "Completed";
        }
        public async Task<JoinResult> JoinAsync(JoinRequest request)
        {
            var game = await gameStatusRepository.Find(request.GameCode);
            var newPlayer = new TicTacToePlayer()
            {
                PlayerId = guid.New(),
                Nickname = request.Nickname
            };
            await gameStatusRepository.Update(request.GameCode, g => g.Players = g.Players.Append(newPlayer));
            return new JoinResult()
            {
                GameType = game.GameType,
                PlayerId = newPlayer.PlayerId,
                Success = true
            };
        }

        public async Task<bool> LobbyExistsAsync(string LobbyCode)
        {
            return LobbyCode != null && await gameStatusRepository.Exists(LobbyCode);
        }

        public async Task<bool> SpotAvailableAsync(string LobbyCode)
        {
            var game = await gameStatusRepository.Find(LobbyCode);
            return game != null && game.PlayerCount < game.MaximumPlayers;
        }
    }
}
