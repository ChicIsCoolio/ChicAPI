using ChicAPI.Models;
using EpicGames.NET.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System;
using Newtonsoft.Json;

namespace ChicAPI.Controllers
{
    [Route("api/v1/shop")]
    [ApiController]
    public class ShopController : Controller
    {
        [HttpGet]
        public ActionResult<ChicResponse<ChicShop>> GetShop()
        {
            if (!IsAuthed())
                return Unauthorized(new { Error = "Unauthorized", Message = "Try again later" });

            return new ChicResponse<ChicShop>(Status.NOT_READY, null);
        }

        [HttpGet("raw")]
        public ActionResult<Catalog> GetShopRaw()
        {
            if (!IsAuthed())
                return Unauthorized(new { Error = "Unauthorized", Message = "Try again later" });
        
            return GetCatalog();
        }

        Catalog GetCatalog()
        {
            if (HasCatalogInCache(out Catalog catalog)) return catalog;
            else return Program.Epic.GetCatalog();
        }

        bool HasCatalogInCache(out Catalog catalog)
        {
            catalog = new Catalog();

            var date = Program.ListCache().Where(x => x.Contains("RawShop_"))
                .Max(x => DateTime.Parse(x.Replace("RawShop_", "")));

            if (date - DateTime.UtcNow > TimeSpan.Zero)
            {
                catalog = JsonConvert.DeserializeObject<Catalog>(Program.LoadFromCache($"RawShop_{date.ToString("o")}"));
                return true;
            }
            else return false;
        }

        bool IsAuthed()
        {
            return !(Program.Epic == null || !Program.Epic.Authenticated);
        }
    }
}
