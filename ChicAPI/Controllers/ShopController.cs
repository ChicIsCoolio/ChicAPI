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
            else 
            {
                Console.WriteLine("Something");

                catalog = Program.Epic.GetCatalog();
                Program.SaveToCache(JsonConvert.SerializeObject(catalog, Formatting.Indented), $"RawShop_{catalog.Expiration.ToString().Replace(' ', '_').Replace(":", ".").Replace("/", "-")}");
                return catalog;
            }
        }

        bool HasCatalogInCache(out Catalog catalog)
        {
            catalog = new Catalog();

            if (!Program.ListCache().Any(x => x.Contains("RawShop_"))) return false;

            var date = Program.ListCache().Where(x => x.Contains("RawShop_"))
                .Max(x => DateTime.Parse(x.Split("RawShop_")[1].Replace('_', ' ').Replace('.', ':').Replace('-', '/')));

            if (date - DateTime.UtcNow > TimeSpan.Zero)
            {
                catalog = JsonConvert.DeserializeObject<Catalog>(Program.LoadFromCache($"RawShop_{date.ToString().Replace(' ', '_').Replace(":", ".").Replace("/", "-")}"));
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
