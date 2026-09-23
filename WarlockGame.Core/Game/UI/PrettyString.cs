using System;
using System.Collections.Generic;
using System.Text;
using WarlockGame.Core.Game.Util;

namespace WarlockGame.Core.Game.UI;

public class PrettyString {

    public List<Token> Tokens { get; }

    private static readonly Dictionary<string, string> _emptyDict = new();
    
    private PrettyString(List<Token> tokens) {
        Tokens = tokens;
    }

    public static PrettyString Parse(string input) {
        return new PrettyString(ParseTokens(input, _emptyDict));
    }
    
    public static PrettyString Parse(string input, Dictionary<string, string> textTokens) {
        return new PrettyString(ParseTokens(input, textTokens));
    }
    
    public static List<Token> ParseTokens(string input) {
        return ParseTokens(input, _emptyDict);
    }
    
    public static List<Token> ParseTokens(string input, Dictionary<string, string> textTokens) {
        var tokens = new List<Token>();
        var sb = new StringBuilder();

        var formatting = new Formatting {
            Color = Color.Black
        };

        ParseText(input, ref formatting, tokens, sb, textTokens);

        return tokens;
    }


    private static ReadOnlySpan<char> ParseText(
        ReadOnlySpan<char> input, 
        scoped ref Formatting formatting, 
        List<Token> tokens,
        StringBuilder sb,  
        Dictionary<string, string> textTokens, 
        char? endToken = null) {

        sb.Clear();
        loop: while (!input.IsEmpty) {
            var character = input[0];
            switch (character) {
                case '\\':
                    sb.Append(input[1]);
                    input = input.Slice(2);
                    break;
                case '<':
                    tokens.Add(sb.ToString());
                    sb.Clear();
                    input = ParseFormatToken(input.Slice(1), ref formatting, tokens, sb, textTokens);
                    break;
                case '$':
                    input = ParseVariableToken(input.Slice(1), sb, textTokens);
                    break;
                default:
                    if (character == endToken) {
                        input = input.Slice(1);
                        break loop;
                    }

                    sb.Append(character);
                    input = input.Slice(1);
                    break;
            }
        }

        if (endToken != null && input.IsEmpty) {
            throw new Exception("End token not found");
        }
        
        if (sb.Length != 0) {
            tokens.Add(sb.ToString());
            sb.Clear();
        } 
        
        return input;
    }

    private static ReadOnlySpan<char> ParseVariableToken(ReadOnlySpan<char> input, StringBuilder sb, Dictionary<string, string> textTokens) {
        int i;
        for (i = 0; i < input.Length; i++) {
            var character = input[i];

            if (!char.IsAsciiLetterOrDigit(character) && character != '_') {
                break;
            }
        }

        sb.Append(textTokens[input.Slice(0, i).ToString()]);
        return input.Slice(i);
    }
    
    private static ReadOnlySpan<char> ParseFormatToken(
        ReadOnlySpan<char> input, 
        scoped ref Formatting formatting, 
        List<Token> tokens,  
        StringBuilder sb, 
        Dictionary<string, string> textTokens) {

        var newFormatting = formatting;
        List<Token>? nestedTokens = null;
        while (true) {
            switch (input[0]) {
                case 'c':
                    if (input[1] != '=') {
                        throw new Exception("Invalid color token format!");
                    }
    
                    input = ParseColorToken(input.Slice(2), ref newFormatting);
                    break;
                case 't':
                    if (input[1] != '=' || input[2] != '\'') {
                        throw new Exception("Invalid text token format");
                    }

                    var innerFormatting = newFormatting;
                    nestedTokens ??= new();
                    input = ParseText(input.Slice(3), ref innerFormatting, nestedTokens, sb, textTokens, endToken: '\'');
                    break;
                case '>':
                    tokens.Add(newFormatting);
                    if (nestedTokens != null) {
                        tokens.AddRange(nestedTokens);
                        tokens.Add(formatting);
                        formatting = newFormatting;
                    }
                    
                    return input.Slice(1);
                default:
                    if (char.IsWhiteSpace(input[0])) {
                        input = input.Slice(1);
                        continue;
                    }

                    throw new Exception($"Error parsing formatting token: unexpected character {input[0]}");
            }
        }
    }

    private static ReadOnlySpan<Char> ParseColorToken(ReadOnlySpan<char> input, scoped ref Formatting formatting) {
        for (var i = 0; i < input.Length; i++) {
            var character = input[i];
            if (!char.IsAsciiLetter(character)) {
                formatting = formatting with {
                    Color = System.Drawing.Color.FromName(input.Slice(0, i).ToString())
                        .Let(c => new Color(c.R, c.G, c.B, c.A))
                };

                return input.Slice(i);
            }
        }

        throw new Exception("Formatting token not closed properly");
    }

    public union Token(string, Formatting);

    public record struct Formatting {
        public required Color Color { get; init; }
    }
}

file static class FileExtensions {
    public static ReadOnlySpan<char> SafeAdvance(this ReadOnlySpan<char> source, int n) {
        return source.Slice(Math.Min(n, source.Length));
    }
}