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
    public class BaseGameController<GameStatusClass, MoveClass> : Controller 
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
        public async Task<SaveSettingsResult> SaveSettings(GameStatusClass settings)
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
            return await manager.GetGameStateAsync(LobbyCode) as GameStatusClass;
        }
    }
}
