using NXO.Server.Dependencies;
using NXO.Server.Modules.TicTacToe;
using NXO.Shared;
using NXO.Shared.Models;
using NXO.Shared.Modules;
using NXO.Shared.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXO.Server.Modules;

public class TicTacToeModuleManager : IModuleManager
{
    private const int MaximumBoardUnits = 2000;

    private readonly IGuidProvider guid;
    private readonly IRepository<TicTacToeGameStatus> gameStatusRepository;
    private readonly Dictionary<string, ITicTacToeBot> bots;
    private readonly TicTacToeGameLogicHandler logic;
    public TicTacToeModuleManager(IGuidProvider guid,
        IRepository<TicTacToeGameStatus> gameStatus, TicTacToeGameLogicHandler logic, 
        IEnumerable<ITicTacToeBot> bots)
    {
        this.guid = guid;
        this.gameStatusRepository = gameStatus;
        this.logic = logic;
        this.bots = bots.ToDictionary(i => i.Type, i => i);
    }
    public string GameType => "tictactoe";
    private char[] Tokens = { 'x', 'o', 'q', 'y', 't', 'p' };
    private char GetToken(int playerCount)
    {
        return Tokens[playerCount % Tokens.Length];
    }
    public async Task<IGameStatus> CreateLobbyAsync(CreateLobbyRequest request)
    {
        var player = new TicTacToePlayer()
        {
            PlayerId = guid.New(),
            Nickname = request.Nickname,
            Token = 'x',
            Bot = false
        };
        var tictactoeGame = new TicTacToeGameStatus()
        {
            DateCreated = DateTime.Now,
            LobbyCode = guid.NewLobbyCode(),
            GameType = request.GameType,
            Nickname = "New Lobby",
            Players = new TicTacToePlayer[]
            {
                    player
            },
            Stage = "Lobby",
            HostPlayerId = player.PlayerId,
            BoardSize = 3,
            Dimensions = 2,
            MaximumPlayers = 2
        };
        await gameStatusRepository.Add(tictactoeGame.LobbyCode, tictactoeGame);
        return tictactoeGame;
    }
    public async Task<IGameStatus> GetGameStateAsync(string LobbyCode)
    {
        var status = await gameStatusRepository.Find(LobbyCode);
        return status;
    }

    public async Task<MoveResult> PerformMoveAsync(IGameMove move)
    {
        var properMove = move as TicTacToeMove;
        var gameStatus = await gameStatusRepository.Find(move.LobbyCode);
        if (gameStatus.Completed)
        {
            return new MoveResult()
            {
                Success = false,
                Message = "Cannot place move, game is over."
            };
        }
        if (IsValidMove(properMove, gameStatus))
        {
            var currentPlayerIndex = gameStatus.Players.Select(i => i.PlayerId).ToList().IndexOf(gameStatus.CurrentPlayerId);
            var currentPlayer = gameStatus.Players.ElementAt(currentPlayerIndex);
            var nextPlayerIndex = (currentPlayerIndex + 1) % gameStatus.Players.Count();
            var nextPlayer = gameStatus.Players.ElementAt(nextPlayerIndex);

            await gameStatusRepository.Update(move.LobbyCode, g =>
            {
                g.Board.Place(currentPlayer.Token, properMove.Path);
                g.CurrentPlayerId = nextPlayer.PlayerId;
                g.CurrentPlayerName = nextPlayer.Nickname;
                g.History.Add(new TicTacToeMoveHistory()
                {
                    PlayerName = currentPlayer.Nickname,
                    MovePath = properMove.Path
                });
            });
            var updatedGame = await gameStatusRepository.Find(move.LobbyCode);
            if (logic.HasPlayerWon(currentPlayer.Token, updatedGame.Board))
            {
                await CompleteGame(move.LobbyCode, currentPlayer);
            }
            else if (logic.IsDraw(updatedGame.Board))
            {
                await CompleteGame(move.LobbyCode, null);
            }
            else if (nextPlayer.Bot)
            {

                var botMove = await bots[nextPlayer.BotType].GetNextMove(gameStatus);
                await PerformMoveAsync(botMove);

            }
            return new MoveResult()
            {
                Success = true
            };
        }
        else
        {
            return new MoveResult()
            {
                Success = false,
                Message = "Move was not valid."
            };
        }
    }

