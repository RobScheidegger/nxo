using Microsoft.AspNetCore.Mvc;
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
    public class BaseGameController<SettingsClass, GameStatusClass, MoveClass> : Controller 
        where SettingsClass : class, IGameSettings 
        where GameStatusClass : class, IGameStatus
        where MoveClass : class, IGameMove
    {
        public virtual string GameType => "default";
        public readonly IModuleManager manager;
        public BaseGameController(IEnumerable<IModuleManager> modules)
        {
            manager = modules.FirstOrDefault(i => i.GameType == GameType);
        }
        [HttpPost("SaveSettings")]
        public async Task<SaveSettingsResult> SaveSettings(SettingsClass settings)
        {
            return await manager.SaveSettingsAsync(settings);
        }
        [HttpPost("PerformMove")]
        public async Task<MoveResult> PerformMove(MoveClass move)
        {
            return await manager.PerformMoveAsync(move);
        }
        [HttpPost("GetGameStatus")]
        public async Task<GameStatusClass> GetGameStatus(string LobbyCode)
        {
            return await manager.GetGameStateAsync<GameStatusClass>(LobbyCode);
        }
        [HttpPost("LobbyStatus")]
        public async Task<LobbyStatusResult<SettingsClass>> Status(LobbyStatusRequest request)
        {
            return await manager.GetLobbyStatus<SettingsClass>(request);
        }
    }
}
