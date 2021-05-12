using System;
using System.Collections;
using System.Collections.Generic;

namespace KarrotObjectNotation
{
    /// <summary>
    /// Array of string values for use in KON nodes
    /// </summary>
    public class KONArray : IEnumerable
    {
        public string Name { get; internal set; }
        public List<string> Items { get; internal set; }
        public KONNode Parent { get; internal set; }

        /// <summary>
        /// Adds an item to the array.
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(string item)
        {
            Items.Add(item);
        }

        /// <summary>
        /// Checks if an item exists in the array and removes it if it does.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="removeAll">Whether or not to remove all instances of the item.</param>
        public void RemoveItem(string item, bool removeAll = false)
        {
            if(Items.Contains(item))
            {
                if(removeAll)
                    Items.RemoveAll(x => x.Equals(item));
                else
                    Items.Remove(item);
            }
        }

        #region Constructors
        public KONArray(string name)
        {
            Name = name;
            Items = new List<string>();
            Parent = null;
        }
        public KONArray(string name, KONNode parent)
        {
            Name = name;
            Parent = parent;
            Items = new List<string>();
        }
        #endregion

        #region Enumeration
        public IEnumerator GetEnumerator()
        {
          return new ItemEnumerator(Items.ToArray());
        }

        private class ItemEnumerator:IEnumerator
        {
            public string[] itemList;
            int position = -1;

            //constructor
            public ItemEnumerator(string[] list)
            {
                itemList=list;
            }
            private IEnumerator getEnumerator()
            {
                return (IEnumerator)this;
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
        }
        #endregion
    }
}