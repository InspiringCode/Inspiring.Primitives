using System;
using System.Collections.Generic;
using System.Linq;

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
            func.MustNotBeNull(nameof(func));
            return results.Aggregate(seed: new Result<T>(), (acc, x) =>
                acc.HasValue && x.HasValue ?
                    (acc + x).SetTo(func(acc, x)) :
                    (acc + x));
        }
    }
}
