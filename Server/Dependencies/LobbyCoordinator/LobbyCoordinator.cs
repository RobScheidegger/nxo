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
        private readonly IRepository<Game> gameRepository;
        private readonly Dictionary<string, IModuleManager> modules;
        private readonly IGuidProvider guid;
        public LobbyCoordinator(IRepository<Game> gameRepository, IGuidProvider guid, IEnumerable<IModuleManager> modules)
        {
            this.gameRepository = gameRepository;
            this.guid = guid;
            this.modules = modules.ToDictionary(i => i.GameType, i => i);
        }
        public async Task<JoinResult> AttemptJoinAsync(JoinRequest request)
        {
            if(!(await LobbyExistsAsync(request.GameCode)))
            {
                return new JoinResult()
                {
                    Success = false,
                    RejectMessage = $"Lobby with code '{request.GameCode}' does not exist." 
                };
            }
            else if(await HasLobbyStartedAsync(request.GameCode))
            {
                return new JoinResult()
                {
                    Success = false,
                    RejectMessage = $"Lobby '{request.GameCode}' has already begun."
                };
            }
            else if (!(await SpotAvailableAsync(request.GameCode)))
            {
                return new JoinResult()
                {
                    Success = false,
                    RejectMessage = $"No spot is available in the lobby '{request.GameCode}'"
                };
            }
            else
            {
                return await JoinAsync(request);
            }
        }

        public async Task<CreateLobbyResult> CreateLobbyAsync(CreateLobbyRequest request)
        {
            var player = new Player()
            {
                Id = guid.New(),
                Nickname = request.Nickname
            };
            var game = new Game()
            {
                DateCreated = DateTime.Now,
                LobbyCode = guid.NewLobbyCode(),
                GameType = request.GameType,
                Nickname = "New Lobby",
                Players = new Player[]
                {
                    player
                },
                Stage = "Lobby",
                Settings = new GameSettings()
                {
                    MinimumPlayers = 2,
                    MaximumPlayers = 3
                },
                HostPlayerId = player.Id
            };
            await gameRepository.Add(game.LobbyCode, game);
            //Configure the game-specific portion using the 
            await modules[request.GameType].CreateLobbyAsync(game);
            return new CreateLobbyResult()
            {
                LobbyCode = game.LobbyCode,
                Message = "Game created successfully",
                PlayerId = player.Id,
                Success = true
            };
        }

        public async Task<bool> HasLobbyStartedAsync(string LobbyCode)
        {
            var game = await gameRepository.Find(LobbyCode);
            return game.Stage == "In Progress" || game.Stage == "Completed";
        }
        public async Task<JoinResult> JoinAsync(JoinRequest request)
        {
            var game = await gameRepository.Find(request.GameCode);
            var newPlayer = new Player()
            {
                Id = guid.New(),
                Nickname = request.Nickname
            };
            await gameRepository.Update(request.GameCode, g => g.Players = g.Players.Append(newPlayer));
            return new JoinResult()
            {
                GameType = game.GameType,
                Player = newPlayer,
                Success = true
            };
        }

        public async Task<bool> LobbyExistsAsync(string LobbyCode)
        {
            return LobbyCode != null && await gameRepository.Exists(LobbyCode);
        }

        public async Task<bool> SpotAvailableAsync(string LobbyCode)
        {
            var game = await gameRepository.Find(LobbyCode);
            return game.Players.Count() < game.Settings.MaximumPlayers;
        }
    }
}
