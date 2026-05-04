namespace Azoxia.AdaIsAkademi.Api
{
    using Azoxia.AdaIsAkademi.Api.DependencyInjection;
    using Azoxia.Core.Api;

    /// <summary>
    /// Host process entry type for the Ada Is Akademi API.
    /// </summary>
    class Program
    {
        #region Utils

        /// <summary>
        /// Boots the API host using the shared startup pipeline.
        /// </summary>
        /// <param name="args">Raw command-line arguments.</param>
        static void Main(string[] args)
        {
            Startup startup = new();

            startup.OnConfigureServices += (builder) =>
            {
                builder.Services.AddAzoxiaCore(builder.Configuration);
            };

            startup.Run(args);
        }

        #endregion Utils
    }
}
