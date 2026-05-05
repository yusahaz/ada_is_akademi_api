namespace Azoxia.AdaIsAkademi.SeedRunner;

/// <summary>
/// Parsed CLI options for the deterministic demo-data seed runner.
/// </summary>
public sealed class SeedOptions
{
    #region Properties

    /// <summary>
    /// When true, allows running against Production environment name (dangerous).
    /// </summary>
    public bool AllowProduction { get; init; }

    /// <summary>
    /// Optional PostgreSQL connection string override (highest precedence).
    /// </summary>
    public string? ConnectionString { get; init; }

    /// <summary>
    /// Number of closed job postings to generate (Completed/Filled/Cancelled mix).
    /// </summary>
    public int ClosedPostings { get; init; } = 50;

    /// <summary>
    /// Number of employer organizations.
    /// </summary>
    public int Employers { get; init; } = 20;

    /// <summary>
    /// Number of open job postings.
    /// </summary>
    public int OpenPostings { get; init; } = 50;

    /// <summary>
    /// When true, truncates seed-owned tables and re-seeds from scratch (keeps migration admin user).
    /// </summary>
    public bool Reset { get; init; }

    /// <summary>
    /// Deterministic seed for Bogus and System.Random.
    /// </summary>
    public int Seed { get; init; } = 12_345;

    /// <summary>
    /// Number of worker profiles.
    /// </summary>
    public int Workers { get; init; } = 100;

    #endregion Properties

    #region Utils

    /// <summary>
    /// Parses CLI arguments into <see cref="SeedOptions"/>.
    /// </summary>
    /// <param name="args">Raw arguments (excluding executable).</param>
    /// <returns>Merged options with defaults.</returns>
    public static SeedOptions Parse(string[] args)
    {
        bool reset = false;
        bool allowProduction = false;
        string? connectionString = null;
        int workers = 100;
        int employers = 20;
        int openPostings = 50;
        int closedPostings = 50;
        int seed = 12_345;

        for (int i = 0; i < args.Length; i++)
        {
            string a = args[i];
            switch (a)
            {
                case "--reset":
                    reset = true;
                    break;
                case "--allow-production":
                    allowProduction = true;
                    break;
                case "--workers":
                    workers = int.Parse(args[++i]);
                    break;
                case "--employers":
                    employers = int.Parse(args[++i]);
                    break;
                case "--open-postings":
                    openPostings = int.Parse(args[++i]);
                    break;
                case "--closed-postings":
                    closedPostings = int.Parse(args[++i]);
                    break;
                case "--seed":
                    seed = int.Parse(args[++i]);
                    break;
                case "--connection-string":
                    connectionString = args[++i];
                    break;
            }
        }

        return new SeedOptions
        {
            Reset = reset,
            AllowProduction = allowProduction,
            ConnectionString = connectionString,
            Workers = workers,
            Employers = employers,
            OpenPostings = openPostings,
            ClosedPostings = closedPostings,
            Seed = seed,
        };
    }

    #endregion Utils
}
