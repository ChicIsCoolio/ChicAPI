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
        [JsonProperty("expiration")]
        public DateTime Expiration;
        [JsonProperty("sections")]
        public Dictionary<string, ShopEntry[]> Sections;
        [JsonProperty("sectionInfos")]
        public ShopSection[] SectionInfos;
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
        public EntryItem[] Items;
        [JsonProperty("sortPriority")]
        public int SortPriority;
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
}