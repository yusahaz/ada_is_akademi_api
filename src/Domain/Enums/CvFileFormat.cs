namespace Azoxia.AdaIsAkademi.Domain
{
    /// <summary>
    /// Supported file formats for worker CV upload sessions.
    /// </summary>
    public enum CvFileFormat
    {
        /// <summary>
        /// Portable Document Format file.
        /// </summary>
        Pdf = 10,

        /// <summary>
        /// Microsoft Word OpenXML document format.
        /// </summary>
        Docx = 20,
    }
}
