using ChicAPI.Models;
using EpicGames.NET.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using EntryItem = ChicAPI.Models.EntryItem;
using ShopSection = ChicAPI.Models.ShopSection;

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
            var content = Program.Epic.FortniteService.GetContent();
            var shop = new ChicShop();

            shop.Expiration = catalog.Expiration;
            shop.Sections = new Dictionary<string, ShopEntry[]>();
            shop.SectionInfos = new ShopSection[0];

            foreach (var storefront in catalog.Storefronts.Where(x => x.Name == "BRWeeklyStorefront"
                || x.Name == "BRDailyStorefront" || x.Name == "BRSpecialFeatured"
                || x.Name == "BRSpecialDaily"))
            {
                foreach (var entry in storefront.Entries)
                {
                    ShopEntry e = new ShopEntry
                    {
                        OfferId = entry.OfferId,
                        OfferType = entry.OfferType,
                        CurrencyType = entry.Prices.Length > 0 ? entry.Prices[0].CurrencyType : "",
                        RegularPrice = entry.Prices.Length > 0 ? entry.Prices[0].RegularPrice : 0,
                        BasePrice = entry.Prices.Length > 0 ? entry.Prices[0].BasePrice : 0,
                        FinalPrice = entry.Prices.Length > 0 ? entry.Prices[0].FinalPrice : 0,
                        Categories = entry.Categories,
                        Items = new EntryItem[0],
                        SortPriority = entry.SortPriority,
                        MetaInfo = entry.MetaInfo,
                        Meta = entry.Meta
                    };

                    foreach (var grant in entry.ItemGrants)
                    {
                        var id = grant.TemplateId.Split(':')[1];
                        var info = Program.FortniteApi.V2.Cosmetics.GetBr(id).Data;

                        e.Items = e.Items.Append(new EntryItem
                        {
                            Id = id,
                            Quantity = grant.Quantity,
                            Image = info.Images.HasFeatured ? info.Images.Featured : info.Images.Icon,
                            Name = info.Name,
                            Rarity = info.Rarity,
                            Series = info.Series,
                            Type = info.Type,
                            ShopHistory = info.ShopHistory.ToArray()
                        }).ToArray();
                    }

                    if (!entry.Meta.TryGetValue("SectionId", out string sectionId))
                        sectionId = entry.MetaInfo.First(x => x.Key == "SectionId").Value;

                    if (!shop.Sections.ContainsKey(sectionId))
                    {
                        var section = content.ShopSections.GetSectionById(sectionId);

                        shop.SectionInfos = shop.SectionInfos.Append(new ShopSection
                        {
                            DisplayName = section.DisplayName,
                            SectionId = sectionId,
                            LandingPriority = section.LandingPriority
                        }).ToArray();

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
