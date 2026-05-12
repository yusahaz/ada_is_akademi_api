namespace Azoxia.AdaIsAkademi.Domain
{
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Value object that stores worker CV render options.
    /// </summary>
    public readonly record struct CvOptions
    {
        private const int MaxLength = 1024;

        #region Ctors

        /// <summary>
        /// Initializes CV options from serialized JSON payload.
        /// </summary>
        /// <param name="value">Serialized CV options payload.</param>
        public CvOptions(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            string normalized = value.Trim();
            if (normalized.Length > MaxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"CV options payload exceeds max length {MaxLength}.");
            }

            if (!normalized.StartsWith('{'))
            {
                // Backward compatibility for legacy rows that store only template id.
                TemplateId = normalized;
                LayoutVariant = "double";
                Palette = "slate";
                Version = 1;
                return;
            }

            CvOptionsPayload? payload = JsonSerializer.Deserialize<CvOptionsPayload>(normalized);
            TemplateId = payload?.TemplateId?.Trim() ?? string.Empty;
            LayoutVariant = payload?.LayoutVariant?.Trim() ?? string.Empty;
            Palette = payload?.Palette?.Trim() ?? string.Empty;
            Version = payload?.Version ?? 1;
            Validate();
        }

        /// <summary>
        /// Initializes CV options with explicit fields.
        /// </summary>
        public CvOptions(string templateId, string layoutVariant, string palette, int version = 1)
        {
            TemplateId = templateId?.Trim() ?? string.Empty;
            LayoutVariant = layoutVariant?.Trim() ?? string.Empty;
            Palette = palette?.Trim() ?? string.Empty;
            Version = version;
            Validate();
        }

        #endregion Ctors

        #region Properties

        /// <summary>
        /// Selected template id.
        /// </summary>
        public string TemplateId { get; }

        /// <summary>
        /// Selected layout variant.
        /// </summary>
        public string LayoutVariant { get; }

        /// <summary>
        /// Selected color palette.
        /// </summary>
        public string Palette { get; }

        /// <summary>
        /// CV options schema version.
        /// </summary>
        public int Version { get; }

        /// <summary>
        /// Serialized CV options payload.
        /// </summary>
        public string Value => ToJson();

        #endregion Properties

        #region Methods

        /// <summary>
        /// Serializes current options into JSON payload.
        /// </summary>
        public string ToJson()
        {
            string json = JsonSerializer.Serialize(new CvOptionsPayload
            {
                TemplateId = TemplateId,
                LayoutVariant = LayoutVariant,
                Palette = Palette,
                Version = Version,
            });
            if (json.Length > MaxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(json), $"CV options payload exceeds max length {MaxLength}.");
            }

            return json;
        }

        private void Validate()
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(TemplateId);
            ArgumentException.ThrowIfNullOrWhiteSpace(LayoutVariant);
            ArgumentException.ThrowIfNullOrWhiteSpace(Palette);
            if (Version <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Version), "CV options schema version must be positive.");
            }
        }

        #endregion Methods

        #region Operators

        /// <summary>
        /// Converts raw payload text to <see cref="CvOptions"/>.
        /// </summary>
        /// <param name="value">Serialized CV options payload.</param>
        public static implicit operator CvOptions(string value) => new(value);

        /// <summary>
        /// Returns serialized payload text.
        /// </summary>
        /// <param name="value">CV options instance.</param>
        public static implicit operator string(CvOptions value) => value.ToJson();

        #endregion Operators

        private sealed class CvOptionsPayload
        {
            [JsonPropertyName("templateId")]
            public string? TemplateId { get; set; }

            [JsonPropertyName("layoutVariant")]
            public string? LayoutVariant { get; set; }

            [JsonPropertyName("palette")]
            public string? Palette { get; set; }

            [JsonPropertyName("version")]
            public int Version { get; set; }
        }
    }
}
