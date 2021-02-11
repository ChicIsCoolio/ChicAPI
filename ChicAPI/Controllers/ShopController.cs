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
    [Route("api/v1/shop")]
    [ApiController]
    public class ShopController : Controller
    {
        [HttpGet("br")]
        public ActionResult<ChicResponse<ChicShop>> GetShop()
        {
            if (!IsAuthed())
                return Unauthorized(new { Error = "Unauthorized", Message = "Try again later" });

            return GetChicShop();
        }

        [HttpGet("raw")]
        public ActionResult<Catalog> GetShopRaw()
        {
            if (!IsAuthed())
                return Unauthorized(new { Error = "Unauthorized", Message = "Try again later" });
        
            return GetCatalog();
        }

        public static ChicResponse<ChicShop> GetChicShop()
        {
            if (HasChicShopInCache(out ChicShop shop)) return new ChicResponse<ChicShop>(Status.OK, shop);
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                var catalog = GetCatalog();
                var content = Program.Epic.FortniteService.GetContent();
                var fnApiShop = Program.FortniteApi.V2.Shop.GetBr().Data;
                
                var empty = new List<BrShopV2StoreFrontEntry>();

                var fnApiEntries = (fnApiShop.HasFeatured ? fnApiShop.Featured : fnApiShop.HasDaily ?
                    fnApiShop.Daily : fnApiShop.HasSpecialFeatured ? fnApiShop.SpecialFeatured : fnApiShop.SpecialDaily).Entries.Concat(fnApiShop.HasDaily ? fnApiShop.Daily.Entries : empty)
                        .Concat(fnApiShop.HasSpecialFeatured ? fnApiShop.SpecialFeatured.Entries : empty)
                        .Concat(fnApiShop.HasSpecialDaily ? fnApiShop.SpecialDaily.Entries : empty).ToList();

                shop.ShopDate = fnApiShop.Date;
                shop.Expiration = catalog.Expiration;
                shop.Sections = new Dictionary<string, List<ShopEntry>>();
                shop.SectionInfos = new List<ShopSection>();

                foreach (var storefront in catalog.Storefronts.Where(x => x.Name == "BRWeeklyStorefront"
                    || x.Name == "BRDailyStorefront" || x.Name == "BRSpecialFeatured"
                    || x.Name == "BRSpecialDaily"))
                {
                    foreach (var entry in storefront.Entries)
                    {
                        var apiEntry = fnApiEntries.First(predicate: x => x.OfferId == entry.OfferId);

                        ShopEntry e = new ShopEntry
                        {
                            OfferId = entry.OfferId,
                            OfferType = entry.OfferType,
                            CurrencyType = entry.Prices.Length > 0 ? entry.Prices[0].CurrencyType : "",
                            RegularPrice = apiEntry.RegularPrice,
                            BasePrice = entry.Prices.Length > 0 ? entry.Prices[0].BasePrice : 0,
                            FinalPrice = apiEntry.FinalPrice,
                            Categories = entry.Categories,
                            Items = new List<EntryItem>(),
                            Bundle = apiEntry.Bundle,
                            Banner = apiEntry.Banner,
                            SortPriority = entry.SortPriority,
                            MetaInfo = entry.MetaInfo,
                            Meta = entry.Meta
                        };

                        foreach (var grant in entry.ItemGrants)
                        {
                            var id = grant.TemplateId.Split(':')[1];
                            var type = grant.TemplateId.Split(':')[0];
                            if (type.Contains("Challenge"))
                            {
                                e.Items.Add(new EntryItem
                                {
                                    Id = id,
                                    Quantity = grant.Quantity,
                                    Image = new Uri("https://ChicAPI.ChicIsCoolio.repl.co/images/Icon_ChallengeBundle.png"),
                                    Name = "Challenge Bundle",
                                    Rarity = new BrCosmeticV2Rarity().Set("legendary", "Legendary", "EFortRarity::Legendary"),
                                    Series = null,
                                    Type = new BrCosmeticV2Type().Set(type.ToLower(), "Challenge Bundle", type),
                                    ShopHistory = null
                                });
                            }
                            else
                            {
                                var info = apiEntry.Items.First(predicate: x => x.Id.ToLower() == id.ToLower());

                                e.Items.Add(new EntryItem
                                {
                                    Id = id,
                                    Quantity = grant.Quantity,
                                    Image = info.Images.HasFeatured ? info.Images.Featured : info.Images.Icon,
                                    Name = info.Name,
                                    Rarity = info.Rarity,
                                    Series = info.Series,
                                    Type = info.Type,
                                    ShopHistory = info.ShopHistory.ToArray()
                                });
                            }
                        }

                        if (!entry.Meta.TryGetValue("SectionId", out string sectionId))
                            sectionId = entry.MetaInfo.First(x => x.Key == "SectionId").Value;

                        if (!shop.Sections.ContainsKey(sectionId))
                        {
                            var section = content.ShopSections.GetSectionById(sectionId);

                            shop.SectionInfos.Add(new ShopSection
                            {
                                DisplayName = section.DisplayName,
                                SectionId = sectionId,
                                LandingPriority = section.LandingPriority
                            });

                            shop.Sections.Add(sectionId, new List<ShopEntry> { e });
                        }
                        else
                        {
                            shop.Sections[sectionId].Add(e);
                        }
                    }
                }

                shop.Sections.ToList().ForEach(section =>
                {
                    section.Value.Sort(ShopEntryComparer.Comparer);
                });

                shop.SectionInfos.Sort(ShopSectionComparer.Comparer);
                var sections = new Dictionary<string, List<ShopEntry>>();

                foreach (var section in shop.SectionInfos)
                {
                    sections.Add(section.SectionId, shop.Sections.First(predicate: x => x.Key == section.SectionId).Value);
                }

                shop.Sections = sections;

                Program.SaveToCache(JsonConvert.SerializeObject(shop, Formatting.Indented), $"ChicShop_{catalog.Expiration.ToString().Replace(' ', '_').Replace(":", ".").Replace("/", "-")}");
                watch.Stop();
                Console.WriteLine($"Done in {watch.Elapsed}");
                return new ChicResponse<ChicShop>(Status.OK, shop);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ChicResponse<ChicShop>(Status.NOT_READY, new ChicShop());
            }
        }

        public static Catalog GetCatalog()
        {
            if (HasCatalogInCache(out Catalog catalog)) return catalog;
            else 
            {
                catalog = Program.Epic.GetCatalog();
                Program.ClearCache();
                Program.SaveToCache(JsonConvert.SerializeObject(catalog, Formatting.Indented), $"RawShop_{catalog.Expiration.ToString().Replace(' ', '_').Replace(":", ".").Replace("/", "-")}");
                return catalog;
            }
        }

        static bool HasChicShopInCache(out ChicShop shop)
        {
            shop = new ChicShop();

            if (!Program.ListCache().Any(x => x.Contains("ChicShop_"))) return false;

            var date = Program.ListCache().Where(x => x.Contains("ChicShop_"))
                .Max(x => DateTime.Parse(x.Split("ChicShop_")[1].Replace('_', ' ').Replace('.', ':').Replace('-', '/')));

            if (date - DateTime.UtcNow > TimeSpan.Zero)
            {
                shop = JsonConvert.DeserializeObject<ChicShop>(Program.LoadFromCache($"ChicShop_{date.ToString().Replace(' ', '_').Replace(":", ".").Replace("/", "-")}"));
                return true;
            }
            else return false;
        }

        static bool HasCatalogInCache(out Catalog catalog)
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
