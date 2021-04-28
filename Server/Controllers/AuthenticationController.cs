using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NXO.Server.Dependencies;
using NXO.Shared;
using NXO.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> logger;
        private readonly IAuthenticationManager authentication;

        public AuthenticationController(ILogger<AuthenticationController> logger, IAuthenticationManager authentication)
        {
            this.logger = logger;
            this.authentication = authentication;
        }

        [HttpPost("Join")]
        public async Task<JoinResult> Join(JoinRequest request)
        {
            return await authentication.AttemptJoinAsync(request);
        }
    }
}
