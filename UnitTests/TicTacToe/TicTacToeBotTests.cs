using NXO.Server.Modules.TicTacToe;
using NXO.Shared.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NXO.UnitTests.TicTacToe;

public class TicTacToeBotTests
{
    private readonly TicTacToeMinimaxBot bot;
    private readonly TicTacToeGameLogicHandler logic;
    public TicTacToeBotTests()
    {
        logic = new TicTacToeGameLogicHandler();
        bot = new TicTacToeMinimaxBot(logic);
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
            CurrentPlayerId = "bot",
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
        var move = await bot.GetNextMove(gameStatus);
        //Assert
        Assert.Equal(move.Path.ToArray(), new int[] { 0, 2 });
    }

    [Fact]
    public async void Bot_3x3_ForcedMove_Horizontal()
    {
        //Arrange
        var board = TicTacToeTestUtilities.Get2DBoard(new char?[,]
        {
                {'x', null, null},
                {null, 'o', 'o'},
                {null, null, 'x' }
        });
        var gameStatus = new TicTacToeGameStatus()
        {
            Board = board,
            CurrentPlayerId = "bot",
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
                        Token = 'x'
                    },
                    new TicTacToePlayer()
                    {
                        PlayerId = "bot",
                        Bot = true,
                        Token = 'o',
                        BotType = "Minimax"
                    }
                }
        };
        //Act
        var move = await bot.GetNextMove(gameStatus);
        var array = move.Path.ToArray();
        //Assert
        Assert.Equal(new int[] { 1, 0 }, array);
    }
}
