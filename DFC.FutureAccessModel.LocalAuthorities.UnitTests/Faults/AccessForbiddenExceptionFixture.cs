using System;
using System.Runtime.Serialization;
using Xunit;

namespace DFC.FutureAccessModel.LocalAuthorities.Faults
{
    /// <summary>
    /// the access forbidden exception fixture
    /// </summary>
    public sealed class AccessForbiddenExceptionFixture
    {
        /// <summary>
        /// the system under test holds it's exception messsage
        /// </summary>
        [Fact]
        public void TheSystemUnderTestHoldsItsExceptionMesssage()
        {
            // arrange
            var sut = MakeSUT();

            // act
            var message = sut.Message;

            // assert
            Assert.Equal("Insufficient access to this resource", message);
        }

        /// <summary>
        /// the system under test supports it's service contract
        /// </summary>
        [Fact]
        public void TheSystemUnderTestSupportsItsServiceContract()
        {
            // arrange / act / assert
            Assert.IsAssignableFrom<Exception>(MakeSUT());
        }

        /// <summary>
        /// the system under test is serialisable
        /// </summary>
        [Fact]
        public void TheSystemUnderTestIsSerializable()
        {
            // arrange / act / assert
            Assert.IsAssignableFrom<ISerializable>(MakeSUT());
        }

        /// <summary>
        /// make a 'system under test'
        /// </summary>
        /// <returns>the system under test</returns>
        internal AccessForbiddenException MakeSUT() =>
            new AccessForbiddenException();
    }
}
