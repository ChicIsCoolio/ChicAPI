using FModel.Chic;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChicAPI.Chic.Creator
{
    public class ChicWatermark
    {
        public static void DrawChicFace(SKCanvas c, SKColor color, int x, int size = 128)
        {
            using (var path = SKPath.ParseSvgPathData(ChicData.ChicHeadPath))
            using (var b = new SKBitmap(320, 320))
            {
                using (var ca = new SKCanvas(b))
                using (var paint = new SKPaint { FilterQuality = SKFilterQuality.High, IsAntialias = true, Color = color })
                    ca.DrawPath(path, paint);

                using (var shadow = SKImageFilter.CreateDropShadow(0, 0, 5, 5, SKColors.Black))
                using (var paint = new SKPaint { FilterQuality = SKFilterQuality.High, IsAntialias = true, ImageFilter = shadow })
                    c.DrawBitmap(b, new SKRect(x, 0, size, size));
            }
        }
    }
}
