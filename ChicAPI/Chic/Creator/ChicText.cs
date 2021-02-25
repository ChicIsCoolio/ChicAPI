using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChicAPI.Chic.Creator
{
    public class ChicText
    {
        private static int STARTER_POSITION = 380;
        private static int BOTTOM_TEXT_SIZE = 23;
        private static int NAME_TEXT_SIZE = 47;

        public static void DrawBackground(SKCanvas c, BaseIcon icon)
        {
            using (var path = new SKPath { FillType = SKPathFillType.EvenOdd })
            {
                path.MoveTo(0, icon.Height);
                path.LineTo(0, icon.Height - 75);
                path.LineTo(icon.Width, icon.Height - 85);
                path.LineTo(icon.Width, icon.Height);
                path.Close();

                using (var shadow = SKImageFilter.CreateDropShadow(0, -3, 5, 5, SKColors.Black))
                using (var paint = new SKPaint { FilterQuality = SKFilterQuality.High, IsAntialias = true, Color = icon.RarityColors[0], ImageFilter = shadow })
                    c.DrawPath(path, paint);
            }

            using (var path = new SKPath { FillType = SKPathFillType.EvenOdd })
            {
                path.MoveTo(0, icon.Height);
                path.LineTo(0, icon.Height - 65);
                path.LineTo(icon.Width, icon.Height - 75);
                path.LineTo(icon.Width, icon.Height);
                path.Close();

                using (var paint = new SKPaint { FilterQuality = SKFilterQuality.High, IsAntialias = true, Color = new SKColor(30, 30, 30) })
                    c.DrawPath(path, paint);
            }
        }

        public static void DrawDisplayName(SKCanvas c, BaseIcon icon)
        {
            string text = icon.DisplayName;
            if (string.IsNullOrEmpty(text)) return;

            int x = 5;
            int y = STARTER_POSITION + NAME_TEXT_SIZE;

            using (var shadow = SKImageFilter.CreateDropShadow(0, 0, 5, 5, SKColors.Black))
            using (var paint = new SKPaint { FilterQuality = SKFilterQuality.High, IsAntialias = true, Typeface = ChicTypefaces.BurbankBigCondensedBold, TextSize = NAME_TEXT_SIZE, ImageFilter = shadow, Color = SKColors.White })
            {
                while (paint.TextSize > icon.Width - x * 5) paint.TextSize--;

                c.DrawText(text, x, y, paint);
            }
        }

        public static void DrawToBottom(SKCanvas c, BaseIcon icon, SKTextAlign align, string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            using (var paint = new SKPaint { FilterQuality = SKFilterQuality.High, IsAntialias = true, Typeface = ChicTypefaces.BurbankBigRegularBlack, TextSize = BOTTOM_TEXT_SIZE, TextAlign = align, Color = SKColors.White })
                if (align == SKTextAlign.Left) c.DrawText(text, 5, icon.Height - 7, paint);
                else c.DrawText(text, icon.Width - 5, icon.Height - 7, paint);
        }

        public static void DrawDescription(SKCanvas c, BaseIcon icon)
        {
            string text = icon.Description;
            int lineCount = text.Split("\n").Length;

            if (string.IsNullOrEmpty(text)) return;

            SKPaint paint = new SKPaint
            {
                IsAntialias = true,
                FilterQuality = SKFilterQuality.High,
                TextSize = 13,
                Color = SKColors.White
            };

            {
                float lineHeight = paint.TextSize * 1.2f;
                float height = lineCount * lineHeight;

                while (height > icon.Height - BOTTOM_TEXT_SIZE - 3 - STARTER_POSITION - NAME_TEXT_SIZE - 12)
                {
                    paint.TextSize--;
                    lineHeight = paint.TextSize * 1.2f;
                    height = lineCount * lineHeight;
                }
            }
            
            ChicHelper.DrawCenteredMultilineText(c, text, 4, icon, SKTextAlign.Right,
                            new SKRect(5, STARTER_POSITION + NAME_TEXT_SIZE, icon.Width - 5, icon.Height - BOTTOM_TEXT_SIZE - 3),
                            paint);
        }
    }
}
