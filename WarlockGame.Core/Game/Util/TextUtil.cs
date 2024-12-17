using System;
using System.Text;
using Microsoft.Xna.Framework;
using SpriteFont = Microsoft.Xna.Framework.Graphics.SpriteFont;

namespace WarlockGame.Core.Game.Util;

public static class TextUtil {
    public delegate Vector2 MeasureTextSize(ReadOnlySpan<Char> text);
    
    public static string WrapText(string text, MeasureTextSize measureText, float maxLineWidth) {
        if (measureText(text).X <= maxLineWidth) {
            return text;
        }

        var span = text.AsSpan();
        
        var sb = new StringBuilder();
        var lineWidth = 0f;
        var spaceWidth = measureText(" ").X;

        var i = 0;
        while (!span.IsEmpty) {
            
            switch (span[0]) {
                case '\r' or '\n':
                    AddNewLine();
                    span = span.Slice(1);
                    continue;
                case ' ':
                    AddSpace();
                    span = span.Slice(1);
                    continue;
            }
            
            var word = GetWord(span);
            span = span.Slice(word.Length);
            Vector2 size = measureText(word);
            
            if (lineWidth + size.X <= maxLineWidth) {
                AddWord(word);
                continue;
            }

            if (size.X <= maxLineWidth) {
                AddNewLine();
                AddWord(word);
                continue;
            }
            
            // Word is larger than one line
            while (!word.IsEmpty) {
                var wordPortion = GetPortionOfWordThatFits(word, measureText, maxLineWidth);
                if (lineWidth != 0) {
                    AddNewLine();
                }
                AddWord(wordPortion);
                word = word.Slice(wordPortion.Length);
            }
            continue;

            void AddSpace() {
                sb.Append(' ');
                lineWidth += spaceWidth;
            }
        
            void AddNewLine() {
                if (sb.Length > 1 && sb[^1] == ' ') {
                    sb[^1] = '\n';
                }
                else {
                    sb.Append('\n');
                }
                lineWidth = 0;
            }

            void AddWord(ReadOnlySpan<Char> newWord) {
                sb.Append(newWord);
                lineWidth += size.X;
            }
        }

        return sb.ToString();
    }
    
    private static ReadOnlySpan<Char> GetWord(ReadOnlySpan<Char> text) {
        for (var index = 0; index < text.Length; index++) {
            var character = text[index];
            if (character is ' ' or '\n' or '\t') {
                if (index == 0) {
                    return Span<char>.Empty;
                }
                return text.Slice(0, index);
            }
        }

        // If we don't hit a whitespace character, then the whole text is the word.
        return text;
    }

    // This is pretty expensive. Something like a binary search would be much faster
    private static ReadOnlySpan<Char> GetPortionOfWordThatFits(
        ReadOnlySpan<Char> word, MeasureTextSize measureText, float remainingLineWidth) {
        for (var i = 0; i < word.Length; i++) {
            var length = measureText(word.Slice(0, i + 1));
            if (length.X > remainingLineWidth) {
                if (i == 0) {
                    return ReadOnlySpan<char>.Empty;
                }
                return word.Slice(0, i);
            }
        }
        
        return word;
    }
}