using System;
using System.Collections.Generic;
using System.Text;

namespace NXO.Shared.Models
{
    public class RemovePlayerRequest
    {
        public string LobbyCode { get; set; }
        public string RequestPlayerId { get; set; }
        public string RemovePlayerId { get; set; }
    }
}
