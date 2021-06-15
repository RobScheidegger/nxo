using System;
using System.Collections.Generic;
using System.Text;

namespace NXO.Shared.Modules
{
    public class TicTacToeMoveHistory
    {
        public string PlayerName { get; set; }
        public IEnumerable<int> MovePath { get; set; }
    }
}
