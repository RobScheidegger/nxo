﻿using Microsoft.Extensions.Hosting;
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
        private readonly IRepository<Game> games;
        private readonly Dictionary<string, IModuleManager> modules;
        public DebugDataService(IRepository<Game> games, IEnumerable<IModuleManager> managers)
        {
            this.games = games;
            modules = managers.ToDictionary(i => i.GameType, i => i);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var tictactoe = modules["tictactoe"];
            var testGame = new Game()
            {
                LobbyCode = "test",
                DateCreated = DateTime.Now,
                GameType = "tictactoe",
                HostPlayerId = "player1",
                Nickname = "Test Game",
                Stage = "In Game",
                Settings = new GameSettings()
                {
                    MaximumPlayers = 2,
                    MinimumPlayers = 2
                },
                Players = new Player[]
                {
                    new Player()
                    {
                        Id = "player1",
                        Nickname = "Test Player 1"
                    },
                    new Player()
                    {
                        Id = "player2",
                        Nickname = "Test Player 2"
                    }
                }
            };

            var testGame2 = new Game()
            {
                LobbyCode = "test2",
                DateCreated = DateTime.Now,
                GameType = "tictactoe",
                HostPlayerId = "player1",
                Nickname = "Test Game",
                Stage = "In Game",
                Settings = new GameSettings()
                {
                    MaximumPlayers = 2,
                    MinimumPlayers = 2
                },
                Players = new Player[]
                {
                    new Player()
                    {
                        Id = "player1",
                        Nickname = "Test Player 1"
                    },
                    new Player()
                    {
                        Id = "player2",
                        Nickname = "Test Player 2"
                    }
                }
            };

            await games.Add(testGame.LobbyCode, testGame);
            await tictactoe.CreateLobbyAsync(testGame);
            var settings = new TicTacToeSettings()
            {
                BoardSize = 4,
                Dimensions = 4,
                LobbyCode = testGame.LobbyCode
            };
            await tictactoe.SaveSettingsAsync(settings);
            await tictactoe.StartGame(testGame.LobbyCode);

            await games.Add(testGame2.LobbyCode, testGame2);
            await tictactoe.CreateLobbyAsync(testGame2);
            var settings2 = new TicTacToeSettings()
            {
                BoardSize = 3,
                Dimensions = 3,
                LobbyCode = testGame2.LobbyCode
            };
            await tictactoe.SaveSettingsAsync(settings2);
            await tictactoe.StartGame(testGame2.LobbyCode);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
