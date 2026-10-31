using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

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
    
    public static List<Line> WrapText(ReadOnlySpan<char> text, 
        MeasureTextSize measureText, 
        float maxLineWidth, 
        float startingIndent = 0,
        float? maxHeight = null, 
        int? maxLines = null, 
        string truncator = "") {
        
        var spaceMeasurement = measureText(" ");
        var spaceWidth = spaceMeasurement.X;

        if (maxHeight != null) {
            maxLines = Math.Min(maxLines ?? int.MaxValue, (int)(maxHeight.Value / spaceMeasurement.Y));
        }
        if (maxLines == 0) {
            maxLines = 1;
        }

        var span = text;

        var lines = new List<Line>();
        var sb = new StringBuilder();
        var lineWidth = startingIndent;
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


    #region TextMeasurement

    private static readonly Dictionary<SpriteFont, CharacterRegion[]> _regions = new();
    private static readonly Dictionary<SpriteFont, int> _defaultGlyphIdices = new();

    public static Vector2 MeasureString(this SpriteFont font, ReadOnlySpan<char> text) {
        var source = new CharacterSource(text);
        Vector2 size;
        MeasureString(font, ref source, out size);
        return size;
    }


    private static unsafe void MeasureString(SpriteFont font, ref CharacterSource text, out Vector2 size) {
        if (text.Length == 0) {
            size = Vector2.Zero;
            return;
        }

        var regions = _regions.GetOrAdd(font, CreateCharacterRegions);
        var defaultGlyphIndex = _defaultGlyphIdices.GetOrAdd(font, f => {
            if (f.DefaultCharacter == null) return -1;
            
            // No need to check for existence because it's validated by the SpriteFont at construction time.
            TryGetGlyphIndex(regions, f.DefaultCharacter.Value, out int result);
            return result;
        });


        var width = 0.0f;
        var finalLineHeight = (float)font.LineSpacing;

        var offset = Vector2.Zero;
        var firstGlyphOfLine = true;

        fixed (SpriteFont.Glyph* pGlyphs = font.Glyphs)
            for (var i = 0; i < text.Length; ++i) {
                var c = text[i];

                if (c == '\r')
                    continue;

                if (c == '\n') {
                    finalLineHeight = font.LineSpacing;

                    offset.X = 0;
                    offset.Y += font.LineSpacing;
                    firstGlyphOfLine = true;
                    continue;
                }

                var currentGlyphIndex = GetGlyphIndexOrDefault(c, regions, defaultGlyphIndex);
                Debug.Assert(currentGlyphIndex >= 0 && currentGlyphIndex < font.Glyphs.Length,
                    "currentGlyphIndex was outside the bounds of the array.");
                var pCurrentGlyph = pGlyphs + currentGlyphIndex;

                // The first character on a line might have a negative left side bearing.
                // In this scenario, SpriteBatch/SpriteFont normally offset the text to the right,
                //  so that text does not hang off the left side of its rectangle.
                if (firstGlyphOfLine) {
                    offset.X = Math.Max(pCurrentGlyph->LeftSideBearing, 0);
                    firstGlyphOfLine = false;
                } else {
                    offset.X += font.Spacing + pCurrentGlyph->LeftSideBearing;
                }

                offset.X += pCurrentGlyph->Width;

                var proposedWidth = offset.X + Math.Max(pCurrentGlyph->RightSideBearing, 0);
                if (proposedWidth > width)
                    width = proposedWidth;

                offset.X += pCurrentGlyph->RightSideBearing;

                if (pCurrentGlyph->Cropping.Height > finalLineHeight)
                    finalLineHeight = pCurrentGlyph->Cropping.Height;
            }

        size.X = width;
        size.Y = offset.Y + finalLineHeight;
    }

    private static unsafe bool TryGetGlyphIndex(CharacterRegion[] regions, char c, out int index) {
        fixed (CharacterRegion* pRegions = regions) {
            if (!TryGetRegionIdx(c, regions.Length, pRegions, out int regionIdx)) {
                c = char.IsUpper(c) ? char.ToLower(c) : char.ToUpper(c);
                TryGetRegionIdx(c, regions.Length, pRegions, out regionIdx);
            }

            if (regionIdx == -1) {
                index = -1;
                return false;
            }

            index = pRegions[regionIdx].StartIndex + (c - pRegions[regionIdx].Start);
        }

        return true;
    }

    private static unsafe bool TryGetRegionIdx(char c, int regionLen, CharacterRegion* pRegions, out int regionIdx) {
        // Get region Index 
        regionIdx = -1;
        var l = 0;
        var r = regionLen - 1;
        while (l <= r) {
            var m = (l + r) >> 1;
            Debug.Assert(m >= 0 && m < regionLen, "Index was outside the bounds of the array.");
            if (pRegions[m].End < c) {
                l = m + 1;
            } else if (pRegions[m].Start > c) {
                r = m - 1;
            } else {
                regionIdx = m;
                break;
            }
        }

        if (regionIdx == -1) {
            return false;
        }

        return true;
    }


    private static int GetGlyphIndexOrDefault(char c, CharacterRegion[] regions, int defaultGlyphIdx) {
        if (!TryGetGlyphIndex(regions, c, out var glyphIdx)) {
            if (defaultGlyphIdx == -1)
                throw new ArgumentException("Text Contains Unresolvable Characters");

            return defaultGlyphIdx;
        } else
            return glyphIdx;
    }


    private static CharacterRegion[] CreateCharacterRegions(SpriteFont font) {
        var characters = font.Characters;

        var regions = new Stack<CharacterRegion>();

        for (var i = 0; i < characters.Count; i++) {
            if (regions.Count == 0 || characters[i] > (regions.Peek().End + 1)) {
                // Start a new region
                regions.Push(new CharacterRegion(characters[i], i));
            } else if (characters[i] == (regions.Peek().End + 1)) {
                var currentRegion = regions.Pop();
                // include character in currentRegion
                currentRegion.End++;
                regions.Push(currentRegion);
            } else // characters[i] < (regions.Peek().End+1)
            {
                throw new InvalidOperationException("Invalid SpriteFont. Character map must be in ascending order.");
            }
        }

        var tmp = regions.ToArray();
        Array.Reverse(tmp);
        return tmp;
    }

    private readonly ref struct CharacterSource {
        private readonly ReadOnlySpan<char> _value;

        public CharacterSource(ReadOnlySpan<char> builder) {
            _value = builder;
            Length = builder.Length;
        }

        public readonly int Length;
        public char this[int index] => _value[index];
    }

    private struct CharacterRegion {
        public char Start;
        public char End;
        public int StartIndex;

        public CharacterRegion(char start, int startIndex) {
            Start = start;
            End = start;
            StartIndex = startIndex;
        }
    }

    #endregion
}
