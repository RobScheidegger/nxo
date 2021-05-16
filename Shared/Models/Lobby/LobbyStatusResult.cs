using System;
using System.Collections.Generic;
using System.Text;

namespace NXO.Shared.Models
{
    public class LobbyStatusResult<SettingsType> where SettingsType : IGameSettings
    {
        public Game Game { get; set; }
        public SettingsType Settings { get; set; }
    }
}
