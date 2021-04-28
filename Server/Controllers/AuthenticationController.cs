using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NXO.Shared;
using NXO.Shared.Models.Authentication;
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

        public AuthenticationController(ILogger<AuthenticationController> logger)
        {
            this.logger = logger;
        }

        [HttpPost("Join")]
        public JoinResult Join(JoinRequest request)
        {
            return new JoinResult()
            {
                Success = true
            };
        }
    }
}
