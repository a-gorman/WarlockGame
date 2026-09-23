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
    [InlineData("aslkdglaksjghpoki asdlgkj asdgiosdafh asdf!@#%^%_)({}")]
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
        var tokens = new Dictionary<string, string> {
            { "token_1", "aaaa" },
            { "token_2", "bb" },
            { "token_3", "$token_1<>" }
        };
        
        PrettyString.ParseTokens(" $token_1 <c=Red t='bb <t='$token_3' c=Blue>ee'> d ", tokens)
            .Select(x => x.Value)
            .Should()
            .ContainInConsecutiveOrder(new List<PrettyString.Token> {
                " aaaa ",
                new PrettyString.Formatting { Color = Color.Red },
                "bb ",
                new PrettyString.Formatting { Color = Color.Blue },
                "$token_1<>",
                new PrettyString.Formatting { Color = Color.Red },
                "ee",
                new PrettyString.Formatting { Color = Color.Black },
                " d "
            }.Select(x => x.Value));
    }
    
    [Theory]
    [MemberData("DataProvider")]
    public void CanParseFormatTokens(string input, List<PrettyString.Token> expectedResult) {
        PrettyString.ParseTokens(input)
            .Select(x => x.Value)
            .Should()
            .ContainInConsecutiveOrder(expectedResult.Select(x => x.Value));
    }

    public static List<object[]> DataProvider() {
        return [
            [
                "<>",
                new List<PrettyString.Token> {
                    new PrettyString.Formatting { Color = Color.Black }
                }
            ],
            [
                "a<>b",
                new List<PrettyString.Token> {
                    "a",
                    new PrettyString.Formatting { Color = Color.Black },
                    "b"
                }
            ],
            [
                "a<t='c'>b",
                new List<PrettyString.Token> {
                    "a",
                    new PrettyString.Formatting { Color = Color.Black },
                    "c",
                    new PrettyString.Formatting { Color = Color.Black },
                    "b"
                }
            ],
            [
                " a <c=Red> b ",
                new List<PrettyString.Token> {
                    " a ",
                    new PrettyString.Formatting { Color = Color.Red },
                    " b "
                }
            ],
            [
                " a <c=Red t='hello'> b ",
                new List<PrettyString.Token> {
                    " a ",
                    new PrettyString.Formatting { Color = Color.Red },
                    "hello",
                    new PrettyString.Formatting { Color = Color.Black },
                    " b "
                }
            ],
            [
                " a <c=Red t='bb <t='cc' c=Blue>ee'> d ",
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