using Microsoft.AspNetCore.Mvc;
using NXO.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Controllers
{
    public class BaseGameController<SettingsClass, GameStatusClass, MoveClass> : Controller 
    {
        public BaseGameController()
        {
            
        }
        [HttpPost("SaveSettings")]
        public IActionResult SaveSettings(SettingsClass settings)
        {
            return View();
        }
    }
}
