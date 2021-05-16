using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NXO.Server.Dependencies;
using NXO.Shared;
using NXO.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LobbyController : ControllerBase
    {
        private readonly ILogger<LobbyController> logger;
        private readonly ILobbyCoordinator lobbyCoordinator;

        public LobbyController(ILogger<LobbyController> logger, ILobbyCoordinator lobbyCoordinator)
        {
            this.logger = logger;
            this.lobbyCoordinator = lobbyCoordinator;
        }

        [HttpPost("Join")]
        public async Task<JoinResult> Join(JoinRequest request)
        {
            return await lobbyCoordinator.AttemptJoinAsync(request);
        }

        [HttpGet("GameTypes")]
        public IEnumerable<GameTypesResponse> GameTypes([FromServices] IEnumerable<INXOModule> modules)
        {
            return modules.Select(i => new GameTypesResponse()
            {
                GameType = i.GameType,
                Name = i.Name 
            });
        }

        [HttpPost("Create")]
        public async Task<CreateLobbyResult> Create(CreateLobbyRequest request)
        {
            return await lobbyCoordinator.CreateLobbyAsync(request);
        }
        
        
    }
}
