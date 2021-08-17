using System;
using System.Collections;
using System.Collections.Generic;

namespace KarrotObjectNotation
{
    public class KONNode
    {
        /// <summary>
        /// The node's name.
        /// </summary>
        /// <value></value>
        public string Name { get; set; }
        /// <summary>
        /// The node which contains this node.
        /// </summary>
        /// <value></value>
        public KONNode Parent { get; internal set; }
        /// <summary>
        /// The values within this node.
        /// </summary>
        /// <value></value>
        public Dictionary<string, object> Values { get; }
        /// <summary>
        /// The list of arrays within this node.
        /// </summary>
        /// <value></value>
        public List<KONArray> Arrays { get; }
        /// <summary>
        /// The list of nodes within this node.
        /// </summary>
        /// <value></value>
        public List<KONNode> Children { get; }
        /// <summary>
        /// The number of parent nodes this node has.
        /// </summary>
        /// <value></value>
        public int Depth { get; internal set; }

        /// <summary>
        /// Adds the given node to this node's children.
        /// </summary>
        /// <param name="node"></param>
        public void AddChild(KONNode node)
        {
            node.Parent = this;
            node.Depth = Depth + 1;
            Children.Add(node);
        }

        /// <summary>
        /// Adds a value to the dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddValue(string key, object value)
        {
            Values.Add(key, value);
        }
        /// <summary>
        /// Adds a value to the dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <summary>
        /// Removes a value from the dictionary.
        /// </summary>
        /// <param name="key"></param>
        public void RemoveValue(string key)
        {
            Values.Remove(key);
        }
        

        /// <summary>
        /// Adds the given array to this node's arrays.
        /// </summary>
        /// <param name="array"></param>
        public void AddArray(KONArray array)
        {
            array.Parent = this;
            Arrays.Add(array);
        }
        
        #region Constructors
        public KONNode(string name)
        {
            Name = name;
            Values = new Dictionary<string, object>();
            Children = new List<KONNode>();
            Arrays = new List<KONArray>();
            Parent = null;
        }
        public KONNode(string name, KONNode parent)
        {
            Name = name;
            Parent = parent;
            Values = new Dictionary<string, object>();
            Children = new List<KONNode>();
            Arrays = new List<KONArray>();
        }
        #endregion
        public override string ToString()
        {
            return KONWriter.Default.Write(this);
        }
        public string ToString(KONWriter writer)
        {
            return writer.Write(this);
        }
    }
}