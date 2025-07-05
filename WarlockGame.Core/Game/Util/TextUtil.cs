using System;
using System.Text;
using Microsoft.Xna.Framework;

namespace WarlockGame.Core.Game.Util;

public static class TextUtil {
    public delegate Vector2 MeasureTextSize(ReadOnlySpan<Char> text);
    
    public static string WrapText(string text, MeasureTextSize measureText, float maxLineWidth, 
        float? maxHeight = null, int? maxLines = null, string truncation = "") {
        if (measureText(text).X <= maxLineWidth) {
            return text;
        }

        var spaceMeasurement = measureText(" ");

        if (maxHeight != null) {
            maxLines = Math.Min(maxLines ?? 0, maxHeight / spaceMeasurement.Y)
        }
        if (maxLines == 0) {
            maxLines = 1;
        }

        var span = text.AsSpan();
        
        var sb = new StringBuilder();
        var lineWidth = 0f;
        var nLines = 0;
        var spaceWidth = spaceMeasurement.X;

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
                if(maxLines != null && maxLines >= nLines) {
                    TruncateAndAddWord(word);
                    return sb.ToString();
                }
                AddNewLine();
                AddWord(word);
                continue;
            }
            
            // Word is larger than one line
            while (!word.IsEmpty) {
                if(maxLines != null && maxLines >= nLines) {
                    TruncateAndAddWord(word);
                    return sb.ToString();
                }
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
                nLines++;
            }

            void AddWord(ReadOnlySpan<Char> newWord) {
                sb.Append(newWord);
                lineWidth += size.X;
            }

            void TruncateAndAddWord(ReadOnlySpan<Char> newWord) {
                if(truncation.IsEmpty) {
                    sb.Append(GetPortionOfWordThatFits(newWord, measureText, maxLineWidth - lineWidth));
                }
                var truncationSize = measureText(truncation);
                var truncatedWord = GetPortionOfWordThatFits(newWord, measureText, maxLineWidth - (lineWidth + truncationSize));
                sb.Append(truncatedWord);
                sb.Append(truncation);
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

    // This is pretty expensive. Something like a binary search could be much faster
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