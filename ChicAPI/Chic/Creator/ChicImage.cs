using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChicAPI.Chic.Creator
{
    public class ChicImage
    {
        public static void DrawPreviewImage(SKCanvas c, BaseIcon icon)
        {
            int x = (icon.Width - icon.Height) / 2;
            int y = 0;

            using (var shadow = SKImageFilter.CreateDropShadow(0, 0, 5, 5, SKColors.Black))
            using (var paint = new SKPaint { FilterQuality = SKFilterQuality.High, IsAntialias = true, ImageFilter = shadow })
                c.DrawBitmap(icon.IconImage, new SKRect(x, y, x + icon.Height, y + icon.Height), paint);
        }
    }
}
