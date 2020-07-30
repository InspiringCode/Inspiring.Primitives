using Inspiring.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Inspiring {
    public class Result<T> : Result, IResult<Result<T>> {
        internal static readonly new Result<T> Empty = new Result<T>();
        private readonly T _value;

        public T Value {
            get {
                if (!HasValue)
                    ThrowValueException();

                return _value;
            }
        }

        internal Result(ImmutableList<IResultItem> items)
            : this(false, default!, items) { }

        internal Result(T value, ImmutableList<IResultItem> items)
            : this(true, value, items) { }

        private Result(bool hasValue = false, T value = default, ImmutableList<IResultItem>? items = null)
            : base(hasValue, items) => _value = value;

        public new Result<T> WithoutItems()
            => new Result<T>(HasValue, _value);

        public override bool Equals(object obj) {
            if (obj is Result r) {
                bool valueEquals =
                    HasValue && r.HasValue && r.Equals(_value) ||
                    !HasValue && !r.HasValue && Equals(GetType(), r.GetType());

                return valueEquals && ItemsEqualToItemsOf(r);
            }

            if (HasValue)
                return Equals(_value, obj);

            return false;
        }

        public override int GetHashCode() {
            HashCode code = GetHashcodeOfItems();
            code.Add(typeof(Result<>));
            code.Add(HasValue);
            code.Add(_value);
            return code.ToHashCode();
        }

        public override string ToString() {
            StringBuilder s = new StringBuilder();

            if (HasValue) {
                if (ReferenceEquals(_value, null)) {
                    s.Append("[<null>]");
                } else {
                    s.Append('[');
                    s.Append(_value);
                    s.Append(']');
                }
            }

            string items = base.ToString();
            if (items != "" && s.Length > 0)
                s.Append(' ');
            s.Append(items);

            return s.ToString();
        }

        Result<T> IResult<Result<T>>.Add(IResultItem item)
            => this + item;

        protected override Result CreateCopy(ImmutableList<IResultItem> items)
            => new Result<T>(HasValue, _value, items);

        protected override Result InvokeWithoutItems()
            => WithoutItems();

        private void ThrowValueException() {
            throw new InvalidOperationException();
        }

        public static implicit operator Result<T>(T value)
            => new Result<T>(true, value);

        public static implicit operator Result<T>(VoidResult result)
            => (result ?? throw new ArgumentNullException(nameof(result))).To<T>();

        public static implicit operator VoidResult(Result<T> result)
            => result.ToVoid();

        public static Result<T> operator +(Result<T> result, IResultItem item)
            => (Result<T>)(result ?? throw new ArgumentNullException(nameof(result))).Add(item);

        public static bool operator ==(Result<T> x, T y)
            => Equals(x, y);

        public static bool operator !=(Result<T> x, T y)
            => !Equals(x, y);
    }
}
