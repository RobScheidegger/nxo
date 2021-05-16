using NXO.Server.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NXO.UnitTests.Management
{
    public class GuidProviderTests
    {
        private readonly GuidProvider provider;
        public GuidProviderTests()
        {
            provider = new GuidProvider();
        }

        [Fact]
        public void New_GeneratesGuid()
        {
            //Act
            var guid = provider.New();
            //Assert
            Assert.True(!string.IsNullOrEmpty(guid));
        }
        [Fact]
        public void New_Unique()
        {
            //Act
            var guid1 = provider.New();
            var guid2 = provider.New();
            //Assert
            Assert.NotEqual(guid1, guid2);
        }
        [Fact]
        public void LobbyCode_Alphabetic()
        {
            //Act
            var code = provider.NewLobbyCode();
            //Assert
            Assert.True(code.All(char.IsLetter));
            
        }
    }
}
