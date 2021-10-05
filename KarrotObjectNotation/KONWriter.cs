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
using System.Numerics;

namespace KarrotObjectNotation
{
    public class KONWriter
    {
        /// <summary>
        /// A KONWriter object with default configuration.
        /// </summary>
        /// <returns></returns>
        public static KONWriter Default = new KONWriter(KONWriterOptions.Default);
        /// <summary>
        /// The configuration of this KONWriter instance.
        /// </summary>
        /// <value></value>
        public KONWriterOptions Options { get; }
        /// <summary>
        /// Takes a KONNode and returns its KON string representation.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="currentDepth"></param>
        /// <returns></returns>
        public string Write(KONNode node, int currentDepth = 0)
        {
            //It feels wrong to do it like this but this way it's nice and neatly indented
            string indent = "";
            for(int i = 0; i < currentDepth; i++)
            {
                indent += "    ";
            }
            string indent2 = indent + "    ";
            string output = $"{indent}{GetCase(node.Name, Options.NodeNameWriteMode)}\n{indent}{{";
            foreach(KeyValuePair<string, object> pair in node.Values)
            {
                if(pair.Value == null)
                    output += $"\n{indent2}{GetCase(pair.Key, Options.KeyWriteMode)} = null";
                else
                    output += $"\n{indent2}{GetTypeMarker(pair.Value.GetType())}{GetCase(pair.Key, Options.KeyWriteMode)} = {GetCase(FormatValue(pair.Value.ToString()), Options.ValueWriteMode)}";
                if(Options.Inline)
                    output += ";";
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
            string[] outputLines = output.Split('\n').Where(x => x.Trim() != "").ToArray();
            output = outputLines[0];
            for(int i = 1; i < outputLines.Length; i++)
            {
                output += Options.Inline ? $" {outputLines[i].Trim()}" : $"\n{outputLines[i]}";
            }
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
            string output = $"{indent}{GetCase(array.Name, Options.NodeNameWriteMode)}\n{indent}[";
            foreach(object item in array.Items)
            {
                if(item == null)
                    output += $"\n{indent2}null";
                else
                    output += $"\n{indent2}{GetTypeMarker(item.GetType())}{GetCase(FormatValue(item.ToString()), Options.ValueWriteMode)}";
                if(Options.ArrayInline)
                    output += ";";
            }
            output += $"\n{indent}]";
            string[] outputLines = output.Split('\n').Where(x => x.Trim() != "").ToArray();
            output = outputLines[0];
            for(int i = 1; i < outputLines.Length; i++)
            {
                output += Options.ArrayInline ? $" {outputLines[i].Trim()}" : $"\n{outputLines[i]}";
            }
            return output;
            
        }
        /// <summary>
        /// Takes a KON node and returns its JSON string representation.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="currentDepth"></param>
        /// <param name="isLast"></param>
        /// <returns>The JSON object representation of the node.</returns>
        public string WriteJSON(KONNode node, int currentDepth = 0, bool isLast = true)
        {
            string indent = "";
            for(int i = 0; i < currentDepth; i++)
            {
                indent += "    ";
            }
            string indent2 = indent + "    ";
            string output = currentDepth == 0 ? $"{indent}{{" : $"{indent}\"{GetCase(node.Name, Options.NodeNameWriteMode)}\": {{";
            foreach(KeyValuePair<string, object> value in node.Values)
            {
                if(value.Value is string)
                    output += $"\n{indent2}\"{value.Key}\": \"{value.Value}\"";
                else if(value.Value == null)
                    output += $"\n{indent2}\"{value.Key}\": null";
                else
                    output += $"\n{indent2}\"{value.Key}\": {value.Value.ToString().ToLower()}";
                if(value.Key != node.Values.Keys.Last() || node.Arrays.Count > 0 || node.Children.Count > 0)
                {
                    output += ",\n";
                }
            }
            foreach(KONArray array in node.Arrays)
            {
                output += this.WriteJSONArray(array, currentDepth + 1, node.Children.Count < 1 || !array.Equals(node.Arrays.Last()));
            }
            for(int i = 0; i < node.Children.Count; i++)
            {
                output += "\n" + this.WriteJSON(node.Children[i], currentDepth + 1, i == node.Children.Count - 1);
            }
            output += isLast ? $"\n{indent}}}" : $"\n{indent}}},";
            //Filter out empty lines-
            //This step also formats the JSON as inline or not
            string[] outputLines = output.Split('\n').Where(x => x.Trim() != "").ToArray();
            output = outputLines[0].Trim();
            for(int i = 1; i < outputLines.Length; i++)
            {
                output += Options.Inline ? $" {outputLines[i].Trim()}" : $"\n{outputLines[i]}";
            }
            return output;
        }
        /// <summary>
        /// Takes a KONArray and returns its JSON string representation.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="currentDepth"></param>
        /// <param name="isLast"></param>
        /// <returns></returns>
        public string WriteJSONArray(KONArray array, int currentDepth = 0, bool isLast = true)
        {
            string indent = "";
            for(int i = 0; i < currentDepth; i++)
            {
                indent += "    ";
            }
            string output = $"{indent}\"{array.Name}\": [";
            for(int i = 0; i < array.Count; i++)
            {
                if(array.Items[i] is string)
                    output += $"\n\"{array.Items[i]}\"";
                else if(array[i] == null)
                    output += "\nnull";
                else
                    output += $"\n{array.Items[i].ToString().ToLower()}";
                if(i != array.Count - 1)
                    output += ",\n";
            }
            output += isLast ? $"\n{indent}]" : $"\n{indent}],";
            string[] outputLines = output.Split('\n').Where(x => x.Trim() != "").ToArray();
            output = outputLines[0];
            for(int i = 1; i < outputLines.Length; i++)
            {
                output += Options.ArrayInline ? $" {outputLines[i].Trim()}" : $"\n{outputLines[i]}";
            }
            return output;
        }
        private string GetCase(string str, KONWriterOptions.CaseWriteMode cwm)
        {
            if(cwm == KONWriterOptions.CaseWriteMode.ToUpper) return str.ToUpper();
            if(cwm == KONWriterOptions.CaseWriteMode.ToLower) return str.ToLower();
            if(cwm == KONWriterOptions.CaseWriteMode.ToTitle) return new CultureInfo("en-US",false).TextInfo.ToTitleCase(str);
            else return str;
        }
        public KONWriter(KONWriterOptions options)
        {
            Options = options;
        }
        
        /// <summary>
        /// Returns a value suitable for use in a KON file
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string FormatValue(string input)
        {
            string[] reservedCharacters = 
            {
                "[", "]", "{", "}", "=", ";", "//", "%", "&", "~", "!", "#", "@", "$"
            };
            for(int i = 0; i < reservedCharacters.Length; i++)
            {
                input = input.Replace(reservedCharacters[i], $"\\{reservedCharacters[i]}").Replace("\n", @"\n");
            }
            return input;
        }
        private static string GetTypeMarker(Type type)
        {
            if(TypeMarkers.ContainsKey(type)) return TypeMarkers[type];
            return "";
        }
        private static readonly Dictionary<Type, string> TypeMarkers = new Dictionary<Type, string>
        {
            [typeof(int)] = "%",
            [typeof(uint)] = "%^",
            [typeof(long)] = "&",
            [typeof(ulong)] = "&^",
            [typeof(BigInteger)] = "&*",
            [typeof(short)] = "~",
            [typeof(ushort)] = "~^",
            [typeof(float)] = "!",
            [typeof(double)] = "#",
            [typeof(bool)] = "@",
            [typeof(string)] = "$",           
        };
    }
    