    internal async Task<bool> RemovePlayer(RemovePlayerRequest request)
    {
        var game = await gameStatusRepository.Find(request.LobbyCode);
        if (game.HostPlayerId != request.RequestPlayerId)
            return false;

        await gameStatusRepository.Update(request.LobbyCode, game =>
        {
            game.Players = game.Players.Where(i => i.PlayerId != request.RemovePlayerId).ToList();
        });

        return true;
    }

    private async Task CompleteGame(string lobbyCode, TicTacToePlayer winningPlayer)
    {
        await gameStatusRepository.Update(lobbyCode, game =>
        {
            game.Winner = winningPlayer;
            game.Completed = true;
        });
    }

    private static bool IsValidMove(TicTacToeMove properMove, TicTacToeGameStatus gameStatus)
    {
        return gameStatus.Board.GetForPosition(properMove.Path) == null
            && properMove.PlayerId == gameStatus.CurrentPlayerId;
    }

    public async Task<SaveSettingsResult> SaveSettingsAsync(IGameStatus settings)
    {
        var properSettings = settings as TicTacToeGameStatus;
        await gameStatusRepository.Update(properSettings.LobbyCode, s =>
        {
            s.BoardSize = properSettings.BoardSize;
            s.Dimensions = properSettings.Dimensions;
            s.MaximumPlayers = properSettings.MaximumPlayers;
            s.Nickname = properSettings.Nickname;
        });
        return new SaveSettingsResult()
        {
            Message = "Saved",
            Success = true
        };
    }

    public async Task<StartGameResult> StartGame(string LobbyCode)
    {
        var rand = new Random();
        var status = await gameStatusRepository.Find(LobbyCode);
        if (Math.Pow(status.BoardSize, status.Dimensions) > MaximumBoardUnits)
        {
            return new StartGameResult()
            {
                Success = false,
                Message = $"Size of the board exceeds the maximum: {MaximumBoardUnits}"
            };
        }

        await gameStatusRepository.Update(LobbyCode, g =>
        {
            var startingPlayer = g.Players.ElementAt(rand.Next(0, g.PlayerCount));
            g.CurrentPlayerId = startingPlayer.PlayerId;
            g.CurrentPlayerName = startingPlayer.Nickname;
            g.Board = TicTacToeBoard.Construct(g.Dimensions, g.BoardSize);
            g.History = new List<TicTacToeMoveHistory>();
            g.Stage = "In Progress";
        });
        var game = await gameStatusRepository.Find(LobbyCode);
        var player = game.Players.Where(p => p.PlayerId == game.CurrentPlayerId).First();
        if (player.Bot)
        {
            _ = Task.Run(async () =>
            {
                var botMove = await bots[player.BotType].GetNextMove(game);
                await PerformMoveAsync(botMove);
            });
        }
        return new StartGameResult() { Success = true };
    }

    public async Task<bool> HasLobbyStartedAsync(string LobbyCode)
    {
        var game = await gameStatusRepository.Find(LobbyCode);
        return game.Stage == "In Progress" || game.Stage == "Completed";
    }
    public async Task<JoinResult> JoinAsync(JoinRequest request)
    {
        var game = await gameStatusRepository.Find(request.GameCode);
        var newPlayer = new TicTacToePlayer()
        {
            PlayerId = guid.New(),
            Nickname = request.Nickname,
            Token = GetToken(game.Players.Count())
        };
        await gameStatusRepository.Update(request.GameCode, g => g.Players = g.Players.Append(newPlayer));
        return new JoinResult()
        {
            GameType = game.GameType,
            PlayerId = newPlayer.PlayerId,
            Success = true
        };
    }

    public async Task<bool> LobbyExistsAsync(string LobbyCode)
    {
        return LobbyCode != null && await gameStatusRepository.Exists(LobbyCode);
    }

    public async Task<bool> SpotAvailableAsync(string LobbyCode)
    {
        var game = await gameStatusRepository.Find(LobbyCode);
        return game != null && game.PlayerCount < game.MaximumPlayers;
    }

    public async Task AddBot(string lobbyCode, string botType)
    {
        var game = await gameStatusRepository.Find(lobbyCode);
        var bot = new TicTacToePlayer()
        {
            Bot = true,
            BotType = botType,
            Nickname = "Bot",
            PlayerId = guid.New(),
            Token = GetToken(game.Players.Count())
        };

        await gameStatusRepository.Update(lobbyCode, status => status.Players = status.Players.Append(bot));
    }
}

public class GameUpdatedEventArgs : EventArgs
{
    public GameUpdatedEventArgs(string lobbyCode)
    {
        LobbyCode = lobbyCode;
    }
    public string LobbyCode { get; set; }
}