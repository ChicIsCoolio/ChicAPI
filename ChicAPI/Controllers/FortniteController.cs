using ChicAPI.Models;
using EpicGames.NET.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Collections.Generic;
using Fortnite_API.Objects.V2;
using EntryItem = ChicAPI.Models.EntryItem;
using ShopSection = ChicAPI.Models.ShopSection;
using ChicAPI.Chic;

namespace ChicAPI.Controllers
{
    [Route("api/v1/fortnite")]
    [ApiController]
    public class FortniteController : Controller
    {
        [HttpGet("content")]
        public ActionResult<Content> GetContent()
        {
            return Program.Epic.FortniteService.GetContent();
        }

        [HttpGet("keychain")]
        public ActionResult<string[]> GetKeychain()
        {
            if (!Program.IsAuthed())
                return Unauthorized(new { Error = "Unauthorized", Message = "Try again later" });

            return Program.Epic.FortniteService.GetKeychain();
        }
    }
}