using NXO.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Dependencies
{
    public interface IAuthenticationManager
    {
        Task<JoinResult> AttemptJoinAsync(JoinRequest request);
    }
}
