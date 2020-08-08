using System;
using System.Collections.Immutable;
using System.Linq;

namespace Inspiring.Core {
    internal static class Utils {

        public static ImmutableList<IResultItem>? Combine(ImmutableList<IResultItem>? first, ImmutableList<IResultItem>? second) {
            if (second == null)
                return first;

            if (first == null)
                return second;

            return first.AddRange(second);
        }

        public static string FormatItemsShort(IImmutableList<IResultItem>? items) {
            if (items == null)
                return "";

            return items.Count switch
            {
                0 => "",
                1 => $"{items.First()}",
                _ => $"{items.Last()} (and {items.Count - 1} more items)"
            };
        }

        public static HashCode GetHashcodeOfItems(IImmutableList<IResultItem>? items) {
            HashCode code = new HashCode();
            if (items != null) {
                foreach (IResultItem item in items)
                    code.Add(item);
            }
            return code;
        }
    }
}
