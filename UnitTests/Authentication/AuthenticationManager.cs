using System;
using Xunit;
using Moq;
using NXO.Server.Dependencies;
using NXO.Shared.Models;
using System.Threading.Tasks;

namespace NXO.UnitTests
{ 
    public class IAuthenticationManagerTests
    {
        internal Mock<ILobbyCoordinator> lobbyCoordinator = new Mock<ILobbyCoordinator>();
        internal AuthenticationManager manager { get; set; }
        public IAuthenticationManagerTests()
        {
            manager = new AuthenticationManager(lobbyCoordinator.Object);
        }
        [Fact]
        public async Task LobbyCodeNull()
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
        public async Task LobbyCodeEmpty()
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
        public async Task LobbyDoesNotExist()
        {
            //Arrange
            var code = "TEST";
            var input = new JoinRequest()
            {
                GameCode = code,
                Nickname = "Anything"
            };
            lobbyCoordinator.Setup(i => i.CanJoinAsync(code)).ReturnsAsync(false);
            //Act
            var result = await manager.AttemptJoinAsync(input);
            //Assert
            Assert.False(result.Success);
            Assert.Null(result.PlayerId);
            Assert.True(!string.IsNullOrEmpty(result.RejectMessage));
        }
        [Fact]
        public async Task LobbyExists_NicknameEmpty()
        {
             //Arrange
            var code = "MORETEST";
            var input = new JoinRequest()
            {
                GameCode = code,
                Nickname = "Anything"
            };
            lobbyCoordinator.Setup(i => i.CanJoinAsync(code)).ReturnsAsync(true);
            //Act
            var result = await manager.AttemptJoinAsync(input);
            //Assert
            Assert.False(result.Success);
            Assert.Null(result.PlayerId);
            Assert.True(!string.IsNullOrEmpty(result.RejectMessage));
        }
        [Fact]
        public void LobbyExists_NicknameNotEmpty()
        {

        }
    }
}
