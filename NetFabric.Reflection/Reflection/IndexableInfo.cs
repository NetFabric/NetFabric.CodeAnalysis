using System;
using System.Reflection;

namespace NetFabric.Reflection;

/// <summary>
/// Contains information about methods and properties that will be used to enumerate a given type using the indexer.
/// </summary>
public class IndexableInfo
{
    /// <summary>
    /// Information on the indexer with a single parameter of type <see cref="int"/>.
    /// </summary>
    /// <remarks><c>null</c> if indexable is an array.</remarks>
    public PropertyInfo? Indexer { get; }
    
    /// <summary>
    /// Information on the indexer with a single parameter of type <see cref="int"/>.
    /// </summary>
    public PropertyInfo CountOrLength { get; }

    internal IndexableInfo(PropertyInfo indexer, PropertyInfo countOrLength)
    {
        Indexer = indexer;
        CountOrLength = countOrLength;
    }

    public object? InvokeIndexer(object instance, int index)
    {
        try
        {
            return Indexer is null
                ? ((Array)instance).GetValue(index)
                : Indexer.GetValue(instance, new object[] { index });
        }
        catch (TargetInvocationException exception)
        {
            throw new EnumerationException($"Unhandled exception in {Indexer.DeclaringType!.Name}.{Indexer!.Name}", exception.InnerException!);
        }
    }

    public int InvokeCountOrLength(object instance)
    {
        try
        {
            return (int)CountOrLength.GetValue(instance);
        }
        catch (TargetInvocationException exception)
        {
            throw new EnumerationException($"Unhandled exception in {CountOrLength.DeclaringType!.Name}.{CountOrLength!.Name}", exception.InnerException!);
        }
    }
}