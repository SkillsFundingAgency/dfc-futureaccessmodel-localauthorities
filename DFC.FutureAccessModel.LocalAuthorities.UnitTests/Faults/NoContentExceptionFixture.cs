using System;
using System.Runtime.Serialization;
using Xunit;

namespace DFC.FutureAccessModel.LocalAuthorities.Faults
{
    /// <summary>
    /// the no content exception fixture
    /// </summary>
    public sealed class NoContentExceptionFixture
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
            Assert.Equal("Resource does not exist", message);
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
        internal NoContentException MakeSUT() =>
            new NoContentException();
    }
}
