//Int: %
//Long: &
//Unsigned: ^
//Float: !
//Double: #
//String: $
//Boolean: @
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;

namespace KarrotObjectNotation
{
    public class KONWriter
    {
        public static KONWriter Default = new KONWriter();
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
            foreach(KeyValuePair<string, IKONValue> pair in node.Values)
            {
                if(pair.Value is KONValue<int> intValue)
                {
                    output += $"\n{indent2}%{GetCase(pair.Key, KeyWriteMode)} = {GetCase(FormatValue(pair.Value.ToString()), ValueWriteMode)}";
                    continue;
                }
                if(pair.Value is KONValue<uint> uintValue)
                {
                    output += $"\n{indent2}%^{GetCase(pair.Key, KeyWriteMode)} = {GetCase(FormatValue(pair.Value.ToString()), ValueWriteMode)}";
                    continue;
                }
                if(pair.Value is KONValue<long> longValue)
                {
                    output += $"\n{indent2}&{GetCase(pair.Key, KeyWriteMode)} = {GetCase(FormatValue(pair.Value.ToString()), ValueWriteMode)}";
                    continue;
                }
                if(pair.Value is KONValue<ulong> ulongValue)
                {
                    output += $"\n{indent2}&^{GetCase(pair.Key, KeyWriteMode)} = {GetCase(FormatValue(pair.Value.ToString()), ValueWriteMode)}";
                    continue;
                }
                if(pair.Value is KONValue<float> floatValue)
                {
                    output += $"\n{indent2}!{GetCase(pair.Key, KeyWriteMode)} = {GetCase(FormatValue(pair.Value.ToString()), ValueWriteMode)}";
                    continue;
                }
                if(pair.Value is KONValue<double> doubleValue)
                {
                    output += $"\n{indent2}#{GetCase(pair.Key, KeyWriteMode)} = {GetCase(FormatValue(pair.Value.ToString()), ValueWriteMode)}";
                    continue;
                }
                if(pair.Value is KONValue<bool> boolValue)
                {
                    output += $"\n{indent2}@{GetCase(pair.Key, KeyWriteMode)} = {GetCase(FormatValue(pair.Value.ToString()), ValueWriteMode)}";
                    continue;
                }
                output += $"\n{indent2}{GetCase(pair.Key, KeyWriteMode)} = {GetCase(FormatValue(pair.Value.ToString()), ValueWriteMode)}";
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
            foreach(IKONValue item in array.Items)
            {
                output += $"\n{indent2}{GetCase(FormatValue(item.ToString()), ValueWriteMode)}";
            }
            output += $"\n{indent}]";
            return output;
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="useLineBreaks">Whether or not to </param>
        /// <param name="currentDepth"></param>
        /// <param name="isLast"></param>
        /// <returns>The JSON object representation of the node.</returns>
        public string WriteJSON(KONNode node, bool useLineBreaks = true, int currentDepth = 0, bool isLast = true)
        {
            string indent = "";
            for(int i = 0; i < currentDepth; i++)
            {
                indent += "    ";
            }
            string indent2 = indent + "    ";
            string output = currentDepth == 0 ? $"{indent}{{" : $"{indent}\"{GetCase(node.Name, NodeNameWriteMode)}\": {{";
            foreach(KeyValuePair<string, IKONValue> value in node.Values)
            {
                if(value.Value is KONValue<string>)
                    output += $"\n{indent2}\"{value.Key}\": \"{value.Value}\"";
                else
                    output += $"\n{indent2}\"{value.Key}\": {value.Value}";
                if(value.Key != node.Values.Keys.Last() || node.Arrays.Count > 0 || node.Children.Count > 0)
                {
                    output += useLineBreaks ? ",\n" : ",";
                }
            }
            foreach(KONArray array in node.Arrays)
            {
                output += this.WriteJSONArray(array, false, currentDepth + 1, node.Children.Count < 1);
            }
            for(int i = 0; i < node.Children.Count; i++)
            {
                output += "\n" + this.WriteJSON(node.Children[i], useLineBreaks, currentDepth + 1, i == node.Children.Count - 1);
            }
            output += isLast ? $"\n{indent}}}" : $"\n{indent}}},";
            //Filter out empty lines
            string[] outputLines = output.Split('\n').Where(x => x.Trim() != "").ToArray();
            output = outputLines[0];
            for(int i = 1; i < outputLines.Length; i++)
            {
                output += $"\n{outputLines[i]}";
            }
            return output;
        }
        public string WriteJSONArray(KONArray array, bool useLineBreaks = false, int currentDepth = 0, bool isLast = true)
        {
            string indent = "";
            for(int i = 0; i < currentDepth; i++)
            {
                indent += "    ";
            }
            string output = $"{indent}\"{array.Name}\": [ ";
            for(int i = 0; i < array.Count; i++)
            {
                if(array.Items[i] is KONValue<string>)
                    output += $"\"{array.Items[i]}\"";
                else
                    output += $"{array.Items[i]}";
                if(i != array.Count - 1)
                    output += ", ";
            }
            output += isLast ? " ]" : " ],";
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
        /// <summary>
        /// Returns a value suitable for use in a KON file
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string FormatValue(string input)
        {
            return input.Replace("\n", @"\n")
            .Replace("[", "\\[")
            .Replace("]", "\\]")
            .Replace("{", "\\{")
            .Replace("}", "\\}")
            .Replace("=", "\\="); 
        }
    }
}