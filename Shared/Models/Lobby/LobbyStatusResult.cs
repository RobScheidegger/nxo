using System;
using System.Collections.Generic;
using System.Text;

namespace NXO.Shared.Models
{
    public class LobbyStatusResult<SettingsType>
    {
        public Game Game { get; set; }
        public SettingsType Settings { get; set; }
    }
}
