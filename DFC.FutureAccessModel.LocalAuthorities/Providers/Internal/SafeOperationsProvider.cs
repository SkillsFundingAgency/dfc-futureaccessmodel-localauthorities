namespace DFC.FutureAccessModel.LocalAuthorities.Providers.Internal
{
    /// <summary>
    /// the safe operations providers
    /// </summary>
    internal sealed class SafeOperationsProvider :
        IProvideSafeOperations
    {
        /// <summary>
        /// try...
        /// </summary>
        /// <param name="doAction">do action</param>
        /// <param name="handleError">handle error (can be null, optional)</param>
        /// <returns>the currently running task</returns>
        public async Task Try(Func<Task> doAction, Func<Exception, Task> handleError)
        {
            try
            {
                await doAction();
            }
            catch (Exception e)
            {
                await handleError?.Invoke(e);
            }
        }

        /// <summary>
        /// try...
        /// </summary>
        /// <typeparam name="TResult">the return type for the action and the error</typeparam>
        /// <param name="doAction">do action</param>
        /// <param name="handleError">handle error (cannot be null)</param>
        /// <returns>the currently running task with the requested result type</returns>
        public async Task<TResult> Try<TResult>(Func<Task<TResult>> doAction, Func<Exception, Task<TResult>> handleError)
        {
            try
            {
                return await doAction();
            }
            catch (Exception e)
            {
                return await handleError(e);
            }
        }
    }
}
