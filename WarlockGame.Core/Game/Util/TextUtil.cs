using System;
using System.Collections.Generic;
using System.Text;

namespace WarlockGame.Core.Game.Util;

public static class TextUtil {
    public delegate Vector2 MeasureTextSize(ReadOnlySpan<char> text);

    public struct Line {
        public string Text { get; }
        public float Width { get; }
        
        public Line(string text, float width) {
            Text = text;
            Width = width;
        }
    }
    
    public static List<Line> WrapText(string text, MeasureTextSize measureText, float maxLineWidth, 
        float? maxHeight = null, int? maxLines = null, string truncator = "") {
        
        var spaceMeasurement = measureText(" ");
        var spaceWidth = spaceMeasurement.X;

        if (maxHeight != null) {
            maxLines = Math.Min(maxLines ?? int.MaxValue, (int)(maxHeight.Value / spaceMeasurement.Y));
        }
        if (maxLines == 0) {
            maxLines = 1;
        }

        var span = text.AsSpan();

        var lines = new List<Line>();
        var sb = new StringBuilder();
        var lineWidth = 0f;
        var nLines = 1;

        Vector2 wordSize;
        
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
            wordSize = measureText(word);
            
            // If the word can fit on the line, add it and continue
            if (lineWidth + wordSize.X <= maxLineWidth) {
                AddCurrentWord(word);
                continue;
            }

            // If the word does not fit on the line, but it is smaller than a line, add a new line unless we
            // are out of vertical space.
            if (wordSize.X <= maxLineWidth) {
                if(maxLines != null && nLines >= maxLines) {
                    TruncateAndAddFinalWord(word);
                    RemoveLastWhitespace();
                    return GetFinalReturnValue();
                }
                AddNewLine();
                AddCurrentWord(word);
                continue;
            }

            // Edge case: word is larger than one line
            while (!word.IsEmpty) {
                if(maxLines != null && nLines >= maxLines) {
                    TruncateAndAddFinalWord(word);
                    return GetFinalReturnValue();
                }
                var wordPortion = GetPortionOfWordThatFits(word, measureText, maxLineWidth, out float sliceWidth);
                if (lineWidth != 0) {
                    AddNewLine();
                }
                AddSlice(wordPortion, sliceWidth);
                word = word.Slice(wordPortion.Length);
            }
        }
        
        RemoveLastWhitespace();
        return GetFinalReturnValue();
        
        void AddSpace() {
            sb.Append(' ');
            lineWidth += spaceWidth;
        }
        
        void AddNewLine() {
            RemoveLastWhitespace();

            lines.Add(new Line(sb.ToString(), lineWidth));
            sb.Clear();
            lineWidth = 0;
            nLines++;
        }

        void AddCurrentWord(ReadOnlySpan<char> newWord) {
            AddSlice(newWord, wordSize.X);
        }
        
        void AddSlice(ReadOnlySpan<char> slice, float width) {
            sb.Append(slice);
            lineWidth += width;
        }
        
        void TruncateAndAddFinalWord(ReadOnlySpan<char> newWord) {
            if(truncator.IsEmpty()) {
                var slice = GetPortionOfWordThatFits(newWord, measureText, maxLineWidth - lineWidth, out float sliceWidth);
                AddSlice(slice, sliceWidth);
            } else {
                var truncatorWidth = measureText(truncator).X;
                var remainingWidth = maxLineWidth - (lineWidth + truncatorWidth);
                var truncatedWord = GetPortionOfWordThatFits(newWord, measureText, remainingWidth, out float truncatedWidth);
                if (!truncatedWord.IsEmpty) {
                    AddSlice(truncatedWord, truncatedWidth);
                    AddSlice(truncator, truncatorWidth);
                    RemoveLastWhitespace();
                } else {
                    RemoveLastWhitespace();
                    // This is a hack, but should be good enough most of the time, and backing up through the
                    // string builder properly is pretty difficult without reworking this state machine.
                    sb.Remove(sb.Length - truncator.Length, truncator.Length);
                    sb.Append(truncator);
                }
            }
        }
        
        List<Line> GetFinalReturnValue() {
            while (RemoveLastWhitespace()) { }

            lines.Add(new Line(sb.ToString(), lineWidth));
            
            return lines;
        }
        
        bool RemoveLastWhitespace() {
            if (sb.Length > 1 && sb[^1] == ' ') {
                sb.Remove(sb.Length - 1, 1);
                lineWidth -= spaceWidth;
                return true;
            }

            return false;
        }
    }
    
    private static ReadOnlySpan<char> GetWord(ReadOnlySpan<char> text) {
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

    // This is pretty expensive. Something like an exponential search could be much faster
    private static ReadOnlySpan<char> GetPortionOfWordThatFits(
        ReadOnlySpan<char> word, MeasureTextSize measureText, float remainingLineWidth, out float width) {
        float previousSliceWidth = 0f;
        for (var i = 0; i < word.Length; i++) {
            // n^2
            var sliceWidth = measureText(word.Slice(0, i + 1)).X;
            if (sliceWidth > remainingLineWidth) {
                if (i == 0) {
                    width = 0;
                    return ReadOnlySpan<char>.Empty;
                }

                width = previousSliceWidth;
                return word.Slice(0, i);
            }

            previousSliceWidth = sliceWidth;
        }

        width = previousSliceWidth;
        return word;
    }
}