using System;
using System.Collections.Generic;
using System.Text;

namespace NXO.Shared.Models.Authentication
{
    public class JoinResult
    {
        public bool Success { get; set; }
        public string PlayerId { get; set; }
        public string Nickname { get; set; }
        public string RejectMessage { get; set; }
    }
}
