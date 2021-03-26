using System.Collections;
using System.Collections.Generic;
using NetFabric.Hyperlinq;

namespace NetFabric.TestData
{
    public readonly struct RangeEnumerable
        : IValueReadOnlyCollection<int, RangeEnumerable.DisposableEnumerator>
    {    
        public RangeEnumerable(int count)
        {
            Count = count;
        }

        public int Count { get; }
            
        public readonly Enumerator GetEnumerator() => new(Count);
        readonly DisposableEnumerator IValueEnumerable<int, DisposableEnumerator>.GetEnumerator() => new(Count);
        readonly IEnumerator<int> IEnumerable<int>.GetEnumerator() => new DisposableEnumerator(Count);
        readonly IEnumerator IEnumerable.GetEnumerator() => new DisposableEnumerator(Count);
        
        public struct Enumerator
        {
            readonly int count;
            int current;
            
            internal Enumerator(int count)
            {
                this.count = count;
                current = -1;
            }
            
            public readonly int Current => current;
            
            public bool MoveNext() => ++current < count;
        }
        
        public struct DisposableEnumerator : IEnumerator<int>
        {
            readonly int count;
            int current;
            
            internal DisposableEnumerator(int count)
            {
                this.count = count;
                current = -1;
            }
            
            public int Current => current;
            object IEnumerator.Current => current;
            
            public bool MoveNext() => ++current < count;
            
            public void Reset() => current = -1;
            
            public void Dispose() {}
        }
    }
}