#nullable disable

// See https://aka.ms/new-console-template for more information
public class SortedSearcher : ISercher
{
    private class StartsWithComparer : IComparer<string>
    {
        public static readonly StartsWithComparer Instance = new StartsWithComparer();

        public int Compare(string x, string y)
        {
            if (x.StartsWith(y))
                return 0;
            return x.CompareTo(y);
        }
    }

    private List<string> sorted;

    public SortedSearcher(string[] words)
    {
        sorted = new List<string>(words);
        sorted.Sort();
    }

    public bool ContainsFullWord(string word) {

        int pos = sorted.BinarySearch(word);
        return pos >= 0;
    }

    public IEnumerable<string> StartWith(string prefix)
    {
        int count = BinarySearch(sorted, prefix, StartsWithComparer.Instance, out var index);
        return sorted.Skip(index).Take(count);
    }

    /// <summary>
    /// Searches a sorted list for an item via binary search. The list must be sorted
    /// by the ordering in the passed instance of IComparer&lt;T&gt;.
    /// </summary>
    /// <param name="list">The sorted list to search.</param>
    /// <param name="item">The item to search for.</param>
    /// <param name="comparer">The comparer instance used to sort the list. Only
    /// the Compare method is used.</param>
    /// <param name="index">Returns the first index at which the item can be found. If the return
    /// value is zero, indicating that <paramref name="item"/> was not present in the list, then this
    /// returns the index at which <paramref name="item"/> could be inserted to maintain the sorted
    /// order of the list.</param>
    /// <returns>
    /// The number of items equal to <paramref name="item"/> that appear in the list.
    /// </returns>
    public static int BinarySearch<T>(IList<T> list, T item, IComparer<T> comparer, out int index)
    {
        if (list == null)
            throw new ArgumentNullException("list");
        if (comparer == null)
            throw new ArgumentNullException("comparer");

        int l = 0;
        int r = list.Count;

        while (r > l)
        {
            int m = l + (r - l) / 2;
            T middleItem = list[m];
            int comp = comparer.Compare(middleItem, item);
            if (comp < 0)
            {
                // middleItem < item
                l = m + 1;
            }
            else if (comp > 0)
            {
                r = m;
            }
            else
            {
                // Found something equal to item at m. Now we need to find the start and end of this run of equal items.
                int lFound = l, rFound = r, found = m;

                // Find the start of the run.
                l = lFound;
                r = found;
                while (r > l)
                {
                    m = l + (r - l) / 2;
                    middleItem = list[m];
                    comp = comparer.Compare(middleItem, item);
                    if (comp < 0)
                    {
                        // middleItem < item
                        l = m + 1;
                    }
                    else
                    {
                        r = m;
                    }
                }
                System.Diagnostics.Debug.Assert(l == r, "Left and Right were not equal");
                index = l;

                // Find the end of the run.
                l = found;
                r = rFound;
                while (r > l)
                {
                    m = l + (r - l) / 2;
                    middleItem = list[m];
                    comp = comparer.Compare(middleItem, item);
                    if (comp <= 0)
                    {
                        // middleItem <= item
                        l = m + 1;
                    }
                    else
                    {
                        r = m;
                    }
                }
                System.Diagnostics.Debug.Assert(l == r, "Left and Right were not equal");
                return l - index;
            }
        }

        // We did not find the item. l and r must be equal. 
        System.Diagnostics.Debug.Assert(l == r);
        index = l;
        return 0;
    }

    public IEnumerable<string> ContainsSubstring(string suffix)
    {
        throw new NotImplementedException();
    }
}


// ContainsFullWord
// StartsWith
// EndsWith
// ContaintsSubstring