using NXO.Server.Modules;
using NXO.Server.Modules.TicTacToe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NXO.UnitTests.TicTacToe
{
    public class TicTacToeGameLogicTests
    {
        private readonly TicTacToeGameLogicHandler logic;
        public TicTacToeGameLogicTests()
        {
            logic = new TicTacToeGameLogicHandler();
        }
        [Fact]
        public void Logic_3x3_GetPositionFromBoardWhere()
        {
            //Arrange
            var board = new char?[,]
            {
                {'x', null, null},
                {null, 'o', null},
                {null, 'o', null }
            };
            //Act
            var x_paths = logic.GetPositionFromBoardWhere(board, (path, arr) => arr.GetValue(path) as char? == 'x', 2);
            var o_paths = logic.GetPositionFromBoardWhere(board, (path, arr) => arr.GetValue(path) as char? == 'o', 2);
            //Assert
            Assert.Contains(x_paths, i => i[0] == 0 && i[1] == 0);
            Assert.Contains(o_paths, i => i[0] == 1 && i[1] == 1);
            Assert.Contains(o_paths, i => i[0] == 2 && i[1] == 1);
        }
        [Fact]
        public void Logic_3x3_GetPositionFromBoardWhere_Horizontal()
        {
            //Arrange
            var board = TicTacToeTestUtilities.Get2DBoard(new char?[,]
            {
                {'x', null, 'x'},
                {'o', 'o', 'o'},
                {null, 'x', null }
            });
            //Act
            var arrayBoard = logic.GetArrayFromBoard(board);
            var x_paths = logic.GetPositionFromBoardWhere(arrayBoard, (path, arr) => arr.GetValue(path) as char? == 'x', 2);
            var o_paths = logic.GetPositionFromBoardWhere(arrayBoard, (path, arr) => arr.GetValue(path) as char? == 'o', 2);
            //Assert
            Assert.Contains(x_paths, i => i[0] == 2 && i[1] == 1);
            Assert.Contains(x_paths, i => i[0] == 0 && i[1] == 0);
            Assert.Contains(x_paths, i => i[0] == 0 && i[1] == 2);

            Assert.Contains(o_paths, i => i[0] == 1 && i[1] == 0);
            Assert.Contains(o_paths, i => i[0] == 1 && i[1] == 1);
            Assert.Contains(o_paths, i => i[0] == 1 && i[1] == 2);
        }
        [Fact]
        public void Logic_3x3_WinHorizontal()
        {
            //Arrange
            var board = TicTacToeTestUtilities.Get2DBoard(new char?[,]
            {
                {'x', null, 'x'},
                {'o', 'o', 'o'},
                {null, 'x', null }
            });
            //Act
            var o_wins = logic.HasPlayerWon('o', board);
            var x_wins = logic.HasPlayerWon('x', board);
            //Assert
            Assert.True(o_wins);
            Assert.False(x_wins);
        }
        [Fact]
        public void Logic_3x3_WinVertical()
        {
            //Arrange
            var board = TicTacToeTestUtilities.Get2DBoard(new char?[,]
            {
                {'x', null, 'x'},
                {'x', 'o', 'o'},
                {'x', 'x', null }
            });
            //Act
            var o_wins = logic.HasPlayerWon('o', board);
            var x_wins = logic.HasPlayerWon('x', board);
            //Assert
            Assert.True(x_wins);
            Assert.False(o_wins);
        }
        [Fact]
        public void Logic_3x3x3_WinAcrossDimensions()
        {
            //Arrange
            var board = TicTacToeTestUtilities.Get3DBoard(new char?[,,]
            {
                { {'x', null, 'x'},
                {'x', 'o', 'o'},
                {'x', 'x', null } }
            });
            //Act
            var o_wins = logic.HasPlayerWon('o', board);
            var x_wins = logic.HasPlayerWon('x', board);
            //Assert
            Assert.True(x_wins);
            Assert.False(o_wins);
        }
    }
}