    /// <summary>
    /// Configuration for a KONWriter.
    /// </summary>
    public class KONWriterOptions
    {
        /// <summary>
        /// Represents the default configuration options for a KONWriter.
        /// </summary>
        /// <returns></returns>
        public static KONWriterOptions Default = new KONWriterOptions();
        /// <summary>
        /// The case the writer will use when writing node and array names.
        /// </summary>
        /// <value></value>
        public CaseWriteMode NodeNameWriteMode { get; set; }
        /// <summary>
        /// The case the writer will use when writing keys.
        /// </summary>
        /// <value></value>
        public CaseWriteMode KeyWriteMode { get; set; }
        /// <summary>
        /// The case the writer will use when writing values.
        /// </summary>
        /// <value></value>
        public CaseWriteMode ValueWriteMode { get; set; }

        /// <summary>
        /// Whether or not to write JSON objects in a single line.
        /// </summary>
        public bool Inline = false;
        /// <summary>
        /// Whether or not to write JSON arrays in a single line.
        /// </summary>
        public bool ArrayInline = true;

        public KONWriterOptions(CaseWriteMode nodeNameWriteMode = CaseWriteMode.KeepOriginal, CaseWriteMode keyWriteMode = CaseWriteMode.KeepOriginal, CaseWriteMode valueWriteMode = CaseWriteMode.KeepOriginal, bool inline = false, bool arrayInline = true)
        {
            NodeNameWriteMode = nodeNameWriteMode;
            KeyWriteMode = keyWriteMode;
            ValueWriteMode = valueWriteMode;

            Inline = inline;
            ArrayInline = arrayInline;
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