using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inspiring {
    public static class EnumerableExtensions {
        public static Result Combine(this IEnumerable<Result> results) => results
            .MustNotBeNull(nameof(results))
            .Aggregate((agg, next) => agg + next);

        public static Result<T> Combine<T>(this IEnumerable<Result<T>> results) => results
            .MustNotBeNull(nameof(results))
            .Aggregate((agg, next) => agg + next);

        public static Result<TAccumulate> Combine<T, TAccumulate>(
            this IEnumerable<Result<T>> results,
            TAccumulate seed,
            Func<TAccumulate, T, TAccumulate> func
        ) {
            Result value = Result.Empty;

            foreach (Result<T> r in results) {
                if (r.HasValue)
                    seed = func(seed, r.Value);

                value += r.ToVoid();
            }

            return value.SetTo(seed);
        }

        public static Result<T> Combine<T>(
            this IEnumerable<Result<T>> results,
            Func<T, T, T> func
        ) {
            Result<T> agg = new();

            using IEnumerator<Result<T>> e = results.GetEnumerator();
            while (e.MoveNext()) {
                agg += e.Current;

                if (agg.HasValue) {
                    while (e.MoveNext()) {
                        agg += e.Current.ToVoid();

                        if (e.Current.HasValue)
                            agg = agg.SetTo(func(agg, e.Current.Value));
                    }
                }
            }

            return agg;
        }
    }
}
