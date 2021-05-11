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
    public class BaseGameController<SettingsClass, GameStatusClass, MoveClass> : Controller 
        where SettingsClass : IGameSettings 
        where GameStatusClass : IGameStatus
        where MoveClass : IGameMove
    {
        public virtual string GameType => "default";
        public readonly IModuleManager manager;
        public BaseGameController(IEnumerable<IModuleManager> modules)
        {
            manager = modules.FirstOrDefault(i => i.Module.GameType == GameType);
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
    }
}
