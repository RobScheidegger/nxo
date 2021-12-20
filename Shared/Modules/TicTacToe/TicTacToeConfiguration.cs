using System;
using System.Collections.Generic;
using System.Text;

namespace NXO.Shared.Modules.TicTacToe
{
    public class TicTacToeConfiguration
    {
        public IEnumerable<string> BotTypes { get; set; } = new[]
        {
            "Static",
            "Minimax"
        };
    }
}
