using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vbcrypt
{
    internal class ListList<T>
    {
        private readonly List<List<T>> listList = new();

        public ListList<T> Add(List<T> item)
        {
            if(item.Count == 0) { return this; }
            listList.Add(item);
            return this;
        }
        public bool Contains(T item)
        {
            foreach(var list in listList) if (list.Contains(item)) return true;
            return false;
        }

        public void Test()
        {
            this
                .Add(new List<T> { })
                .Add(new List<T> { });
        }

        // Similar to Contains(), but used to map multiple items to a single item.
        // Great for short/long forms of options. [ "--enable", "-e" ] would both coalesce to "--enable".
        public T? Coalesce(T item)
        {
            foreach (var list in listList) if (list.Contains(item)) return list[0];
            return default;
        }

        // Returns a short list consisting of only the first entries from each list in the listList.
        // Basically enumerates all possible values from Coalesce()
        public List<T> CoalescedList { get {
                List<T> shortList = new();
                foreach(var list in listList) shortList.Add(list[0]); // Will never throw IndexOutOfRange exception due to Add() silently ignoring empty lists.
                return shortList;
            } }

    }
}
