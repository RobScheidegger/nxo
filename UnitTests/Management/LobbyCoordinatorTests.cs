using System;
using Xunit;
using Moq;
using NXO.Server.Dependencies;
using NXO.Shared.Models;
using System.Threading.Tasks;
using NXO.Shared.Repository;

namespace NXO.UnitTests.Management
{ 
    public class LobbyCoordinatorTests
    {
        internal LobbyCoordinator manager { get; set; }
        Mock<IRepository<Game>> gameMock = new Mock<IRepository<Game>>();
        Mock<IGuidProvider> guidMock = new Mock<IGuidProvider>();
        public LobbyCoordinatorTests()
        {
            manager = new LobbyCoordinator(gameMock.Object, guidMock.Object, null);
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
            var code = "MORETEST";
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
