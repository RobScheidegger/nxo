using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Client.Pages.TicTacToe
{
    public class TicTacToeDisplay
    {
        public IEnumerable<int> HighlightedCellPath { get; set; }
        public IEnumerable<int> LastMove { get; set; }
    }
}
