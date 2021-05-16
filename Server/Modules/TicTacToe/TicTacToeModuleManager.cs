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
        private readonly IRepository<TicTacToeSettings> settings;
        private readonly IRepository<Game> gameRepository;
        public TicTacToeModuleManager(IGuidProvider guid, IRepository<TicTacToeSettings> settings, IRepository<Game> gameRepository)
        {
            this.guid = guid;
            this.settings = settings;
            this.gameRepository = gameRepository;
        }
        public string GameType => "tictactoe";

        public async Task<bool> CreateLobbyAsync(Game game)
        {
            var setting = new TicTacToeSettings()
            { 
                BoardSize = 3,
                Dimensions = 2
            };
            await settings.Add(game.LobbyCode, setting);
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
        public Task<T> GetGameStateAsync<T>(string LobbyCode)
        {
            throw new NotImplementedException();
        }

        public async Task<IGameSettings> GetSettings(string LobbyCode)
        {
            return await settings.Find(LobbyCode);
        }

        public Task<MoveResult> PerformMoveAsync(IGameMove move)
        {
            throw new NotImplementedException();
        }

        public Task<SaveSettingsResult> SaveSettingsAsync(IGameSettings settings)
        {
            var properSettings = settings as TicTacToeSettings;
            throw new NotImplementedException();
        }
    }
}
