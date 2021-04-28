using NXO.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Dependencies
{
    public interface ILobbyCoordinator
    {
        Task<bool> CanJoinAsync(string LobbyCode);
        Task<JoinResult> JoinAsync(string LobbyCode);
        Task<CreateLobbyResult> CreateLobbyAsync(CreateLobbyRequest request);
    }
}
