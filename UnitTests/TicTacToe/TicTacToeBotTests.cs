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
        private readonly TicTicToeBot bot;
        public TicTacToeBotTests()
        {
            bot = new TicTicToeBot();
        }

        [Fact]
        public async void Bot_3x3_ForcedMove()
        {
            //Arrange
            var board = TicTacToeTestUtilities.Get2DBoard(new char?[,]
            {
                {'x', null, null},
                {null, 'o', null},
                {null, 'o', null }
            });
            var gameStatus = new TicTacToeGameStatus()
            {
                Board = board,
                CurrentPlayerId = "idk"
            };
            //Act
            var move = await bot.GetNextMove('x', gameStatus);
            //Assert
            Assert.Equal(move.Path.ToArray(), new int[] { 0, 1 });
        }
    }
}
