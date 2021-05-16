using NXO.Shared;
using NXO.Shared.Models;
using NXO.Shared.Modules;
using System.Threading.Tasks;

namespace NXO.Server.Dependencies
{
    /// <summary>
    /// Manages the module-specific portion of the game/settings.
    /// </summary>
    public interface IModuleManager
    {
        string GameType { get; }
        /// <summary>
        /// Creates the game-specific portion of the lobby (settings, establishing it in the lobby repository)
        /// </summary>
        /// <param name="request">The request for lobby creation.</param>
        /// <returns>The result indicating success and status.</returns>
        Task<bool> CreateLobbyAsync(Game game);
        Task<MoveResult> PerformMoveAsync(IGameMove move);
        Task<SaveSettingsResult> SaveSettingsAsync(IGameSettings settings);
        Task<T> GetGameStateAsync<T>(string LobbyCode);
        Task<IGameSettings> GetSettings(string LobbyCode);
        Task<LobbyStatusResult<T>> GetLobbyStatus<T>(LobbyStatusRequest request) where T : class, IGameSettings;
    }
}