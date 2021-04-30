using Microsoft.AspNetCore.Mvc;
using NXO.Server.Controllers;
using NXO.Shared.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Modules
{
    public class TicTacToeController : BaseGameController<TicTacToeSettings, TicTacToeGameStatus, TicTacToeMove>
    {
        
    }
}
