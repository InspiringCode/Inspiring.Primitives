using System;
using System.Collections.Generic;
using System.Text;

namespace Inspiring {
    public interface IResultItem {
    }

    public interface IResultItemWithInfo : IResultItem {
        bool IsError { get; }

        string Message { get; }
    }
}
