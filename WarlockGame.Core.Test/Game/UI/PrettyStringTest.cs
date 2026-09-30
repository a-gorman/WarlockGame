using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using WarlockGame.Core.Game.UI;
using Xunit;

namespace WarlockGame.Core.Test.Game.UI;

[TestSubject(typeof(PrettyString))]
public class PrettyStringTest {

    [Theory]
    [InlineData("a")]
    [InlineData(" ")]
    [InlineData("aslkdglaksjghpoki asdlgkj asdgiosdafh asdf!@\\%^%_)({}")]
    public void SimpleParsing(string input) {
        PrettyString.ParseTokens(input).Single().Value.As<string>().Should().Be(input);
    }
    
    [Theory]
    [InlineData("$token_1", "aaaa")]
    [InlineData(" $token_1 $token_3 ", " aaaa $token_1<> ")]
    public void CanInterpolateTokens(string input, string expectedResult) {
        var tokens = new Dictionary<string, string> {
            { "token_1", "aaaa" },
            { "token_2", "bb" },
            { "token_3", "$token_1<>" }
        };
        PrettyString.ParseTokens(input, tokens).Single().Value.As<string>().Should().Be(expectedResult);
    }

    [Fact]
    public void ComplicatedCase() {
        var variables = new Dictionary<string, string> {
            { "token_1", "aaaa" },
            { "token_2", "bb" },
            { "token_3", "$token_1<>" },
            { "colorToken", "Blue" }
        };
        
        PrettyString.ParseTokens(" $token_1#c=Green  <#c=Red bb <$token_3 #c=Blue>ee> d### #c=$colorToken$a", variables)
            .Select(x => x.Value)
            .Should()
            .ContainInConsecutiveOrder(new List<PrettyString.Token> {
                " aaaa",
                new PrettyString.Formatting { Color = Color.Green },
                " ",
                new PrettyString.Formatting { Color = Color.Red },
                "bb $token_1<> ",
                new PrettyString.Formatting { Color = Color.Red },
                "ee",
                new PrettyString.Formatting { Color = Color.Green },
                " d# ",
                new PrettyString.Formatting { Color = Color.Blue },
                "a"
            }.Select(x => x.Value));
    }
    
    [Fact]
    public void CanParseColorVariable() {
        var variables = new Dictionary<string, string> {
            { "colorToken", "Blue" }
        };
        
        PrettyString.ParseTokens("#c=$colorToken$a", variables)
            .Select(x => x.Value)
            .Should()
            .ContainInConsecutiveOrder(new List<PrettyString.Token> {
                new PrettyString.Formatting { Color = Color.Blue },
                "a"
            }.Select(x => x.Value));
    }
    
    [Theory]
    [MemberData("FormatTokenDataProvider")]
    public void CanParseFormatTokens(string input, List<PrettyString.Token> expectedResult) {
        PrettyString.ParseTokens(input)
            .Select(x => x.Value)
            .Should()
            .ContainInConsecutiveOrder(expectedResult.Select(x => x.Value));
    }

    public static List<object[]> FormatTokenDataProvider() {
        return [
            [
                "<>",
                new List<PrettyString.Token> { }
            ],
            [
                "a<>b",
                new List<PrettyString.Token> {
                    "a",
                    "b"
                }
            ],
            [
                "a<c>b",
                new List<PrettyString.Token> {
                    "a",
                    "c",
                    "b"
                }
            ],
            [
                " a <#c=Red> b ",
                new List<PrettyString.Token> {
                    " a ",
                    new PrettyString.Formatting { Color = Color.Black },
                    " b "
                }
            ],
            [
                " a <#c=Red hello> b ",
                new List<PrettyString.Token> {
                    " a ",
                    new PrettyString.Formatting { Color = Color.Red },
                    "hello",
                    new PrettyString.Formatting { Color = Color.Black },
                    " b "
                }
            ],
            [
                " a <#c=Red bb <#c=Blue cc>ee> d ",
                new List<PrettyString.Token> {
                    " a ",
                    new PrettyString.Formatting { Color = Color.Red },
                    "bb ",
                    new PrettyString.Formatting { Color = Color.Blue },
                    "cc",
                    new PrettyString.Formatting { Color = Color.Red },
                    "ee",
                    new PrettyString.Formatting { Color = Color.Black },
                    " d "
                }
            ]
        ];
    }
}