using NXO.Shared;
using NXO.Shared.Models;
using NXO.Shared.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Dependencies
{
    public class LobbyCoordinator : ILobbyCoordinator
    {
        private readonly Dictionary<string, IModuleManager> modules;
        private readonly IGuidProvider guid;
        public LobbyCoordinator(IGuidProvider guid, IEnumerable<IModuleManager> modules)
        {
            this.guid = guid;
            this.modules = modules.ToDictionary(i => i.GameType, i => i);
        }
        public async Task<JoinResult> AttemptJoinAsync(JoinRequest request)
        {
            var module = await FindModule(request.GameCode);
            if(module == null)
            {
                return new JoinResult()
                {
                    Success = false,
                    RejectMessage = $"Lobby with code '{request.GameCode}' does not exist." 
                };
            }
            else if(await module.HasLobbyStartedAsync(request.GameCode))
            {
                return new JoinResult()
                {
                    Success = false,
                    RejectMessage = $"Lobby '{request.GameCode}' has already begun."
                };
            }
            else if (!(await module.SpotAvailableAsync(request.GameCode)))
            {
                return new JoinResult()
                {
                    Success = false,
                    RejectMessage = $"No spot is available in the lobby '{request.GameCode}'"
                };
            }
            else
            {
                return await module.JoinAsync(request);
            }
        }

        private async Task<IModuleManager> FindModule(string gameCode)
        {
            foreach(var module in modules)
            {
                if (await module.Value.GetGameStateAsync(gameCode) != null)
                    return module.Value;
            }
            return null;
        }

        public async Task<CreateLobbyResult> CreateLobbyAsync(CreateLobbyRequest request)
        {
            if (string.IsNullOrEmpty(request.Nickname))
            {
                return new CreateLobbyResult()
                {
                    Message = "Player nickname cannot be empty",
                    Success = false
                };
            }
            var game = await modules[request.GameType].CreateLobbyAsync(request);
            return new CreateLobbyResult()
            {
                LobbyCode = game.LobbyCode,
                Message = "Game created successfully",
                PlayerId = game.HostPlayerId,
                Success = true
            };
        }
    }
}
