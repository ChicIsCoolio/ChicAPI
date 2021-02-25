using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChicAPI.Chic.Creator
{
    public class BaseIcon : IDisposable
    {
        public SKBitmap IconImage;
        public SKBitmap RarityBackgroundImage;
        public SKColor[] RarityColors;
        public string DisplayName;
        public string Description;
        public string ShortDescription;
        public string CosmeticSource;
        public int Height = 512;
        public int Width = 512;

        public void Dispose()
        {
            if (IconImage != null) IconImage.Dispose();
            if (RarityBackgroundImage != null) RarityBackgroundImage.Dispose();

            RarityColors = null;
            DisplayName = "";
            Description = "";
            ShortDescription = "";
            Height = 0;
            Width = 0;
        }
    }
}
