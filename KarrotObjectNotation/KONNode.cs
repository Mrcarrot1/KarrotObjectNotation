using System;
using System.Collections;
using System.Collections.Generic;

namespace KarrotObjectNotation
{
    public class KONNode
    {
        public string Name { get; set; }
        public KONNode Parent { get; internal set; }
        public Dictionary<string, IKONValue> Values { get; }
        public List<KONArray> Arrays { get; }
        public List<KONNode> Children { get; }
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
        public void AddValue(string key, IKONValue value)
        {
            Values.Add(key, value);
        }
        /// <summary>
        /// Adds a value to the dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddValue(string key, string value)
        {
            Values.Add(key, new KONValue<string>(value));
        }
        /// <summary>
        /// Adds a value to the dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddValue(string key, int value)
        {
            Values.Add(key, new KONValue<int>(value));
        }
        /// <summary>
        /// Adds a value to the dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddValue(string key, uint value)
        {
            Values.Add(key, new KONValue<uint>(value));
        }
        /// <summary>
        /// Adds a value to the dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddValue(string key, long value)
        {
            Values.Add(key, new KONValue<long>(value));
        }
        /// <summary>
        /// Adds a value to the dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddValue(string key, ulong value)
        {
            Values.Add(key, new KONValue<ulong>(value));
        }
        /// <summary>
        /// Adds a value to the dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddValue(string key, float value)
        {
            Values.Add(key, new KONValue<float>(value));
        }
        /// <summary>
        /// Adds a value to the dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddValue(string key, double value)
        {
            Values.Add(key, new KONValue<double>(value));
        }
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
            Values = new Dictionary<string, IKONValue>();
            Children = new List<KONNode>();
            Arrays = new List<KONArray>();
            Parent = null;
        }
        public KONNode(string name, KONNode parent)
        {
            Name = name;
            Parent = parent;
            Values = new Dictionary<string, IKONValue>();
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