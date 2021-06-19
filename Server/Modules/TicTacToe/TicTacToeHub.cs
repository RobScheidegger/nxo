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
            var gameStatus = await statusRepository.Find(lobbyCode);
            if (gameStatus.Stage == "Lobby")
            {
                await Clients.Client(Context.ConnectionId).SendAsync("SetStatus", gameStatus);
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync("UpdateBoard", gameStatus);
            }
            
            await base.OnConnectedAsync();
        }

        public async Task PerformMove(TicTacToeMove move)
        {
            var lobbyCode = move.LobbyCode;
            var moveResult = await moduleManager.PerformMoveAsync(move);
            if (moveResult.Success)
            {
                await Clients.Group(lobbyCode).SendAsync("UpdateBoard", await statusRepository.Find(lobbyCode));
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync("Error", moveResult.Message);
            }
        }

        public async Task SaveSettings(TicTacToeGameStatus status)
        {
            var result = await moduleManager.SaveSettingsAsync(status);
            if(result.Success)
            {
                await Clients.Group(status.LobbyCode).SendAsync("UpdateSettings", await statusRepository.Find(status.LobbyCode));
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync("Error", result.Message);
            }
        }
    }
}
