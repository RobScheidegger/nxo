using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Dependencies
{
    public class GuidProvider : IGuidProvider
    {
        public string New()
        {
            return Guid.NewGuid().ToString();
        }

        public string NewLobbyCode()
        {
            return Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
        }
    }
}
