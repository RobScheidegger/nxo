using NXO.Shared.Modules;
using System.Threading.Tasks;

namespace NXO.Server.Modules.TicTacToe
{
    public interface ITicTacToeBot
    {
        string Type { get; }
        Task<TicTacToeMove> GetNextMove(TicTacToeGameStatus GameStatus);
    }
}