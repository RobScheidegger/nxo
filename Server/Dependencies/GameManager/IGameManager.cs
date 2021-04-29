using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NXO.Shared.Models;

namespace NXO.Server.Dependencies
{
    public interface IGameManager
    {
        Task<Player> AddPlayerAsync(string LobbyCode);

    }
}
