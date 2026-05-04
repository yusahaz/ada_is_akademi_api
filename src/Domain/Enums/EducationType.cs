namespace Azoxia.AdaIsAkademi.Domain
{
    /// <summary>
    /// Kind of formal education credential associated with a worker.
    /// </summary>
    public enum EducationType
    {
        /// <summary>
        /// Two-year associate-level degree.
        /// </summary>
        AssociateDegree = 30,

        /// <summary>
        /// Four-year undergraduate degree.
        /// </summary>
        BachelorDegree = 40,

        /// <summary>
        /// Doctoral or equivalent terminal degree.
        /// </summary>
        Doctorate = 60,

        /// <summary>
        /// Secondary school completion or equivalent.
        /// </summary>
        HighSchool = 10,

        /// <summary>
        /// Graduate-level degree beyond bachelor.
        /// </summary>
        MasterDegree = 50,

        /// <summary>
        /// Other or unspecified education path.
        /// </summary>
        Other = 99,

        /// <summary>
        /// Short vocational or certificate-style program.
        /// </summary>
        VocationalCourse = 20,
    }
}
