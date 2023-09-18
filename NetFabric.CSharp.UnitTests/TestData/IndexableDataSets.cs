using NetFabric.CodeAnalysis;
using System;
using System.Collections;
using Xunit;
using static NetFabric.CSharp.TestData.EnumerableDataSets;

namespace NetFabric.CSharp.TestData;

public static partial class IndexableDataSets
{

    public class IndexableTestData
    {
        public Type IndexerDeclaringType;
        public Type CountOrLengthDeclaringType;
    }

    public static TheoryData<Type, IndexableTestData> Arrays =>
        new()
        {
            {
                typeof(int[]),
                new IndexableTestData
                {
                    IndexerDeclaringType = typeof(int[]),
                    CountOrLengthDeclaringType = typeof(int[]),
                }
            },
            {
                typeof(Span<int>),
                new IndexableTestData
                {
                    IndexerDeclaringType = typeof(Span<int>),
                    CountOrLengthDeclaringType = typeof(Span<int>),
                }
            },
            {
                typeof(ReadOnlySpan<int>),
                new IndexableTestData
                {
                    IndexerDeclaringType = typeof(ReadOnlySpan<int>),
                    CountOrLengthDeclaringType = typeof(ReadOnlySpan<int>),
                }
            },
        };

    public static TheoryData<Type, IndexableTestData> Indexables =>
        new()
        {
            {
                typeof(IndexableWithCount<int>),
                new IndexableTestData 
                {
                    IndexerDeclaringType = typeof(IndexableWithCount<int>),
                    CountOrLengthDeclaringType = typeof(IndexableWithCount<int>),
                }
            },
            {
                typeof(IndexableWithLength<int>),
                new IndexableTestData
                {
                    IndexerDeclaringType = typeof(IndexableWithLength<int>),
                    CountOrLengthDeclaringType = typeof(IndexableWithLength<int>),
                }
            },
            {
                typeof(IIndexable<int>),
                new IndexableTestData
                {
                    IndexerDeclaringType = typeof(IIndexable<int>),
                    CountOrLengthDeclaringType = typeof(IIndexable<int>),
                }
            },
            {
                typeof(IDerivedIndexable<int>),
                new IndexableTestData
                {
                    IndexerDeclaringType = typeof(IIndexable<int>),
                    CountOrLengthDeclaringType = typeof(IIndexable<int>),
                }
            },
        };

    public static TheoryData<Type, IsIndexableError> InvalidIndexables =>
        new()
        {
            {
                typeof(IndexableWithMissingIndexer<int>),
                IsIndexableError.MissingIndexer
            },
            {
                typeof(IndexableWithMissingCountOrLength<int>),
                IsIndexableError.MissingCountOrLength
            },
            {
                typeof(IndexableWithDifferentIndexingTypes<int>),
                IsIndexableError.MissingIndexer
            },
        };

}
