using Microsoft.Extensions.Hosting;
using NXO.Server.Dependencies;
using NXO.Server.Modules;
using NXO.Shared.Models;
using NXO.Shared.Modules;
using NXO.Shared.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NXO.Server.Services
{
    public class DebugDataService : IHostedService
    {
        private const bool IsEnabled = false; 
        private readonly IRepository<TicTacToeGameStatus> tictactoeGames;
        private readonly Dictionary<string, IModuleManager> modules;
        public DebugDataService(IRepository<TicTacToeGameStatus> games, IEnumerable<IModuleManager> managers)
        {
            this.tictactoeGames = games;
            modules = managers.ToDictionary(i => i.GameType, i => i);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!IsEnabled)
                return;
            var tictactoe = modules["tictactoe"];
            var testGame = new TicTacToeGameStatus()
            {
                LobbyCode = "test",
                DateCreated = DateTime.Now,
                GameType = "tictactoe",
                HostPlayerId = "player1",
                Nickname = "Test Game",
                Stage = "In Game",
                MaximumPlayers = 2,
                Players = new List<TicTacToePlayer>()
                {
                    new TicTacToePlayer()
                    {
                        PlayerId = "player1",
                        Nickname = "Test Player 1",
                        Token = 'x'
                    },
                    new TicTacToePlayer()
                    {
                        PlayerId = "player2",
                        Nickname = "Test Player 2",
                        Token = 'o'
                    }
                },
                BoardSize = 4,
                Dimensions = 4
            };
            await tictactoeGames.Add(testGame.LobbyCode, testGame);
            await tictactoe.StartGame(testGame.LobbyCode);

            var testGame2 = new TicTacToeGameStatus()
            {
                LobbyCode = "test2",
                DateCreated = DateTime.Now,
                GameType = "tictactoe",
                HostPlayerId = "player1",
                Nickname = "Test Game",
                Stage = "In Game",
                MaximumPlayers = 2,
                
                Players = new List<TicTacToePlayer>()
                {
                    new TicTacToePlayer()
                    {
                        PlayerId = "player1",
                        Nickname = "Test Player 1",
                        Token = 'x'
                    },
                    new TicTacToePlayer()
                    {
                        PlayerId = "player2",
                        Nickname = "Test Player 2",
                        Token = 'o'
                    }
                },
                BoardSize = 3,
                Dimensions = 3
            };
            
            await tictactoeGames.Add(testGame2.LobbyCode, testGame2);
            await tictactoe.StartGame(testGame2.LobbyCode);

            var testGame3 = new TicTacToeGameStatus()
            {
                LobbyCode = "test3",
                DateCreated = DateTime.Now,
                GameType = "tictactoe",
                HostPlayerId = "player1",
                Nickname = "Test Game",
                Stage = "In Game",
                MaximumPlayers = 2,

                Players = new List<TicTacToePlayer>()
                {
                    new TicTacToePlayer()
                    {
                        PlayerId = "player1",
                        Nickname = "Test Player 1",
                        Token = 'x',
                        Bot = false
                    },
                    new TicTacToePlayer()
                    {
                        PlayerId = "bot",
                        Nickname = "Test Bot",
                        Token = 'o',
                        Bot = true,
                        BotType = "Static"
                    }
                },
                BoardSize = 4,
                Dimensions = 3
            };

            await tictactoeGames.Add(testGame3.LobbyCode, testGame3);
            _ = tictactoe.StartGame(testGame3.LobbyCode);

            return;
            var botvsbotGame = new TicTacToeGameStatus()
            {
                LobbyCode = "botvsbot",
                DateCreated = DateTime.Now,
                GameType = "tictactoe",
                HostPlayerId = "player1",
                Nickname = "Test Game",
                Stage = "In Game",
                MaximumPlayers = 2,

                Players = new List<TicTacToePlayer>()
                {
                    new TicTacToePlayer()
                    {
                        PlayerId = "bot2",
                        Nickname = "Test Box 2",
                        Token = 'x',
                        Bot = true,
                        BotType = "Minimax"
                    },
                    new TicTacToePlayer()
                    {
                        PlayerId = "bot1",
                        Nickname = "Test Bot",
                        Token = 'o',
                        Bot = true,
                        BotType = "Minimax"
                    }
                },
                BoardSize = 4,
                Dimensions = 3
            };

            await tictactoeGames.Add(botvsbotGame.LobbyCode, botvsbotGame);
            await tictactoe.StartGame(botvsbotGame.LobbyCode);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
