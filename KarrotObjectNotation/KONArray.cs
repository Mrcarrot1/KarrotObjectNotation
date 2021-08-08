using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KarrotObjectNotation
{
    /// <summary>
    /// Array of string values for use in KON nodes
    /// </summary>
    public class KONArray : IEnumerable
    {
        public string Name { get; internal set; }
        public List<IKONValue> Items { get; internal set; }
        public KONNode Parent { get; internal set; }

        public int Count { get { return Items.Count; } }

        /// <summary>
        /// Adds an item to the array.
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(IKONValue item)
        {
            Items.Add(item);
        }

        public void AddItem(string item)
        {
            Items.Add(new KONValue<string>(item));
        }

        /// <summary>
        /// Checks if an item exists in the array and removes it if it does.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="removeAll">Whether or not to remove all instances of the item.</param>
        public void RemoveItem(IKONValue item, bool removeAll = false)
        {
            if(Items.Contains(item))
            {
                if(removeAll)
                    Items.RemoveAll(x => x.Equals(item));
                else
                    Items.Remove(item);
            }
        }
        /// <summary>
        /// Checks if the string representation of an item exists in the array and removes it if it does.
        /// </summary>
        /// <param name="item">The string representation of the item to remove.</param>
        /// <param name="removeAll">Whether to remove all instances of the item</param>
        public void RemoveItem(string item, bool removeAll = false)
        {
            if(Items.Where(x => x.ToString() == item).ToList().Count > 0)
            {
                if(removeAll)
                    Items.RemoveAll(x => x.ToString().Equals(item));
                else
                    Items.Remove(Items.FirstOrDefault(x => x.ToString().Equals(item)));
            }
        }

        #region Constructors
        public KONArray(string name)
        {
            Name = name;
            Items = new List<IKONValue>();
            Parent = null;
        }
        public KONArray(string name, KONNode parent)
        {
            Name = name;
            Parent = parent;
            Items = new List<IKONValue>();
        }
        #endregion

        #region Enumeration
        public IEnumerator GetEnumerator()
        {
          return new ItemEnumerator(Items.ToArray());
        }

        private class ItemEnumerator:IEnumerator
        {
            public IKONValue[] itemList;
            int position = -1;

            //constructor
            public ItemEnumerator(IKONValue[] list)
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