using System;
using System.Collections.Generic;
using System.Text;

namespace NXO.Shared.Models
{
    public class CreateLobbyRequest
    {
        public string GameType { get; set; }
        public IGameSettings GameSettings { get; set; }
    }
}
