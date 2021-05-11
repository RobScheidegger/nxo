using System;
using System.Collections.Generic;
using System.Text;

namespace NXO.Shared.Models
{
    public interface IGameMove
    {
        public string PlayerId { get; set; }
        public string LobbyCode { get;set; }
    }
}
