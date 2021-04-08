using System;
using System.Collections.Generic;

namespace KarrotObjectNotation
{
    /// <summary>
    /// Array of string values for use in KON nodes
    /// </summary>
    public class KONArray
    {
        public string Name { get; set; }
        public List<string> Items { get; set; }
        public KONNode Parent { get; set; }

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
    }
}