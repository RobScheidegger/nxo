using System;
using System.Collections.Generic;
using System.Text;

namespace NXO.Shared.Modules
{
    public class TicTacToeGameHistoryEntry
    {
        public string PlayerName { get; set; }
        public string Message { get; set; }
        public IEnumerable<int> MovePath { get; set; }
    }
}
