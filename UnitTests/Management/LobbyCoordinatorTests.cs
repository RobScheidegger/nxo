using System;
using Xunit;
using Moq;
using NXO.Server.Dependencies;
using NXO.Shared.Models;
using System.Threading.Tasks;
using NXO.Shared.Repository;
using System.Collections.Generic;

namespace NXO.UnitTests.Management
{ 
    public class LobbyCoordinatorTests
    {
        internal LobbyCoordinator manager { get; set; }
        Mock<IRepository<Game>> gameMock = new Mock<IRepository<Game>>();
        Mock<IGuidProvider> guidMock = new Mock<IGuidProvider>();
        Mock<IModuleManager> moduleManagerMock_1 = new Mock<IModuleManager>();
        public LobbyCoordinatorTests()
        {
            //Set this here because it is used in the constructor
            moduleManagerMock_1.Setup(i => i.GameType).Returns("tictactoe");
            manager = new LobbyCoordinator(gameMock.Object, guidMock.Object, new IModuleManager[] { moduleManagerMock_1.Object });
        }
        [Fact]
        public async Task AttemptJoin_LobbyCodeNull()
        {
            //Arrange
            var input = new JoinRequest()
            {
                GameCode = null,
                Nickname = "Something"
            };
            //Act
            var result = await manager.AttemptJoinAsync(input);
            //Assert
            Assert.False(result.Success);
            Assert.Null(result.Player);
            Assert.True(!string.IsNullOrEmpty(result.RejectMessage));
        }
        [Fact]
        public async Task AttemptJoin_LobbyCodeEmpty()
        {
            //Arrange
            var input = new JoinRequest()
            {
                GameCode = "",
                Nickname = "Something Else"
            };
            //Act
            var result = await manager.AttemptJoinAsync(input);
            //Assert
            Assert.False(result.Success);
            Assert.Null(result.Player);
            Assert.True(!string.IsNullOrEmpty(result.RejectMessage));
        }
        [Fact]
        public async Task AttemptJoin_LobbyDoesNotExist()
        {
            //Arrange
            var code = "TEST";
            var input = new JoinRequest()
            {
                GameCode = code,
                Nickname = "Anything"
            };
            //Act
            var result = await manager.AttemptJoinAsync(input);
            //Assert
            Assert.False(result.Success);
            Assert.Null(result.Player);
            Assert.True(!string.IsNullOrEmpty(result.RejectMessage));
        }
        [Fact]
        public async Task AttemptJoin_LobbyExists_NicknameEmpty()
        {
            //Arrange
            var code = "MORETEST";
            var input = new JoinRequest()
            {
                GameCode = code,
                Nickname = "Anything"
            };
            //Act
            var result = await manager.AttemptJoinAsync(input);
            //Assert
            Assert.False(result.Success);
            Assert.Null(result.Player);
            Assert.True(!string.IsNullOrEmpty(result.RejectMessage));
        }
        [Fact]
        public async Task AttemptJoin_LobbyExists_NicknameNotEmpty()
        {
            //Arrange
            const string code = "MORETEST";
            gameMock.Setup(i => i.Exists(code)).ReturnsAsync(true);
            gameMock.Setup(i => i.Find(code)).ReturnsAsync(new Game()
            {
                LobbyCode = code,
                Stage = "Lobby",
                GameType = "tictactoe",
                Nickname = "Some Nickname",
                Players = new List<Player>()
                {
                    new Player()
                    {
                        Id = "player1",
                        Nickname = "Player 1"
                    }
                },
                Settings = new GameSettings()
                {
                    MaximumPlayers = 2,
                    MinimumPlayers = 1
                }
            });
           
            var input = new JoinRequest()
            {
                GameCode = code,
                Nickname = "Anything"
            };
            //Act
            var result = await manager.AttemptJoinAsync(input);
            //Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Player);
            Assert.True(string.IsNullOrEmpty(result.RejectMessage));
        }
        [Fact]
        public async Task CreateLobbyAsync_Success()
        {
            //Arrange
            
            //Act

            //Assert

        }

    }
}
