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
            var startingGuid = Guid.NewGuid().ToString();
            var alphabetic = string.Join("", startingGuid.Where(char.IsLetter));
            return alphabetic.Substring(0, 6).ToUpper();
        }
    }
}
