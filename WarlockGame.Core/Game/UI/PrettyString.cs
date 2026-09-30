using System;
using System.Collections.Generic;
using System.Linq;
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

        var remaining = ParseText(input, ref formatting, tokens, sb, textTokens);

        if (remaining.Length != 0) {
            throw new Exception("Unmatched '>' detected");
        }
        
        return tokens;
    }


    private static ReadOnlySpan<char> ParseText(
        ReadOnlySpan<char> input, 
        scoped ref Formatting formatting, 
        List<Token> tokens,
        StringBuilder sb,  
        Dictionary<string, string> textTokens) {

        sb.Clear();
        loop: while (!input.IsEmpty) {
            var character = input[0];
            switch (character) {
                case '#':
                    input = ParseEscapeToken(input.Slice(1), ref formatting, tokens, sb, textTokens);
                    break;
                case '<':
                    tokens.Add(sb.ToString());
                    sb.Clear();
                    var innerFormatting = formatting;
                    input = ParseText(input.Slice(1), ref innerFormatting, tokens, sb, textTokens);
                    if (innerFormatting != formatting) {
                        if (tokens.Count > 1 && tokens[^1] is Formatting lastToken && lastToken == formatting) {
                            tokens.RemoveAt(tokens.Count - 1);
                        } else {
                            AddFormattingToken(formatting, tokens, sb);
                        }
                    }
                    break;
                case '>':
                    input = input.Slice(1);
                    break loop;
                case '$':
                    input = ParseVariableToken(input.Slice(1), textTokens, out var variable);
                    sb.Append(variable);
                    break;
                default:
                    sb.Append(character);
                    input = input.Slice(1);
                    break;
            }
        }
        
        if (sb.Length != 0) {
            tokens.Add(sb.ToString());
            sb.Clear();
        } 
        
        return input;
    }

    private static ReadOnlySpan<char> ParseVariableToken(ReadOnlySpan<char> input, Dictionary<string, string> textTokens, out string output) {
        int i;
        bool consumeExtraChar = false;
        for (i = 0; i < input.Length; i++) {
            var character = input[i];

            if (!char.IsAsciiLetterOrDigit(character) && character != '_') {
                consumeExtraChar = character == '$';
                break;
            }
        }

        output = textTokens[input.Slice(0, i).ToString()];
        return input.Slice(consumeExtraChar ? i + 1 : i);
    }
    
    private static ReadOnlySpan<char> ParseEscapeToken(
        ReadOnlySpan<char> input,
        scoped ref Formatting formatting,
        List<Token> tokens,
        StringBuilder sb,
        Dictionary<string, string> textTokens) {
        switch (input[0]) {
            case 'c':
                if (input[1] != '=') {
                    throw new Exception("Invalid color token format!");
                }
                
                return ParseColorToken(input.Slice(2), ref formatting, tokens, textTokens, sb);
            default:
                sb.Append(input[0]);
                return input.Slice(1);
        }
    }

    private static ReadOnlySpan<char> ParseColorToken(ReadOnlySpan<char> input, 
        scoped ref Formatting formatting, 
        List<Token> tokens,
        Dictionary<string, string> textTokens,
        StringBuilder sb) {
        if (input[0] == '$') {
            var newSlice = ParseVariableToken(input.Slice(1), textTokens, out var variableToken);
            formatting = GetFormatting(variableToken, formatting);
            AddFormattingToken(formatting, tokens, sb);
            return newSlice;
        }

        for (var i = 0; i < input.Length; i++) {
            var character = input[i];
            if (!char.IsAsciiLetter(character)) {
                formatting = GetFormatting(input.Slice(0, i), formatting);
                AddFormattingToken(formatting, tokens, sb);

                return input.Slice(character == ' ' ? i + 1 : i);
            }
        }

        throw new Exception("Formatting token not closed properly");

        Formatting GetFormatting(ReadOnlySpan<char> input, scoped in Formatting formatting) {
            return formatting with {
                Color = System.Drawing.Color.FromName(input.ToString())
                    .Let(c => new Color(c.R, c.G, c.B, c.A))
            };
        }
    }
    
    private static void AddFormattingToken(in Formatting formatting, List<Token> tokens, StringBuilder sb) {
        AddTextToken(sb, tokens);
        if (tokens.Count != 0 && tokens[^1] is Formatting formatToken && formatting != formatToken) {
            tokens[^1] = formatting;
        } else {
            tokens.Add(formatting);
        }
    }
    
    private static void AddTextToken(StringBuilder sb, List<Token> tokens) {
        if (sb.Length == 0) {
            return;
        }
        
        if (tokens.Count != 0 && tokens[^1] is string stringToken) {
            tokens[^1] = string.Concat(stringToken, sb);
        } else {
            tokens.Add(sb.ToString());
        }

        sb.Clear();
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