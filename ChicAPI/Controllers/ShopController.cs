using ChicAPI.Models;
using EpicGames.NET;
using EpicGames.NET.Structs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            return Program.Epic.GetCatalog();
        }

        bool IsAuthed()
        {
            return !(Program.Epic == null || !Program.Epic.Authenticated);
        }
    }
}
