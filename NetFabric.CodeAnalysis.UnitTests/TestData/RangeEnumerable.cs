using System.Collections;
using System.Collections.Generic;
using NetFabric.Hyperlinq;

namespace NetFabric.CodeAnalysis.TestData
{
    public readonly struct RangeEnumerable
        : IValueEnumerable<int, RangeEnumerable.DisposableEnumerator>
    {    
        readonly int count;
        
        public RangeEnumerable(int count)
        {
            this.count = count;
        }
            
        public readonly Enumerator GetEnumerator() => new Enumerator(count);
        readonly DisposableEnumerator IValueEnumerable<int, DisposableEnumerator>.GetEnumerator() => new DisposableEnumerator(count);
        readonly IEnumerator<int> IEnumerable<int>.GetEnumerator() => new DisposableEnumerator(count);
        readonly IEnumerator IEnumerable.GetEnumerator() => new DisposableEnumerator(count);
        
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