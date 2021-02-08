using ChicAPI.Models;
using EpicGames.NET.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using EntryItem = ChicAPI.Models.EntryItem;
using Microsoft.AspNetCore.Http.Features;

namespace ChicAPI.Controllers
{
    [Route("api/v1/shop")]
    [ApiController]
    public class ShopController : Controller
    {
        [HttpGet("br")]
        public ActionResult<ChicResponse<ChicShop>> GetShop()
        {
            if (!IsAuthed())
                return Unauthorized(new { Error = "Unauthorized", Message = "Try again later" });

            var catalog = GetCatalog();
            var shop = new ChicShop();

            shop.Expiration = catalog.Expiration;
            shop.Sections = new Dictionary<string, ShopEntry[]>();

            foreach (var storefront in catalog.Storefronts.Where(x => x.Name == "BRWeeklyStorefront"
                || x.Name == "BRDailyStorefront" || x.Name == "BRSpecialFeatured"
                || x.Name == "BRSpecialDaily"))
            {
                foreach (var entry in storefront.Entries)
                {
                    ShopEntry e = new ShopEntry
                    {
                        Items = new EntryItem[0]
                    };

                    foreach (var grant in entry.ItemGrants)
                    {
                        e.Items = e.Items.Append(new EntryItem
                        {
                            Id = grant.TemplateId.Split(':')[1],
                            Quantity = grant.Quantity
                        }).ToArray();
                    }

                    if (!entry.Meta.TryGetValue("SectionId", out string sectionId))
                        sectionId = entry.MetaInfo.First(x => x.Key == "SectionId").Value;

                    if (!shop.Sections.ContainsKey(sectionId))
                    {
                        shop.Sections.Add(sectionId, new ShopEntry[] { e });
                    }
                    else
                    {
                        shop.Sections[sectionId] = shop.Sections[sectionId].Append(e).ToArray();
                    }
                }
            }

            return new ChicResponse<ChicShop>(Status.OK, shop);
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
