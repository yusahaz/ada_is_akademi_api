namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Queries;

    /// <summary>
    /// Loads full employer detail by employer id.
    /// </summary>
    public class GetEmployerDetailQuery :
        QueryBase<EmployerFullDetailModel>
    {
        #region Properties

        /// <summary>
        /// Employer primary key.
        /// </summary>
        public int EmployerId { get; set; }

        #endregion Properties
    }
}
