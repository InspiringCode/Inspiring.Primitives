using System.Threading.Tasks;

namespace Inspiring.Primitives.Core {
    public abstract class ResultItem : IResultItem {
        public static implicit operator Task<Result>(ResultItem item)
            => Task.FromResult((Result)item.MustNotBeNull(nameof(item)));
    }
}
