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

namespace NXO.Server.Modules
{
    public class TicTacToeModuleManager : IModuleManager
    {
        private readonly IGuidProvider guid;
        private readonly IRepository<TicTacToeSettings> settingsRepository;
        private readonly IRepository<TicTacToeGameStatus> gameStatusRepository;
        private readonly IRepository<Game> gameRepository;
        private readonly TicTicToeBot bot;
        public TicTacToeModuleManager(IGuidProvider guid, IRepository<TicTacToeSettings> settings, IRepository<Game> gameRepository, IRepository<TicTacToeGameStatus> gameStatus)
        {
            this.guid = guid;
            this.settingsRepository = settings;
            this.gameRepository = gameRepository;
            this.gameStatusRepository = gameStatus;
            bot = new TicTicToeBot();
        }
        public string GameType => "tictactoe";
        private char[] Tokens = { 'x', 'o', 'q'};
        private int TokenIndex = 0;
        private char GetNextToken()
        {
            var token = Tokens[TokenIndex % Tokens.Length];
            TokenIndex++;
            return token;
        }
        public async Task<bool> CreateLobbyAsync(Game game)
        {
            var setting = new TicTacToeSettings()
            {
                BoardSize = 3,
                Dimensions = 2,
                MaximumPlayers = 2,
                LobbyCode = game.LobbyCode,
                Players = game.Players.Select(i => new TicTacToePlayer()
                {
                    Bot = false,
                    Nickname = i.Nickname,
                    Token = GetNextToken(),
                    PlayerId = i.Id
                })
            };
            await settingsRepository.Add(game.LobbyCode, setting);
            return true;
        }
        public async Task<LobbyStatusResult<T>> GetLobbyStatus<T>(LobbyStatusRequest request) where T : class, IGameSettings
        {
            var game = await gameRepository.Find(request.LobbyCode);
            var settings = await GetSettings(request.LobbyCode);
            return new LobbyStatusResult<T>()
            {
                Game = game,
                Settings = settings as T
            };
        }
        public  async Task<IGameStatus> GetGameStateAsync(string LobbyCode)
        {
            var status = await gameStatusRepository.Find(LobbyCode);
            return status;
        }

        public async Task<IGameSettings> GetSettings(string LobbyCode)
        {
            return await settingsRepository.Find(LobbyCode);
        }

        public async Task<MoveResult> PerformMoveAsync(IGameMove move)
        {
            var properMove = move as TicTacToeMove;
            var gameStatus = await gameStatusRepository.Find(move.LobbyCode);
            var settings = await settingsRepository.Find(move.LobbyCode);
            if(IsValidMove(properMove, gameStatus))
            {
                var playerToken = settings.Players.Where(i => i.PlayerId == move.PlayerId).First().Token;
                var currentPlayerIndex = settings.Players.Select(i => i.PlayerId).ToList().IndexOf(gameStatus.CurrentPlayerId);
                var currentPlayer = settings.Players.ElementAt(currentPlayerIndex);
                var nextPlayerIndex = (currentPlayerIndex + 1) % settings.Players.Count();
                var nextPlayer = settings.Players.ElementAt(nextPlayerIndex);
                await gameStatusRepository.Update(move.LobbyCode, g =>
                {
                    g.Board.Place(playerToken, properMove.Path);
                    g.CurrentPlayerId = nextPlayer.PlayerId;
                    g.CurrentPlayerName = nextPlayer.Nickname;
                });
                return new MoveResult()
                {
                    Success = true,
                    Message = $"{currentPlayer.Nickname} placed a '{currentPlayer.Token}' at '{string.Join(',', properMove.Path)}'"
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

        private static bool IsValidMove(TicTacToeMove properMove, TicTacToeGameStatus gameStatus)
        {
            return gameStatus.Board.GetForPosition(properMove.Path) == null
                && properMove.PlayerId == gameStatus.CurrentPlayerId;
        }

        public async Task<SaveSettingsResult> SaveSettingsAsync(IGameSettings settings)
        {
            var properSettings = settings as TicTacToeSettings;
            await settingsRepository.Update(properSettings.LobbyCode, s =>
            {
                s.BoardSize = properSettings.BoardSize;
                s.Dimensions = properSettings.Dimensions;
            });
            return new SaveSettingsResult()
            {
                Message = "Saved",
                Success = true
            };
        }

        public async Task StartGame(string LobbyCode)
        {
            var gameSettings = await settingsRepository.Find(LobbyCode);

            var game = await gameRepository.Find(LobbyCode);

            var playerCount = game.Players.Count();
            var rand = new Random();

            var startingPlayer = game.Players.ElementAt(rand.Next(0, playerCount));
            var lengths = Enumerable.Range(0, gameSettings.Dimensions).Select(i => gameSettings.BoardSize);

            var gameStatus = new TicTacToeGameStatus()
            {
                CurrentPlayerId = startingPlayer.Id,
                CurrentPlayerName = startingPlayer.Nickname,
                Board = TicTacToeBoard.Construct(gameSettings.Dimensions, gameSettings.BoardSize)
            };

            await gameStatusRepository.Add(LobbyCode, gameStatus);
        }

        public bool HasPlayerWon(char playerToken, TicTacToeBoard board)
        {
            var vectors = GetVectorsForDimension(board.Dimension);
            var arrayBoard = bot.GetArrayFromBoard(board);
            var playerMoves = bot.GetPositionFromBoardWhere(arrayBoard, (i, arr) => arr.GetValue(i) as char? == playerToken, board.Dimension);
            var playerEdgeMoves = playerMoves.Where(move => move.Contains(0));
            var playerMovesHash = new HashSet<int[]>(playerMoves);
            var boardSize = board.Boards.Count();

            return playerEdgeMoves.Any(move => vectors.Any(vector =>
                {
                    var moveCheck = Enumerable.Range(0, boardSize).Select(n => Add(move, Multiply(n, vector)));

                    return moveCheck.All(playerMovesHash.Contains);
                }));

            int[] Add(int[] firstArray, int[] secondArray)
            {
                return firstArray.Zip(secondArray, (x, y) => x + y).ToArray();
            }

            int[] Multiply(int scalar, int[] array)
            {
                return array.Select(i => i * scalar).ToArray();
            }
        }

        private int[][] GetVectorsForDimension(int dimension)
        {
            throw new NotImplementedException();
        }
    }
}
