namespace Azoxia.AdaIsAkademi.DomainTests;

using Azoxia.AdaIsAkademi.Domain;
using FluentAssertions;

public class SkillLabelNormalizerTests
{
    [Theory]
    [InlineData("barista", "Barista")]
    [InlineData("BARISTA", "Barista")]
    [InlineData("  team leader  ", "Team Leader")]
    [InlineData("TEAM LEADER", "Team Leader")]
    [InlineData("siparis_alma", "Siparis Alma")]
    [InlineData("SIPARIS_ALMA", "Siparis Alma")]
    [InlineData("  CSharp  ", "CSharp")]
    [InlineData("csharp", "Csharp")]
    [InlineData("CSHARP", "Csharp")]
    [InlineData("sipariş alma", "Sipariş Alma")]
    [InlineData("SIPARIŞ_ALMA", "Sipariş Alma")]
    [InlineData("İSTANBUL", "İstanbul")]
    [InlineData("mağaza müdürü", "Mağaza Müdürü")]
    public void ToDisplayPascalCase_normalizes_with_turkish_letter_aware_casing(string input, string expected)
    {
        SkillLabelNormalizer.ToDisplayPascalCase(input).Should().Be(expected);
    }

    [Theory]
    [InlineData("guest communication", "GuestCommunication")]
    [InlineData("GUEST COMMUNICATION", "GuestCommunication")]
    [InlineData("GuestCommunication", "GuestCommunication")]
    [InlineData("sipariş alma", "SiparişAlma")]
    public void ToCompoundPascalCase_joins_without_spaces(string input, string expected)
    {
        SkillLabelNormalizer.ToCompoundPascalCase(input).Should().Be(expected);
    }

    [Fact]
    public void SkillTag_normalizes_to_display_pascal_case()
    {
        SkillTag tag = new("  barista  ");
        ((string)tag).Should().Be("Barista");
    }
}
