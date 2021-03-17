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

        public void AddChild(KONNode node)
        {
            node.Parent = this;
            node.Depth = Depth + 1;
            Children.Add(node);
        }
        public void AddValue(string key, string value)
        {
            Values.Add(key, value);
        }
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
        public KONNode(string name, Dictionary<string, string> values)
        {
            Name = name;
            Values = values;
            Children = new List<KONNode>();
            Arrays = new List<KONArray>();
        }
        public KONNode(string name, List<KONNode> children)
        {
            Name = name;
            Children = children;
            Values = new Dictionary<string, string>();
            Arrays = new List<KONArray>();
        }
        public KONNode(string name, List<KONArray> arrays)
        {
            Name = name;
            Arrays = arrays;
            Children = new List<KONNode>();
            Values = new Dictionary<string, string>();
        }
        public KONNode(string name, Dictionary<string, string> values, List<KONNode> children)
        {
            Name = name;
            Values = values;
            Children = children;
            Arrays = new List<KONArray>();
        }
        #endregion
    }
}