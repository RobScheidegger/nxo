using NXO.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Dependencies
{
    public interface ILobbyCoordinator
    {
        /// <summary>
        /// Assumes that all of the conditions for joining a lobby are met and adds the player to the lobby.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The result of the </returns>
        Task<JoinResult> JoinAsync(JoinRequest request);
        Task<JoinResult> AttemptJoinAsync(JoinRequest request);
        Task<CreateLobbyResult> CreateLobbyAsync(CreateLobbyRequest request);
    }
}
