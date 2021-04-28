using NXO.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Dependencies
{
    public interface IGameRepository
    {
        Task<bool> GameExistsAsync(string LobbyCode);
        Task<string> GetGameTypeAsync(string LobbyCode);
        Task<Game> GetGameAsync(string LobbyCode);
        Task UpdateGame(Game game);
    }
}
