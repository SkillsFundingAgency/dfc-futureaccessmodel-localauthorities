using System.ComponentModel.DataAnnotations;
using Xunit;

namespace DFC.FutureAccessModel.LocalAuthorities.Factories.Internal
{
    /// <summary>
    /// the validation message content factory fixture
    /// </summary>
    public sealed class ValidationMessageContentFactoryFixture :
        MoqTestingFixture
    {
        /// <summary>
        /// the system under test supports it's service contract
        /// </summary>
        [Fact]
        public void TheSystemUnderTestSupportsItsServiceContract()
        {
            // arrange / act / assert
            Assert.IsAssignableFrom<ICreateValidationMessageContent>(MakeSUT());
        }

        /// <summary>
        /// create with null returns empty errors
        /// </summary>
        [Fact]
        public void CreateWithNullReturnsEmptyErrors()
        {
            // arrange
            var sut = MakeSUT();

            // act
            var result = sut.Create(null);

            // assert
            Assert.NotNull(result);
            Assert.Equal("{\"errors\":[]}", result);
        }

        /// <summary>
        /// create with validation result returns errors
        /// </summary>
        [Fact]
        public void CreateWithValidationResultReturnsErrors()
        {
            // arrange
            var sut = MakeSUT();
            var validation = new ValidationResult("test error", new string[] { "testProperty" });

            // act
            var result = sut.Create(new[] { validation });

            // assert
            Assert.NotNull(result);
            Assert.Equal("{\"errors\":[{\"code\":\"testProperty\",\"message\":\"test error\"}]}", result);
        }

        /// <summary>
        /// make (a) 'system under test'
        /// </summary>
        /// <returns>the system under test</returns>
        internal ValidationMessageContentFactory MakeSUT() =>
            new ValidationMessageContentFactory();
    }
}
