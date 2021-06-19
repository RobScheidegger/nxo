using Microsoft.AspNetCore.SignalR;
using NXO.Shared.Models;
using NXO.Shared.Modules;
using NXO.Shared.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Modules.TicTacToe
{
    public class TicTacToeHub : Hub
    {
        private readonly TicTacToeModuleManager moduleManager;
        private readonly IRepository<TicTacToeGameStatus> statusRepository;

        public TicTacToeHub(TicTacToeModuleManager moduleManager, IRepository<TicTacToeGameStatus> statusRepository) : base()
        {
            this.moduleManager = moduleManager;
            this.statusRepository = statusRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            if (!httpContext.Request.Query.TryGetValue("LobbyCode", out var lobbyCode))
            {
                throw new DataMisalignedException("No LobbyCode was found in the query string of the connection.");
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyCode);
            await Clients.Client(Context.ConnectionId).SendAsync("UpdateBoard", await statusRepository.Find(lobbyCode));
            await base.OnConnectedAsync();
        }

        public async Task<MoveResult> PerformMove(TicTacToeMove move)
        {
            var lobbyCode = move.LobbyCode;
            var moveResult = await moduleManager.PerformMoveAsync(move);
            if (moveResult.Success)
            {
                await Clients.Group(lobbyCode).SendAsync("UpdateBoard", await statusRepository.Find(lobbyCode));
            }
            return moveResult;
        }
    }
}
