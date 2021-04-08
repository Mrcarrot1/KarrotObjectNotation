using System.Collections;
using System.Collections.Generic;

namespace KarrotObjectNotation
{
    public class KONNode
    {
        public string Name { get; set; }
        public KONNode Parent { get; set; }
        public Dictionary<string, string> Values { get; }
        public List<KONArray> Arrays { get; }
        public List<KONNode> Children { get; }
        public int Depth { get; set; }

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
        public void AddValue(string key, string value)
        {
            Values.Add(key, value);
        }

        /// <summary>
        /// Adds the given array to this node's arrays.
        /// </summary>
        /// <param name="array"></param>
        public void AddArray(KONArray @array)
        {
            @array.Parent = this;
            Arrays.Add(@array);
        }
        
        #region Constructors
        public KONNode(string name)
        {
            Name = name;
            Values = new Dictionary<string, string>();
            Children = new List<KONNode>();
            Arrays = new List<KONArray>();
            Parent = null;
        }
        public KONNode(string name, KONNode parent)
        {
            Name = name;
            Parent = parent;
            Values = new Dictionary<string, string>();
            Children = new List<KONNode>();
            Arrays = new List<KONArray>();
        }
        #endregion
    }
}