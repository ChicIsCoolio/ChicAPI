using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChicAPI.Chic.Creator
{
    public class ChicIcon
    {
        public static SKBitmap DrawIcon(BaseIcon icon)
        {
            var ret = new SKBitmap(icon.Width, icon.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
            using (var c  = new SKCanvas(ret))
            {
                ChicRarity.DrawRarity(c, icon);
                ChicWatermark.DrawChicFace(c, SKColors.White, icon.Width - 116);
                ChicImage.DrawPreviewImage(c, icon);
                ChicText.DrawBackground(c, icon);
                ChicText.DrawDisplayName(c, icon);
                ChicText.DrawDescription(c, icon);

                if (!(icon.ShortDescription == icon.DisplayName) && !(icon.ShortDescription == icon.Description))
                    ChicText.DrawToBottom(c, icon, SKTextAlign.Left, icon.ShortDescription);

                /*string sourceText = icon.CosmeticSource switch
                {
                    "ItemShop" => "Item Shop",
                    "Granted.Founders" => "Founder's Pack",
                    _ => icon.CosmeticSource
                };

                if (sourceText.Contains("BattlePass.Paid"))
                {
                    string season = sourceText.Replace("Season", "").Replace(".BattlePass.Paid", "");
                    season = season == "10" ? "X" : season;

                    sourceText = $"Season {season} Battle Pass";
                }

                ChicText.DrawToBottom(c, icon, SKTextAlign.Right, sourceText);*/
            }

            return ret;
        }
    }
}
