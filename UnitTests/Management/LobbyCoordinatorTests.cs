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
        Mock<IGuidProvider> guidMock = new Mock<IGuidProvider>();
        Mock<IModuleManager> moduleManagerMock_1 = new Mock<IModuleManager>();
        public LobbyCoordinatorTests()
        {
            //Set this here because it is used in the constructor
            moduleManagerMock_1.Setup(i => i.GameType).Returns("tictactoe");
            manager = new LobbyCoordinator(guidMock.Object, new IModuleManager[] { moduleManagerMock_1.Object });
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
            Assert.Null(result.PlayerId);
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
            Assert.Null(result.PlayerId);
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
            Assert.Null(result.PlayerId);
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
            Assert.Null(result.PlayerId);
            Assert.True(!string.IsNullOrEmpty(result.RejectMessage));
        }
        [Fact]
        public async Task CreateLobbyAsync_NonEmptyNickname()
        {
            //Arrange
            
            //Act

            //Assert

        }
        [Fact]
        public async Task CreateLobbyAsync_Rejects_EmptyNickname()
        {
            //Arrange
            var request_empty = new CreateLobbyRequest()
            {
                GameType = "tictactoe",
                Nickname = ""
            };
            var request_null = new CreateLobbyRequest()
            {
                GameType = "tictactoe",
                Nickname = null
            };
            //Act
            var result_null = await manager.CreateLobbyAsync(request_null);
            var result_empty = await manager.CreateLobbyAsync(request_empty);
            //Assert
            Assert.False(result_null.Success);
            Assert.False(result_empty.Success);
        }
    }
}
