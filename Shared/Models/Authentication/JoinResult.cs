using System;
using System.Collections.Generic;
using System.Text;

namespace NXO.Shared.Models
{
    public class JoinResult
    {
        public bool Success { get; set; }
        public string PlayerId { get; set; }
        public string GameType { get; set; }
        public string RejectMessage { get; set; }
    }
}
