using ChicAPI.Models;
using EpicGames.NET.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult<string[]> GetShopRaw()
        {
            if (!IsAuthed())
                return Unauthorized(new { Error = "Unauthorized", Message = "Try again later" });
        
            Program.SaveToCache("data", "test.txt");
            Program.SaveToCache("data2", "test2.txt");

            return Program.ListCache();

            /*if (HasCatalogInCache(out Catalog catalog)) return catalog;
            else return GetCatalog();*/
        }

        Catalog GetCatalog()
        {
            var catalog = Program.Epic.GetCatalog();



            return catalog;
        }

        bool HasCatalogInCache(out Catalog catalog)
        {
            catalog = new Catalog();

            foreach (var file in Program.ListCache().Where(x => x.Contains("RawShop_")))
            {

            }

            return false;
        }

        bool IsAuthed()
        {
            return !(Program.Epic == null || !Program.Epic.Authenticated);
        }
    }
}
