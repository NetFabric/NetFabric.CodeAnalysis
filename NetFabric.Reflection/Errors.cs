using System;

namespace NetFabric.Reflection
{
    [Flags]
    public enum Errors
    {
        None = 0x0000,
        MissingGetEnumerable = 0x0001,
        MissingCurrent = 0x0010,
        MissingMoveNext = 0x0100,
    }
}
