using System;
using System.Collections.Generic;
using System.Text;

namespace NXO.Shared.Models
{
    public class GameHistory
    {
        public IEnumerable<GameHistoryEntry> Entries;
    }
    public struct GameHistoryEntry
    {
        public DateTime Date { get; set; }
        public string Player { get; set; }
        public string Message { get; set; }
    }
}
