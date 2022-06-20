using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KarrotObjectNotation
{
    /// <summary>
    /// Array of string values for use in KON nodes
    /// </summary>
    public class KONArray : IEnumerable<object>
    {
        /// <summary>
        /// The array's name.
        /// </summary>
        /// <value></value>
        public string Name { get; internal set; }
        /// <summary>
        /// The list of items within the array.
        /// </summary>
        /// <value></value>
        public List<object> Items { get; internal set; }
        /// <summary>
        /// The node which contains this array.
        /// </summary>
        /// <value></value>
        public KONNode Parent { get; internal set; }
        /// <summary>
        /// The number of items within this array.
        /// </summary>
        /// <value></value>
        public int Count { get { return Items.Count; } }

        /// <summary>
        /// Adds an item to the array.
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(object item)
        {
            Items.Add(item);
        }

        /// <summary>
        /// Checks if an item exists in the array and removes it if it does.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="removeAll">Whether or not to remove all instances of the item.</param>
        public void RemoveItem(object item, bool removeAll = false)
        {
            if (Items.Contains(item))
            {
                if (removeAll)
                    Items.RemoveAll(x => x.Equals(item));
                else
                    Items.Remove(item);
            }
        }

        #region Constructors
        public KONArray(string name)
        {
            Name = name;
            Items = new List<object>();
            Parent = null;
        }
        public KONArray(string name, KONNode parent)
        {
            Name = name;
            Parent = parent;
            Items = new List<object>();
        }
        #endregion

        #region Enumeration
        public IEnumerator<object> GetEnumerator()
        {
            return new ItemEnumerator(Items.ToArray());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ItemEnumerator(Items.ToArray());
        }

        private class ItemEnumerator : IEnumerator, IEnumerator<object>
        {
            public object[] itemList;
            int position = -1;

            //constructor
            public ItemEnumerator(object[] list)
            {
                itemList = list;
            }
            private IEnumerator<object> getEnumerator()
            {
                return (IEnumerator<object>)this;
            }
            //IEnumerator
            public bool MoveNext()
            {
                position++;
                return (position < itemList.Length);
            }
            //IEnumerator
            public void Reset()
            {
                position = -1;
            }
            //IEnumerator
            public object Current
            {
                get
                {
                    try
                    {
                        return itemList[position];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
            public void Dispose()
            {

            }
        }

        public object this[int index]
        {
            get
            {
                return Items[index];
            }
            set
            {
                Items[index] = value;
            }
        }
        #endregion
    }
}