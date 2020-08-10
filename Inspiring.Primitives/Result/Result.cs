#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()

using Inspiring.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Inspiring {
    public readonly struct Result : IResultType<Result>, IEquatable<Result> {
        public static readonly Result Empty = new Result();

        internal readonly ImmutableList<IResultItem>? _items;

        public bool HasValue => false;

        internal ImmutableList<IResultItem> Items
            => _items ?? ImmutableList.Create<IResultItem>();

        internal Result(ImmutableList<IResultItem>? items)
            => _items = items;


        /**************************** ITEM METHODS ***************************/

        public Result Add(IResultItem item)
            => new Result(Items.Add(item));

        public Result WithoutItems()
            => new Result();

        public IEnumerable<TItem> Get<TItem>() where TItem : IResultItem
            => Items.OfType<TItem>();


        /*************************** VALUE METHODS ***************************/

        public Result<T> To<T>()
            => new Result<T>(_items);

        public Result<T> SetTo<T>(T value)
            => new Result<T>(value, _items);


        /****************************** EQUALITY *****************************/

        public bool Equals(Result other)
            => Items.SequenceEqual(other.Items);

        public override bool Equals(object obj)
            => obj is Result r && Equals(r);

        public override int GetHashCode() {
            HashCode code = GetItemsHashCode(Items);
            code.Add(typeof(Result));
            return code.ToHashCode();
        }


        /****************************** TOSTRING *****************************/

        public override string ToString() {
            string itemsText = FormatItemsShort(Items);
            return itemsText != "" ? itemsText : "<void>";
        }


        /************************** FACTORY METHODS **************************/

        public static Result<T> Of<T>() => default;

        public static Result<T> From<T>(T value) => value;


        /************************** CAST OPERATORS ***************************/

        public static implicit operator Result(ResultItem item)
            => new Result(ImmutableList.Create<IResultItem>(item));


        /************************** MERGE OPERATORS **************************/

        public static Result operator +(Result first, Result second)
            => new Result(first.Items.AddRange(second.Items));


        /************************* EQUALITY OPERATORS ************************/

        public static bool operator ==(Result first, Result second)
            => first.Equals(second);

        public static bool operator !=(Result first, Result second)
            => !first.Equals(second);

        public static bool operator ==(Result first, object second)
            => first.Equals(second);

        public static bool operator !=(Result first, object second)
            => !first.Equals(second);


        /**************************** ITEM HELPERS ***************************/

        internal static string FormatItemsShort(IImmutableList<IResultItem> items) {
            return items.Count switch
            {
                0 => "",
                1 => $"{items.First()}",
                _ => $"{items.Last()} (and {items.Count - 1} more items)"
            };
        }
        
        internal static HashCode GetItemsHashCode(IImmutableList<IResultItem> items) {
            HashCode code = new HashCode();
            foreach (IResultItem item in items)
                code.Add(item);
            return code;
        }
    }
}
