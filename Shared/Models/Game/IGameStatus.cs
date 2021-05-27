using System;
using System.Collections.Generic;
using System.Text;

namespace NXO.Shared.Models
{
    public interface IGameStatus
    {
        public DateTime DateCreated { get; set; }
        public string LobbyCode { get; set; }
        public string GameType { get; set; }
        public string Stage { get; set; }
        public string Nickname { get; set; }
        public string HostPlayerId { get; set; }
        public int MaximumPlayers { get; set; }
        public int PlayerCount { get; }
    }
}
