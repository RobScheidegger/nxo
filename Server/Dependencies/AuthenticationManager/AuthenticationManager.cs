using NXO.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Dependencies
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly ILobbyCoordinator lobbyCoordinator;
        public AuthenticationManager(ILobbyCoordinator lobbyCoordinator)
        {
            this.lobbyCoordinator = lobbyCoordinator;
        }
        public Task<JoinResult> AttemptJoinAsync(JoinRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
