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
        public async void Bot_3x3_ForcedMove_Vertical()
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

        [Fact]
        public void Bot_3x3_GetPositionFromBoardWhere()
        {
            //Arrange
            var board = new char?[,]
            {
                {'x', null, null},
                {null, 'o', null},
                {null, 'o', null }
            };
            //Act
            var x_paths = bot.GetPositionFromBoardWhere(board, (path, arr) => arr.GetValue(path) as char? == 'x', 2);
            var o_paths = bot.GetPositionFromBoardWhere(board, (path, arr) => arr.GetValue(path) as char? == 'o', 2);
            //Assert
            Assert.Contains(x_paths, i => i[0] == 0 && i[1] == 0);
            Assert.Contains(o_paths, i => i[0] == 1 && i[1] == 1);
            Assert.Contains(o_paths, i => i[0] == 2 && i[1] == 1);
        }
    }
}
