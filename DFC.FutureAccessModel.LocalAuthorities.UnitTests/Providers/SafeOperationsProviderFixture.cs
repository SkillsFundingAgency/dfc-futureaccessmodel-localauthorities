using System;
using System.Threading.Tasks;
using DFC.FutureAccessModel.LocalAuthorities.Faults;
using Xunit;

namespace DFC.FutureAccessModel.LocalAuthorities.Providers.Internal
{
    /// <summary>
    /// the safe operations provider fixture
    /// </summary>
    public sealed class SafeOperationsProviderFixture :
        MoqTestingFixture
    {
        /// <summary>
        /// the system under test supports it's service contract
        /// </summary>
        [Fact]
        public void TheSystemUnderTestSupportsItsServiceContract()
        {
            // arrange / act / assert
            Assert.IsAssignableFrom<IProvideSafeOperations>(MakeSUT());
        }

        /// <summary>
        /// void try with exception meets expectation
        /// </summary>
        /// <param name="expectedException">the expected exception</param>
        /// <returns>the currently running (test) task</returns>
        [Theory]
        [InlineData(typeof(MalformedRequestException))]
        [InlineData(typeof(NoContentException))]
        [InlineData(typeof(UnprocessableEntityException))]
        [InlineData(typeof(ArgumentNullException))]
        public async Task VoidTryWithExceptionMeetsExpectation(Type expectedException)
        {
            // arrange
            var sut = MakeSUT();
            var exception = (Exception)expectedException.Assembly.CreateInstance(expectedException.FullName);

            // act / assert
            await sut.Try(
                () => throw exception,
                x => { Assert.IsType(expectedException, x); return Task.CompletedTask; });
        }

        /// <summary>
        /// void try meets expectation
        /// </summary>
        /// <returns>the currently running (test) task</returns>
        [Fact]
        public async Task VoidTryMeetsExpectation()
        {
            // arrange
            var sut = MakeSUT();
            var threwError = false;
            var testItem = 1;

            // act / assert
            await sut.Try(
                () => Task.Run(() => testItem++),
                x => { threwError = true; return Task.CompletedTask; });

            // assert
            Assert.Equal(2, testItem);
            Assert.False(threwError);
        }

        /// <summary>
        /// try with result meets expectation
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="expectedResult"></param>
        /// <param name="expectedState"></param>
        /// <returns>the currently running (test) task</returns>
        [Theory]
        [InlineData(0, 0, -1, true)] // this will throw divide by zero exception
        [InlineData(0, 1, 0, false)]
        [InlineData(1, 1, 1, false)]
        [InlineData(1, 0, -1, true)] // this will throw divide by zero exception
        public async Task TryWithResultMeetsExpectation(int first, int second, int expectedResult, bool expectedState)
        {
            // arrange
            var threwError = false;
            var sut = MakeSUT();

            // act
            var result = await sut.Try(
                () => Task.FromResult(first / second),
                x => { threwError = true; return Task.FromResult(-1); });

            // assert
            Assert.Equal(expectedResult, result);
            Assert.Equal(expectedState, threwError);
        }

        /// <summary>
        /// make a 'system under test'
        /// </summary>
        /// <returns>the system under test</returns>
        internal SafeOperationsProvider MakeSUT() =>
            new SafeOperationsProvider();
    }
}
