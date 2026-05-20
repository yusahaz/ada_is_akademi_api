namespace Azoxia.AdaIsAkademi.Domain
{
    using System.Globalization;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Normalizes skill labels to PascalCase. Turkish letters (ç, ğ, ı, ö, ş, ü, İ) use <c>tr-TR</c> per character;
    /// other letters use <see cref="CultureInfo.InvariantCulture"/> so ASCII <c>I</c>/<c>i</c> are not mapped to <c>ı</c>/<c>İ</c>.
    /// </summary>
    public static partial class SkillLabelNormalizer
    {
        private static readonly CultureInfo Turkish = CultureInfo.GetCultureInfo("tr-TR");
        private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

        /// <summary>
        /// Display form: words separated by a single space (e.g. <c>Team Leader</c>, <c>Sipariş Alma</c>).
        /// </summary>
        public static string ToDisplayPascalCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            IEnumerable<string> tokens = Tokenize(value);
            List<string> words = [];
            foreach (string token in tokens)
            {
                words.AddRange(NormalizeTokenToWords(token));
            }

            return string.Join(" ", words);
        }

        /// <summary>
        /// Compound form without spaces (e.g. <c>GuestCommunication</c>) for <see cref="JobSkill"/> dictionary names.
        /// </summary>
        public static string ToCompoundPascalCase(string value)
        {
            string display = ToDisplayPascalCase(value);
            if (display.Length == 0)
            {
                return display;
            }

            return display.Replace(" ", string.Empty, StringComparison.Ordinal);
        }

        private static IEnumerable<string> Tokenize(string value)
        {
            string trimmed = value.Trim();
            string expanded = SeparatorRegex().Replace(trimmed, " ");
            return expanded
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        private static IEnumerable<string> NormalizeTokenToWords(string token)
        {
            if (token.Length == 0)
            {
                yield break;
            }

            foreach (string segment in SplitCamelCaseSegments(token))
            {
                yield return NormalizeWord(segment);
            }
        }

        private static IEnumerable<string> SplitCamelCaseSegments(string token)
        {
            if (token.Length == 0)
            {
                yield break;
            }

            int start = 0;
            for (int index = 1; index < token.Length; index++)
            {
                char previous = token[index - 1];
                char current = token[index];

                if (!char.IsLower(previous) || !char.IsUpper(current))
                {
                    continue;
                }

                yield return token[start..index];
                start = index;
            }

            yield return token[start..];
        }

        private static string NormalizeWord(string word)
        {
            if (word.Length == 0)
            {
                return word;
            }

            bool hasLower = false;
            bool hasUpper = false;
            foreach (char character in word)
            {
                if (char.IsLower(character))
                {
                    hasLower = true;
                }

                if (char.IsUpper(character))
                {
                    hasUpper = true;
                }
            }

            if (hasUpper && hasLower)
            {
                return word;
            }

            string lower = ToLowerHybrid(word);
            CultureInfo firstCharCulture = ContainsTurkishSpecificLetter(word) ? Turkish : Invariant;
            return char.ToUpper(lower[0], firstCharCulture) + lower[1..];
        }

        private static string ToLowerHybrid(string word)
        {
            char[] lower = new char[word.Length];
            for (int index = 0; index < word.Length; index++)
            {
                char character = word[index];
                CultureInfo culture = IsTurkishSpecificLetter(character) ? Turkish : Invariant;
                lower[index] = char.ToLower(character, culture);
            }

            return new string(lower);
        }

        private static bool ContainsTurkishSpecificLetter(string value)
        {
            foreach (char character in value)
            {
                if (IsTurkishSpecificLetter(character))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsTurkishSpecificLetter(char character) =>
            character is 'ç' or 'Ç' or 'ğ' or 'Ğ' or 'ı' or 'İ' or 'ö' or 'Ö' or 'ş' or 'Ş' or 'ü' or 'Ü';

        [GeneratedRegex(@"[\s_\-\/]+", RegexOptions.CultureInvariant)]
        private static partial Regex SeparatorRegex();
    }
}
