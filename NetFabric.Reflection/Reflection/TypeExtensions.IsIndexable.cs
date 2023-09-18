using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NetFabric.Reflection;

public static partial class TypeExtensions
{        
    public static bool IsIndexable(this Type type)
        => IsIndexable(type, out _, out _);

    public static bool IsIndexable(this Type type, [NotNullWhen(true)] out IndexableInfo? indexableInfo)
        => IsIndexable(type, out indexableInfo, out _);

    public static bool IsIndexable(this Type type,
        [NotNullWhen(true)] out IndexableInfo? indexableInfo,
        out IsIndexableError error)
    {
        var countOrLength = type.GetPublicReadProperty(NameOf.Count);
        countOrLength ??= type.GetPublicReadProperty(NameOf.Length);
        if (countOrLength is null || !countOrLength.PropertyType.IsIntegerType())
        {
            indexableInfo = default;
            error = IsIndexableError.MissingCountOrLength;
            return false;
        }

        var indexer = default(PropertyInfo);
        if (!type.IsArray)
        {
            indexer = type.GetPublicReadIndexer(countOrLength.PropertyType);
            if (indexer is null)
            {
                indexableInfo = default;
                error = IsIndexableError.MissingIndexer;
                return false;
            }
        }

        indexableInfo = new IndexableInfo(indexer, countOrLength);
        error = IsIndexableError.None;
        return true;
    }
}