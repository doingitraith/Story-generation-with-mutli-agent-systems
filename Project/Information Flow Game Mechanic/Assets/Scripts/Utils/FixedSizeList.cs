using System.Collections.Generic;

namespace Utils
{
    public class FixedSizeList<T> : List<T>
    {
        private int _size;

        public FixedSizeList(int size)
            => (_size) = (size);
    
        public new void Add(T item)
        {
            base.Add(item);
            while (base.Count > _size)
                base.RemoveAt(0);
        }
    }
}
