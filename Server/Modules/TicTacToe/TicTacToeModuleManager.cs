using NXO.Server.Dependencies;
using NXO.Shared;
using NXO.Shared.Models;
using NXO.Shared.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Modules
{
    public class TicTacToeModuleManager : IModuleManager
    {
        private readonly IGuidProvider guid;
        public TicTacToeModuleManager(IGuidProvider guid)
        {
            this.guid = guid;
        }
        public INXOModule Module => new TicTacToeModule();

        public Task<CreateLobbyResult> CreateLobbyAsync(Game game)
        {
            
        }

        public Task<T> GetGameStateAsync<T>(string LobbyCode)
        {
            throw new NotImplementedException();
        }

        public Task<MoveResult> PerformMoveAsync(IGameMove move)
        {
            throw new NotImplementedException();
        }

        public Task<SaveSettingsResult> SaveSettingsAsync(IGameSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
