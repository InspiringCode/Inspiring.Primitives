using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspiring {
    public static class ResultExtensions {
        public static async Task<Result> ToVoid<T>(this Task<Result<T>> resultTask)
            => (await resultTask).ToVoid();

        public static IEnumerable<TItem> Get<TItem>(this IResult result, Func<TItem, bool> predicate) where TItem : IResultItem {
            result.MustNotBeNull(nameof(result));
            predicate.MustNotBeNull(nameof(predicate));
            return result.Get<TItem>().Where(predicate);
        }

        public static bool Has<TItem>(this IResult result, Func<TItem, bool>? predicate = null) where TItem : IResultItem {
            result.MustNotBeNull(nameof(result));
            return result.Get<TItem>().Any(predicate ?? (_ => true));
        }

        public static Result Combine(this IEnumerable<Result> results) => results
            .MustNotBeNull(nameof(results))
            .Aggregate(seed: Result.Empty, (agg, next) => agg + next);

        public static Result<T> Combine<T>(this IEnumerable<Result<T>> results) => results
            .MustNotBeNull(nameof(results))
            .Aggregate(seed: new Result<T>(), (agg, next) => agg + next);

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
