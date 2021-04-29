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
            Console.WriteLine(guid);
        }
    }
}
