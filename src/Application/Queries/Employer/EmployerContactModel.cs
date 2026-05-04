namespace Azoxia.AdaIsAkademi.Application
{
    using Azoxia.Core.Application.Models;

    /// <summary>
    /// Flattened contact fields for employer detail read models.
    /// </summary>
    /// <param name="FirstName">Given name.</param>
    /// <param name="LastName">Family name.</param>
    /// <param name="Email">Email address.</param>
    /// <param name="Phone">Phone number.</param>
    public sealed record EmployerContactModel(
        string FirstName,
        string LastName,
        string Email,
        string Phone) :
        ModelBase;
}
