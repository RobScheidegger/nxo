using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace NXO.Shared.Modules
{
    public class TicTacToeModule : INXOModule
    {
        public string Name => "Tic Tac Toe";

        public string GameType => "tictactoe";

        public Type Settings => typeof(TicTacToeSettings);

        public void RegisterServices(IServiceCollection services)
        {
            throw new NotImplementedException();
        }
    }
}
