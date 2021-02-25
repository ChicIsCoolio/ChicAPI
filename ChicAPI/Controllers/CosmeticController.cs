using ChicAPI.Chic.Creator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IOFile = System.IO.File;

namespace ChicAPI.Controllers
{
    [Route("api/v1/cosmetics")]
    [ApiController]
    public class CosmeticController : ControllerBase
    {
        [HttpGet("{cosmeticId}")]
        public IActionResult DrawCosmetic(string cosmeticId, [FromQuery] bool cache = true)
        {
            try
            {
                if (string.IsNullOrEmpty(cosmeticId)) return BadRequest("{\"error\": \"emptyId\", \"message\": \"The id is empty dummy\"}");
                if (cache && Program.IsInCache(cosmeticId + ".png"))
                    return File(IOFile.ReadAllBytes($"{Program.Root}Cache/{cosmeticId}.png"), "image/png");

                var cosmetic = Program.FortniteApi.V2.Cosmetics.GetBr(cosmeticId).Data;
                if (cosmetic == null) throw new Exception($"cosmetic ({cosmeticId}) is null sadge");

                using (var icon = new BaseIcon
                {
                    DisplayName = cosmetic.Name,
                    Description = cosmetic.Description,
                    ShortDescription = cosmetic.Type.DisplayValue,
                    //CosmeticSource = cosmetic.HasGameplayTags ? cosmetic.GameplayTags.Any(x => x.StartsWith("Cosmetics.Source.")) ? cosmetic.GameplayTags.First(x => x.StartsWith("Cosmetics.Source.")).Replace("Cosmetics.Source.", "") : "" : "",
                })
                {
                    icon.IconImage = Program.BitmapFromUrl(cosmetic.Images.HasFeatured ? cosmetic.Images.Featured : cosmetic.Images.HasIcon ? cosmetic.Images.Icon : cosmetic.Images.HasSmallIcon ? cosmetic.Images.SmallIcon : null, "icon_" + cosmeticId);
                    icon.RarityBackgroundImage = cosmetic.HasSeries ? Program.BitmapFromUrl(cosmetic.Series.Image, cosmetic.Series.Value) : null;
                    ChicRarity.GetRarityColors(icon, cosmetic.Rarity.BackendValue);
                    Program.SaveToCache(ChicIcon.DrawIcon(icon), cosmeticId);
                }

                return File(IOFile.ReadAllBytes($"{Program.Root}Cache/{cosmeticId}.png"), "image/png");
            } 
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
