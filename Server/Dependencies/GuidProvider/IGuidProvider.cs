using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Dependencies
{
    public interface IGuidProvider
    {
        string New();
        string NewLobbyCode();
    }
}
