using System.Collections;
using System.Collections.Generic;
using NetFabric.Hyperlinq;

namespace NetFabric.CSharp.TestData
{
    public readonly struct RangeEnumerable
        : IValueReadOnlyCollection<int, RangeEnumerable.DisposableEnumerator>
    {    
        public RangeEnumerable(int count)
            => Count = count;

        public int Count { get; }
            
        public Enumerator GetEnumerator() 
            => new(Count);
        DisposableEnumerator IValueEnumerable<int, DisposableEnumerator>.GetEnumerator() 
            => new(Count);
        IEnumerator<int> IEnumerable<int>.GetEnumerator() 
            // ReSharper disable once HeapView.BoxingAllocation
            => new DisposableEnumerator(Count);
        IEnumerator IEnumerable.GetEnumerator() 
            // ReSharper disable once HeapView.BoxingAllocation
            => new DisposableEnumerator(Count);
        
        public struct Enumerator
        {
            readonly int count;
            int current;
            
            internal Enumerator(int count)
                => (this.count, current) = (count, -1);
            
            public readonly int Current 
                => current;
            
            public bool MoveNext() 
                => ++current < count;
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
            
            public readonly int Current 
                => current;
            object IEnumerator.Current 
                // ReSharper disable once HeapView.BoxingAllocation
                => current;
            
            public bool MoveNext() 
                => ++current < count;
            
            public void Reset() 
                => current = -1;
            
            public void Dispose() 
            {}
        }
    }
}