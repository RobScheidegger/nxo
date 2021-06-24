using Microsoft.Extensions.DependencyInjection;
using NXO.Server.Dependencies;
using NXO.Server.Modules;
using NXO.Server.Modules.TicTacToe;
using NXO.Shared.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace NXO.Shared.Modules
{
    public class TicTacToeModule : INXOModule
    {
        public string Name => "Tic Tac Toe";

        public string GameType => "tictactoe";

        public void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IRepository<TicTacToeGameStatus>, InMemoryRepository<TicTacToeGameStatus>>();
            services.AddSingleton<IModuleManager, TicTacToeModuleManager>();
            services.AddSingleton<TicTacToeModuleManager>();
            services.AddSingleton<TicTacToeGameLogicHandler>();

            //Register Bots
            services.AddSingleton<ITicTacToeBot, TicTacToeMinimaxBot>();
        }

    }
}
