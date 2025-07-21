using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.Util;
using Xunit;

namespace WarlockGame.Core.Test.Game.Util;

[TestSubject(typeof(TextUtil))]
public class TextUtilTest {
    [Theory]
    [InlineData("", "")]
    [InlineData("123", "123")]
    [InlineData("1 2", "1 2")]
    [InlineData("12345", "12345")]
    [InlineData("1 2 3 4 5", "1 2 3\n4 5")]
    [InlineData("12345 1", "12345\n1")]
    [InlineData("123456", "12345\n6")]
    [InlineData("12345678901", "12345\n67890\n1")]
    [InlineData("1 123456", "1\n12345\n6")]
    public void TextIsWrappedCorrectlySimple(string input, string expected) {
        TextUtil.WrapText(input, x => new Vector2(x.Length, 1), 5).Should().Be(expected);
    }
    
    [Theory]
    [InlineData(0, "", "")]
    [InlineData(1, "", "")]
    [InlineData(2, "", "")]
    [InlineData(0, "123", "123")]
    [InlineData(2, "12345 1", "12345")]
    [InlineData(2, "1 2345", "1 234")]
    [InlineData(4, "12345 1", "12345\n1")]
    [InlineData(6, "12345678901", "12345\n67890\n1")]
    [InlineData(4, "12345678901", "12345\n67890")]
    public void TextIsWrappedCorrectlyMaxHeight(int maxHeight, string input, string expected) {
        TextUtil.WrapText(input, x => new Vector2(x.Length, 2 * (x.Length / 5 + 1)), 5, maxHeight: maxHeight)
            .Should().Be(expected);
    }
    
    [Theory]
    [InlineData(0, "", "")]
    [InlineData(1, "", "")]
    [InlineData(2, "", "")]
    [InlineData(1, "123", "123")]
    [InlineData(1, "1 234", "1 234")]
    [InlineData(2, "12345 1", "12345\n1")]
    [InlineData(1, "12345 1", "123..")]
    [InlineData(1, "1 2345", "1 2..")]
    [InlineData(2, "12345678901", "12345\n678..")]
    public void TextIsWrappedCorrectlyWithTruncationCharacter(int maxLines, string input, string expected) {
        TextUtil.WrapText(input, x => new Vector2(x.Length, 2 * (x.Length / 5 + 1)), 5, maxLines: maxLines, truncation: "..")
            .Should().Be(expected);
    }
}