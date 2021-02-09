using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace ChicAPI.Chic
{
    public static class ChicExtensions
    {
        public static void Add<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, KeyValuePair<TKey, TValue> value)
            => dictionary.Add(value.Key, value.Value);

        public static void Add<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> values)
           => values.ToList().ForEach(value => dictionary.Add(value.Key, value.Value));

        public static void Sort<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IComparer<KeyValuePair<TKey, TValue>> comparer)
        {
            var list = dictionary.ToList();
            list.Sort(comparer);
            dictionary.Add(list);
        }
    }
}
