using Xunit;

namespace DFC.FutureAccessModel.LocalAuthorities.Factories.Internal
{
    /// <summary>
    /// the validation message fixture
    /// </summary>
    public sealed class ValidationMessageFixture :
        MoqTestingFixture
    {
        [Theory]
        [InlineData("1", "2", "3")]
        [InlineData("1")]
        [InlineData("1", "2", "3","4","5","6","7")]
        [InlineData("1", "2", "3", "4")]
        public void AddMeetsVerification(params string[] codes)
        {
            // arrange
            const string message = "recording a message";

            var sut = MakeSUT();

            // act
            foreach(var code in codes)
                sut.Add(code, message);

            // assert
            Assert.Equal(codes.Length, sut.Messages.Count);
        }

        /// <summary>
        /// make (a) 'system under test'
        /// </summary>
        /// <returns>the system under test</returns>
        internal ValidationMessage MakeSUT() =>
            new ValidationMessage();
    }
}
