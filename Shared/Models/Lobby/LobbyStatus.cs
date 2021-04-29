using System.Collections.Generic;

namespace NXO.Shared.Models
{
    public class LobbyStatus
    {
        public string Nickname { get; set; }
        public string LobbyCode { get; set; }
        public IEnumerable<Player> Players { get; set; }
    }
}