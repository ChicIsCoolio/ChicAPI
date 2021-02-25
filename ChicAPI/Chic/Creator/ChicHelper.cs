using SkiaSharp;
using SkiaSharp.HarfBuzz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChicAPI.Chic.Creator
{
    public class ChicHelper
    {
        public class Line
        {
            public string Value;
            public float Width;
        }

        public static void DrawCenteredMultilineText(SKCanvas canvas, string text, int maxLineCount, BaseIcon icon, SKTextAlign side, SKRect area, SKPaint paint)
            => DrawCenteredMultilineText(canvas, text, maxLineCount, icon.Width, 5, side, area, paint);
        public static void DrawCenteredMultilineText(SKCanvas canvas, string text, int maxLineCount, int size, int margin, SKTextAlign align, SKRect area, SKPaint paint)
        {
            float lineHeight = paint.TextSize * 1.2f;
            List<Line> lines = SplitLines(text, paint, area.Width - margin);

            if (lines == null)
                return;
            if (lines.Count <= maxLineCount)
                maxLineCount = lines.Count;

            float height = maxLineCount * lineHeight;
            float y = area.MidY - height / 2;

            for (int i = 0; i < maxLineCount; i++)
            {
                Line line = lines[i];

                y += lineHeight;
                float x = align switch
                {
                    SKTextAlign.Center => area.MidX - line.Width / 2,
                    SKTextAlign.Right => size - margin - line.Width,
                    SKTextAlign.Left => margin,
                    _ => area.MidX - line.Width / 2
                };

                string lineText = line.Value.TrimEnd();

                canvas.DrawText(lineText, x, y, paint);
            }
        }

        public static List<Line> SplitLines(string text, SKPaint paint, float maxWidth)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            float spaceWidth = paint.MeasureText(" ");
            string[] lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            List<Line> ret = new List<Line>(lines.Length);
            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                float width = 0;
                var lineResult = new StringBuilder();
                string[] words = lines[i].Split(' ');
                foreach (var word in words)
                {
                    float wordWidth = paint.MeasureText(word);
                    float wordWithSpaceWidth = wordWidth + spaceWidth;
                    string wordWithSpace = word + " ";

                    if (width + wordWidth > maxWidth)
                    {
                        ret.Add(new Line { Value = lineResult.ToString(), Width = width });
                        lineResult = new StringBuilder(wordWithSpace);
                        width = wordWithSpaceWidth;
                    }
                    else
                    {
                        lineResult.Append(wordWithSpace);
                        width += wordWithSpaceWidth;
                    }
                }
                ret.Add(new Line { Value = lineResult.ToString(), Width = width });
            }
            return ret;
        }
    }
}
