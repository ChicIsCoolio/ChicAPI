using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChicAPI.Chic.Creator
{
    public class ChicTypefaces
    {
        public static SKTypeface BurbankBigCondensedBold { get; } = SKTypeface.FromFile(Program.Root + "Resources/BurbankBigCondensed-Bold.ttf");
        public static SKTypeface BurbankBigRegularBlack { get; } = SKTypeface.FromFile(Program.Root + "Resources/BurbankBigRegular-Black.otf");
    }
}
