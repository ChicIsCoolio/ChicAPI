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
        public Dictionary<ShopSection, ShopEntry[]> Sections;
    }

    public struct ShopSection
    {
        [JsonProperty("id")]
        public string SectionId;
    }

    public struct ShopEntry
    {
        [JsonProperty("items")]
        public EntryItem[] Items;
    }

    public struct EntryItem
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("quantity")]
        public int Quantity;
    }
}