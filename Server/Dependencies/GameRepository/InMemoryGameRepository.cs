using NXO.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Dependencies
{
    public class InMemoryGameRepository : IGameRepository
    {
        private Dictionary<string, Game> Repository { get; set; }
        public InMemoryGameRepository()
        {
            Repository = new Dictionary<string, Game>();
        }
        public async Task UpdateGame(Game game)
        {
            Repository[game.LobbyCode] = game;
        }

        public async Task<bool> GameExistsAsync(string LobbyCode)
        {
            return Repository.ContainsKey(LobbyCode);
        }

        public async Task<Game> GetGameAsync(string LobbyCode)
        {
            return Repository[LobbyCode];
        }

        public async Task<string> GetGameTypeAsync(string LobbyCode)
        {
            return Repository[LobbyCode].GameType;
        }
    }
}
