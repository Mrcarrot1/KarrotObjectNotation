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