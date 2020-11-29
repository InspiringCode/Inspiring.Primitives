using Inspiring.Primitives.Core;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspiring {
    public readonly struct Result<T> : IResult, IResultType<Result<T>>, IEquatable<Result<T>> {
        private readonly ImmutableList<IResultItem>? _items;
        private readonly T _value;
        private readonly bool _hasValue;

        public bool HasValue => _hasValue;

        private ImmutableList<IResultItem> Items
            => _items ?? ImmutableList.Create<IResultItem>();

        public T Value {
            get {
                if (!HasValue)
                    ThrowValueException();

                return _value;
            }
        }

        public bool HasErrors => Items
            .OfType<IResultItemInfo>()
            .Any(i => i.IsError);

        internal Result(ImmutableList<IResultItem>? items)
            : this(false, default!, items) { }


        internal Result(T value, ImmutableList<IResultItem>? items)
            : this(true, value, items) { }

        private Result(bool hasValue = false, T value = default!, ImmutableList<IResultItem>? items = null) {
            _items = items;
            _value = value!;
            _hasValue = hasValue;
        }


        /**************************** ITEM METHODS ***************************/

        public Result<T> Add(IResultItem item)
            => new Result<T>(_hasValue, _value, Items.Add(item.MustNotBeNull(nameof(item))));

        public Result<T> WithoutItems()
            => new Result<T>(_hasValue, _value, null);

        public IEnumerable<TItem> Get<TItem>() where TItem : IResultItem
            => Items.OfType<TItem>();


        /*************************** VALUE METHODS ***************************/

        public Result ToVoid()
            => new Result(_items);

        public Result<U> To<U>()
            => new Result<U>(_items);

        public Result<U> SetTo<U>(U value)
            => new Result<U>(value, _items);

        public Result<T> Or(T defaultValue) =>
            HasValue ? this : SetTo(defaultValue);


        /*********************** TRANSFORMATION METHODS **********************/

        public Result<U> Transform<U>(Func<T, Result<U>> transformation) =>
            HasValue ?
                ToVoid() + transformation(Value) :
                To<U>();

        public Result<U> Transform<U>(Func<T, U> transformation) =>
            HasValue ?
                ToVoid() + Result.From(transformation(Value)) :
                To<U>();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Result<R> SelectMany<U, R>(Func<T, Result<U>> transformation, Func<T, U, R> resultSelector) {
            if (!HasValue)
                return To<R>();

            Result<U> transformed = Transform(transformation);
            return transformed.HasValue ?
                transformed.SetTo(resultSelector(Value, transformed.Value)) :
                transformed.To<R>();
        }

        /****************************** EQUALITY *****************************/

        public bool Equals(Result<T> other) =>
            _hasValue == other._hasValue &&
            Equals(_value, other._value) &&
            Items.SequenceEqual(other.Items);

        public override bool Equals(object obj) {
            if (obj is IResult r) {
                bool valueEquals =
                    HasValue && r.HasValue && r.Equals(_value) ||
                    !HasValue && !r.HasValue && Equals(GetType(), r.GetType());

                return valueEquals && Items.SequenceEqual(r.Get<IResultItem>());
            }

            if (HasValue)
                return Equals(_value, obj);

            return false;
        }

        public override int GetHashCode() {
            HashCode code = Result.GetItemsHashCode(Items);
            code.Add(typeof(Result<>));
            code.Add(HasValue);
            code.Add(_value);
            return code.ToHashCode();
        }


        /****************************** TOSTRING *****************************/

        public override string ToString() {
            StringBuilder s = new StringBuilder();

            if (HasValue) {
                if (ReferenceEquals(_value, null)) {
                    s.Append("[<null>]");
                } else {
                    s.Append('[');
                    if (_value is string) {
                        s.Append('"');
                        s.Append(_value);
                        s.Append('"');
                    } else {
                        s.Append(_value);
                    }
                    s.Append(']');
                }
            }

            string items = Result.FormatItemsShort(Items);
            if (items != "" && s.Length > 0)
                s.Append(' ');
            s.Append(items);

            return s.ToString();
        }


        /************************** PRIVATE METHODS **************************/

        private void ThrowValueException() {
            throw new InvalidOperationException(
                $"The result '{this}' does not have a value. Use 'HasValue' to check if a result has a value.");
        }

        private Result<T> CreateCopy(ImmutableList<IResultItem>? items)
            => new Result<T>(_hasValue, _value, items);


        /************************** CAST OPERATORS ***************************/

        public static implicit operator Result<T>(T value)
            => new Result<T>(true, value);

        public static implicit operator Result<T>(Result result)
            => result.To<T>();

        public static implicit operator Result<T>(ResultItem item)
            => new Result<T>(ImmutableList.Create<IResultItem>(item));

        public static implicit operator Task<Result<T>>(Result<T> result)
            => Task.FromResult(result);


        /************************** MERGE OPERATORS **************************/

        public static Result<T> operator +(Result<T> first, Result second)
            => first.CreateCopy(first.Items.AddRange(second.Items));

        public static Result<T> operator +(Result first, Result<T> second)
            => second.CreateCopy(first.Items.AddRange(second.Items));

        public static Result<T> operator +(Result<T> first, Result<T> second) {
            ImmutableList<IResultItem>? items = first.Items.AddRange(second.Items);
            return second.HasValue ?
                second.CreateCopy(items) :
                first.CreateCopy(items);
        }


        /************************* EQUALITY OPERATORS ************************/

        public static bool operator ==(Result<T> x, T y)
            => Equals(x, y);

        public static bool operator !=(Result<T> x, T y)
            => !Equals(x, y);

        public static bool operator ==(Result<T> x, object y)
            => Equals(x, y);

        public static bool operator !=(Result<T> x, object y)
            => !Equals(x, y);
    }
}
