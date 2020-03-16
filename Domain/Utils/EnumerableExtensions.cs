namespace DispatcherDesktop.Domain.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtensions
    {
        public static int GetSequenceHashCode<TItem>(this IEnumerable<TItem> self)
        {
            if (self == null) return 0;
            const int seedValue = 0x2D2816FE;
            const int primeNumber = 397;
            return self.Aggregate(seedValue, (current, item) => (current * primeNumber) + (Equals(item, default(TItem)) ? 0 : item.GetHashCode()));
        }
        
        public static IEnumerable<List<T>> SplitByChunk<T>(this List<T> self, int size=30)  
        {        
            for (var i = 0; i < self.Count; i += size) 
            { 
                yield return self.GetRange(i, Math.Min(size, self.Count - i)); 
            }  
        } 
    }
}