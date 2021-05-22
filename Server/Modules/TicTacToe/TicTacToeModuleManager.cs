using NXO.Server.Dependencies;
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
        private readonly IRepository<TicTacToeSettings> settingsRepository;
        private readonly IRepository<TicTacToeGameStatus> gameStatusRepository;
        private readonly IRepository<Game> gameRepository;
        public TicTacToeModuleManager(IGuidProvider guid, IRepository<TicTacToeSettings> settings, IRepository<Game> gameRepository, IRepository<TicTacToeGameStatus> gameStatus)
        {
            this.guid = guid;
            this.settingsRepository = settings;
            this.gameRepository = gameRepository;
            this.gameStatusRepository = gameStatus;
        }
        public string GameType => "tictactoe";

        public async Task<bool> CreateLobbyAsync(Game game)
        {
            var setting = new TicTacToeSettings()
            { 
                BoardSize = 3,
                Dimensions = 2,
                MaximumPlayers = 2
            };
            await settingsRepository.Add(game.LobbyCode, setting);
            return true;
        }
        public async Task<LobbyStatusResult<T>> GetLobbyStatus<T>(LobbyStatusRequest request) where T : class, IGameSettings
        {
            var game = await gameRepository.Find(request.LobbyCode);
            var settings = await GetSettings(request.LobbyCode);
            return new LobbyStatusResult<T>()
            {
                Game = game,
                Settings = settings as T
            };
        }
        public  async Task<IGameStatus> GetGameStateAsync(string LobbyCode)
        {
            var status = await gameStatusRepository.Find(LobbyCode);
            return status;
        }

        public async Task<IGameSettings> GetSettings(string LobbyCode)
        {
            return await settingsRepository.Find(LobbyCode);
        }

        public Task<MoveResult> PerformMoveAsync(IGameMove move)
        {
            var properMove = move;
            throw new NotImplementedException();
        }

        public async Task<SaveSettingsResult> SaveSettingsAsync(IGameSettings settings)
        {
            var properSettings = settings as TicTacToeSettings;
            await settingsRepository.Update(properSettings.LobbyCode, s =>
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

        public async Task StartGame(string LobbyCode)
        {
            var gameSettings = await settingsRepository.Find(LobbyCode);

            var game = await gameRepository.Find(LobbyCode);

            var playerCount = game.Players.Count();
            var rand = new Random();

            var startingPlayer = game.Players.ElementAt(rand.Next(0, playerCount));
            var lengths = Enumerable.Range(0, gameSettings.Dimensions).Select(i => gameSettings.BoardSize);

            var gameStatus = new TicTacToeGameStatus()
            {
                CurrentPlayerId = startingPlayer.Id,
                CurrentPlayerName = startingPlayer.Nickname,
                Board = TicTacToeBoard.Construct(gameSettings.Dimensions, gameSettings.BoardSize)
            };

            gameStatus.Board.Boards.Append(null);

            await gameStatusRepository.Add(LobbyCode, gameStatus);
        }
    }
}
