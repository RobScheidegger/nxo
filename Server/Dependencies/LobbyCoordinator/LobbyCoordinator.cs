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
        private readonly IGuidProvider guid;
        public LobbyCoordinator(IGameRepository gameRepository, IGuidProvider guid)
        {
            this.gameRepository = gameRepository;
            this.guid = guid;
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

        public async Task<CreateLobbyResult> CreateLobbyAsync(CreateLobbyRequest request)
        {
            var game = new Game()
            {
                DateCreated = DateTime.Now,
                LobbyCode = guid.NewLobbyCode(),
                GameType = request.GameType,
                Nickname = "New Lobby",
                Players = new Player[]
                {
                    new Player()
                    {
                        Id = guid.New(),
                        Nickname = request.Nickname
                    }
                },
                Stage = "Lobby",
                Settings = new GameSettings()
                {
                    MinimumPlayers = 2,
                    MaximumPlayers = 3
                }
            };
            await gameRepository.AddGameAsync(game);
            //Configure the game-specific portion using the 
            return await moduleManagers[request.GameType].CreateLobbyAsync(game);
        }

        public async Task<bool> HasLobbyStartedAsync(string LobbyCode)
        {
            var game = await gameRepository.GetGameAsync(LobbyCode);
            return game.Stage == "In Progress" || game.Stage == "Completed";
        }
        public async Task<JoinResult> JoinAsync(JoinRequest request)
        {
            var game = await gameRepository.GetGameAsync(request.GameCode);
            var newPlayer = new Player()
            {
                Id = guid.New(),
                Nickname = request.Nickname
            };
            game.Players = game.Players.Append(newPlayer);
            await gameRepository.UpdateGame(game);
            return new JoinResult()
            {
                GameType = game.GameType,
                Player = newPlayer,
                Success = true
            };
        }

        public async Task<bool> LobbyExistsAsync(string LobbyCode)
        {
            return await gameRepository.GameExistsAsync(LobbyCode);
        }

        public async Task<bool> SpotAvailableAsync(string LobbyCode)
        {
            var game = await gameRepository.GetGameAsync(LobbyCode);
            return game.Players.Count() < game.Settings.MaximumPlayers;
        }
    }
}
