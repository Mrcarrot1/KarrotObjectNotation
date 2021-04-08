using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace KarrotObjectNotation
{
    public class KONWriter
    {
        public CaseWriteMode NodeNameWriteMode { get; set; }
        public CaseWriteMode KeyWriteMode { get; set; }
        public CaseWriteMode ValueWriteMode { get; set; }
        public string Write(KONNode node, int currentDepth = 0)
        {
            //It feels wrong to do it like this but this way it's nice and neatly indented
            string indent = "";
            for(int i = 0; i < currentDepth; i++)
            {
                indent += "    ";
            }
            string indent2 = indent + "    ";
            string output = $"{indent}{GetCase(node.Name, NodeNameWriteMode)}\n{indent}{{";
            foreach(KeyValuePair<string, string> pair in node.Values)
            {
                output += $"\n{indent2}{GetCase(pair.Key, KeyWriteMode)} = {GetCase(pair.Value, ValueWriteMode)}";
            }
            for(int i = 0; i < node.Children.Count; i++)
            {
                output += $"\n{Write(node.Children[i], currentDepth + 1)}";
            }
            for(int i = 0; i < node.Arrays.Count; i++)
            {
                KONArray currentArray = node.Arrays[i];
                output += $"\n{WriteArray(currentArray, currentDepth + 1)}";
            }
            output += $"\n{indent}}}";
            return output;
        }
        public string WriteArray(KONArray array, int currentDepth = 0)
        {
            string indent = "";
            for(int i = 0; i < currentDepth; i++)
            {
                indent += "    ";
            }
            string indent2 = indent + "    ";
            string output = $"{indent}{GetCase(array.Name, NodeNameWriteMode)}\n{indent}[";
            foreach(string str in array.Items)
            {
                output += $"\n{indent2}{GetCase(str, ValueWriteMode)}";
            }
            output += $"\n{indent}]";
            return output;
            
        }
        private string GetCase(string str, CaseWriteMode cwm)
        {
            if(cwm == CaseWriteMode.ToUpper) return str.ToUpper();
            if(cwm == CaseWriteMode.ToLower) return str.ToLower();
            if(cwm == CaseWriteMode.ToTitle) return new CultureInfo("en-US",false).TextInfo.ToTitleCase(str);
            else return str;
        }
        public KONWriter(CaseWriteMode nodeNameWriteMode = CaseWriteMode.KeepOriginal, CaseWriteMode keyWriteMode = CaseWriteMode.KeepOriginal, CaseWriteMode valueWriteMode = CaseWriteMode.KeepOriginal)
        {
            NodeNameWriteMode = nodeNameWriteMode;
            KeyWriteMode = keyWriteMode;
            ValueWriteMode = valueWriteMode;
        }
        public enum CaseWriteMode
        {
            KeepOriginal,
            ToUpper,
            ToLower,
            ToTitle
        }
    }
}