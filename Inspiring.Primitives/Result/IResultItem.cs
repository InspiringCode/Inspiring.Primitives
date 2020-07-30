using System;
using System.Collections.Generic;
using System.Text;

namespace Inspiring {
    public interface IResultItem {
    }

    public interface IResultItemInfo{
        bool IsError { get; }
    }
}
