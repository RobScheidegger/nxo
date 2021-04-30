using NXO.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Dependencies
{
    public class LobbyCoordinator : ILobbyCoordinator
    {
        private readonly IGameRepository gameRepository;
        private readonly Dictionary<string, IModuleManager> moduleManagers;
        public LobbyCoordinator(IGameRepository gameRepository)
        {
            this.gameRepository = gameRepository;
        }
        public async Task<JoinResult> AttemptJoinAsync(JoinRequest request)
        {
            if(!(await LobbyExistsAsync(request.GameCode)))
            {
                return new JoinResult()
                {
                    Success = false,
                    RejectMessage = $"Lobby with code {request.GameCode} does not exist." 
                };
            }
            else if(await HasLobbyStartedAsync(request.GameCode))
            {
                return new JoinResult()
                {
                    Success = false,
                    RejectMessage = $"Lobby {request.GameCode} has already begun."
                };
            }
            else if (!(await SpotAvailableAsync(request.GameCode)))
            {
                return new JoinResult()
                {
                    Success = false,
                    RejectMessage = $"No spot is available in the lobby {request.GameCode}"
                };
            }
            else
            {
                return await JoinAsync(request);
            }
        }

        public Task<CreateLobbyResult> CreateLobbyAsync(CreateLobbyRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasLobbyStartedAsync(string LobbyCode)
        {
            throw new NotImplementedException();
        }
        public Task<JoinResult> JoinAsync(JoinRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LobbyExistsAsync(string LobbyCode)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SpotAvailableAsync(string LobbyCode)
        {
            throw new NotImplementedException();
        }
    }
}
