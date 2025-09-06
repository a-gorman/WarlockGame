using System;
using System.Text;

namespace WarlockGame.Core.Game.Util;

public static class TextUtil {
    public delegate Vector2 MeasureTextSize(ReadOnlySpan<Char> text);
    
    public static string WrapText(string text, MeasureTextSize measureText, float maxLineWidth, 
        float? maxHeight = null, int? maxLines = null, string truncation = "") {
        if (measureText(text).X <= maxLineWidth) {
            return text;
        }

        var spaceMeasurement = measureText(" ");
        var spaceWidth = spaceMeasurement.X;

        if (maxHeight != null) {
            maxLines = Math.Min(maxLines ?? int.MaxValue, (int)(maxHeight.Value / spaceMeasurement.Y));
        }
        if (maxLines == 0) {
            maxLines = 1;
        }

        var span = text.AsSpan();
        
        var sb = new StringBuilder();
        var lineWidth = 0f;
        var nLines = 1;

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
            
            // If the word can fit on the line, add it and continue
            if (lineWidth + size.X <= maxLineWidth) {
                AddWord(word);
                continue;
            }

            if (size.X <= maxLineWidth) {
                if(maxLines != null && nLines >= maxLines) {
                    TruncateAndAddFinalWord(word);
                    RemoveLastWhitespace();
                    return sb.ToString();
                }
                AddNewLine();
                AddWord(word);
                continue;
            }

            // Word is larger than one line
            while (!word.IsEmpty) {
                if(maxLines != null && nLines >= maxLines) {
                    TruncateAndAddFinalWord(word);
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

            void TruncateAndAddFinalWord(ReadOnlySpan<Char> newWord) {
                if(truncation.IsEmpty()) {
                    sb.Append(GetPortionOfWordThatFits(newWord, measureText, maxLineWidth - lineWidth));
                } else {
                    var truncationSize = measureText(truncation).X;
                    var truncatedWord = GetPortionOfWordThatFits(newWord, measureText, maxLineWidth - (lineWidth + truncationSize));
                    if (!truncatedWord.IsEmpty) {
                        sb.Append(truncatedWord);
                        sb.Append(truncation);
                        RemoveLastWhitespace();
                    } else {
                        RemoveLastWhitespace();
                        // This is a hack, but should be good enough most of the time, and backing up through the
                        // string builder properly is pretty difficult without reworking this state machine.
                        sb.Remove(sb.Length - truncation.Length, truncation.Length);
                        sb.Append(truncation);
                    }
                }
            }
        }
        
        RemoveLastWhitespace();
        return sb.ToString();
        
        void RemoveLastWhitespace() {
            if (sb.Length > 1 && sb[^1] == ' ' || sb[^1] == '\n') {
                sb.Remove(sb.Length - 1, 1);
                lineWidth -= spaceWidth;
            }
        }
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