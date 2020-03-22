using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Utils
{
    public static class Extensions
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> values) where T : class
        {
            if (values == null)
                return;
            foreach (var value in values)
            {
                collection.Add(value);
            }
        }
    }
}
