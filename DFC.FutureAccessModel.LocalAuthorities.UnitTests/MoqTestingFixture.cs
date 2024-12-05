using Moq;

namespace DFC.FutureAccessModel.LocalAuthorities
{
    public abstract class MoqTestingFixture
    {
        /// <summary>
        /// make strict mock
        /// </summary>
        /// <typeparam name="TEntity">for this type</typeparam>
        /// <returns>a strict behaviour mock</returns>
        public TEntity MakeStrictMock<TEntity>()
            where TEntity : class =>
            new Mock<TEntity>(MockBehavior.Strict).Object;

        /// <summary>
        /// get mock
        /// </summary>
        /// <typeparam name="TEntity">the type</typeparam>
        /// <param name="forItem">for this instance of <typeparamref name="TEntity"/>the type</param>
        /// <returns>the mock</returns>
        public Mock<TEntity> GetMock<TEntity>(TEntity forItem)
            where TEntity : class =>
            Mock.Get(forItem);
    }
}
