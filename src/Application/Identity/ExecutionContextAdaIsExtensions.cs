namespace Azoxia.AdaIsAkademi.Application.Identity
{
    using Azoxia.AdaIsAkademi.Application;
    using Azoxia.Core.Extensions;
    using Azoxia.Core.Identity;

    /// <summary>
    /// Resolves Ada İş Akademi actor identifiers from bearer claims (<c>employer_id</c>, <c>worker_id</c>).
    /// </summary>
    internal static class ExecutionContextAdaIsExtensions
    {
        #region Methods

        /// <summary>
        /// Returns a positive employer id from the <c>employer_id</c> claim or throws a validation error when the claim is absent or invalid.
        /// </summary>
        /// <param name="context">Current HTTP-backed or test execution context.</param>
        internal static int RequireAdaIsEmployerActorId(this IExecutionContext context)
        {
            if (int.TryParse(context.GetClaim("employer_id"), out int id) &&
                id > 0)
            {
                return id;
            }

            ApplicationValidationCodes.ActorEmployerIdClaimRequired.Throw();
            return 0;
        }

        /// <summary>
        /// Returns a positive worker id from the <c>worker_id</c> claim or throws a validation error when the claim is absent or invalid.
        /// </summary>
        /// <param name="context">Current HTTP-backed or test execution context.</param>
        internal static int RequireAdaIsWorkerActorId(this IExecutionContext context)
        {
            if (int.TryParse(context.GetClaim("worker_id"), out int id) &&
                id > 0)
            {
                return id;
            }

            ApplicationValidationCodes.ActorWorkerIdClaimRequired.Throw();
            return 0;
        }

        #endregion Methods
    }
}
