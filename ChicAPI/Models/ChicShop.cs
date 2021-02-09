using EpicGames.NET.Models;
using Fortnite_API.Objects.V2;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChicAPI.Models
{
    public struct ChicShop
    {
        [JsonProperty("shopDate")]
        public DateTime ShopDate;
        [JsonProperty("expiration")]
        public DateTime Expiration;
        [JsonProperty("sections")]
        public Dictionary<string, List<ShopEntry>> Sections;
        [JsonProperty("sectionInfos")]
        public List<ShopSection> SectionInfos;
    }

    public struct ShopSection
    {
        [JsonProperty("displayName")]
        public string DisplayName;
        [JsonProperty("id")]
        public string SectionId;
        [JsonProperty("landingPriority")]
        public int LandingPriority;
    }

    public struct ShopEntry
    {
        [JsonProperty("offerId")]
        public string OfferId;
        [JsonProperty("offerType")]
        public string OfferType;
        [JsonProperty("currencyType")]
        public string CurrencyType;
        [JsonProperty("basePrice")]
        public int BasePrice;
        [JsonProperty("regularPrice")]
        public int RegularPrice;
        [JsonProperty("finalPrice")]
        public int FinalPrice;
        [JsonProperty("categories")]
        public string[] Categories;
        [JsonProperty("items")]
        public List<EntryItem> Items;
        [JsonProperty("sortPriority")]
        public int SortPriority;
        [JsonProperty("bundle")]
        public BrShopV2StoreFrontEntryBundle Bundle;
        [JsonProperty("metaInfo")]
        public KeyValue[] MetaInfo;
        [JsonProperty("meta")]
        public Dictionary<string, string> Meta;
    }

    public struct EntryItem
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("quantity")]
        public int Quantity;
        [JsonProperty("icon")]
        public Uri Image;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("rarity")]
        public BrCosmeticV2Rarity Rarity;
        [JsonProperty("series")]
        public BrCosmeticV2Series Series;
        [JsonProperty("type")]
        public BrCosmeticV2Type Type;
        [JsonProperty("shopHistory")]
        public DateTime[] ShopHistory;
    }

    public class ShopSectionComparer : IComparer<ShopSection>
    {
        public static ShopSectionComparer Comparer = new ShopSectionComparer();

        public int Compare(ShopSection x, ShopSection y)
        {
            string xId = x.SectionId;
            string yId = y.SectionId;


            if (xId.Contains("Featured") || yId.Contains("Featured"))
            {
                if (xId == "Featured") return -1;
                if (yId == "Featured") return 1;

                int.TryParse(xId.Replace("Featured", ""), out int xFeatured);
                int.TryParse(yId.Replace("Featured", ""), out int yFeatured);

                if (xFeatured > yFeatured) return -1;
                if (yFeatured > xFeatured) return 1;
            }

            if (xId.Contains("Daily") || yId.Contains("Daily"))
            {
                if (xId.Contains("Featured")) return -1;
                if (yId.Contains("Featured")) return 1;

                if (xId == "Daily") return -1;
                if (yId == "Daily") return 1;

                int.TryParse(xId.Replace("Daily", ""), out int xDaily);
                int.TryParse(yId.Replace("Daily", ""), out int yDaily);

                if (xDaily > yDaily) return -1;
                if (yDaily > xDaily) return 1;
            }

            return x.LandingPriority > y.LandingPriority ? -1 : x.LandingPriority < y.LandingPriority ?
                1 : 0;
        }
    }
}