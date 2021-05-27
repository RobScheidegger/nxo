using NXO.Server.Modules.TicTacToe;
using NXO.Shared.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NXO.UnitTests.TicTacToe
{
    public class TicTacToeBotTests
    {
        private readonly TicTacToeBot bot;
        private readonly TicTacToeGameLogicHandler logic;
        public TicTacToeBotTests()
        {
            logic = new TicTacToeGameLogicHandler();
            bot = new TicTacToeBot(logic);
        }

        [Fact]
        public async void Bot_3x3_ForcedMove_Vertical()
        {
            //Arrange
            var board = TicTacToeTestUtilities.Get2DBoard(new char?[,]
            {
                {'x', null, null},
                {null, null, 'o'},
                {null, null, 'o' }
            });
            var gameStatus = new TicTacToeGameStatus()
            {
                Board = board,
                CurrentPlayerId = "bot"
            };
            var gameSettings = new TicTacToeSettings()
            {
                BoardSize = 3,
                Dimensions = 2,
                LobbyCode = "test",
                MaximumPlayers = 2,
                Players = new List<TicTacToePlayer>()
                {
                    new TicTacToePlayer()
                    {
                        PlayerId = "playerTest",
                        Bot = false,
                        Token = 'o'
                    },
                    new TicTacToePlayer()
                    {
                        PlayerId = "bot",
                        Bot = true,
                        Token = 'x'
                    }
                }
            };
            //Act
            var move = await bot.GetNextMove('x', gameStatus, gameSettings);
            //Assert
            Assert.Equal(move.Path.ToArray(), new int[] { 0, 2 });
        }

        [Fact]
        public void Bot_3x3_DetermineMinimaxScore()
        {
            //Arrange
            var board = new char?[,]
            {
                {'x', 'o', null},
                {null, 'o', 'x'},
                {'x', 'o', null }
            };
            //Act
            var score = bot.EvaluateWin(board);
            //Assert
            Assert.Equal(10,score);
        }
    }
}
