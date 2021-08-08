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
using System.Text.RegularExpressions;
using System.Globalization;

namespace KarrotObjectNotation
{
    public class KONParser
    {
        /// <summary>
        /// A static instance of the KONParser class with default settings.
        /// </summary>
        public static KONParser Default = new KONParser();
        /// <summary>
        /// The way in which to read the case of the names of nodes
        /// </summary>
        public CaseReadMode NodeNameReadMode { get; set; }
        /// <summary>
        /// The way in which to read the case of keys within a node
        /// </summary>
        public CaseReadMode KeyReadMode { get; set; }
        /// <summary>
        /// The way in which to read the case of values within a node or array
        /// </summary>
        public CaseReadMode ValueReadMode { get; set; }
        /// <summary>
        /// Parses the provided string as a KON node.
        /// </summary>
        /// <param name="contents">The string representation of the KON node.</param>
        /// <returns>The KONNode object.</returns>
        public KONNode Parse(string contents)
        {
            try
            {
                Regex specialCharactersRegex = new Regex(@"[^\w\-]");
                Regex bracketsRegex = new Regex(@"[\[\]\{\}]");
                foreach(Match match in bracketsRegex.Matches(contents))
                {
                    contents = contents.Replace(match.Value, $"\n{match.Value}\n");
                }
                string[] lines = contents.Split('\n');
                string previousLine = "";
                string line = lines[0];
                int currentIndex = 0;
                //Skip any preceding comments before using a line as the name
                while(line.StartsWith("//"))
                {
                    currentIndex++;
                    line = lines[currentIndex];
                }
                //Create the KONNode object and find its name based on the current line 
                KONNode output = new KONNode(GetCase(line.Trim(), NodeNameReadMode));
                KONNode currentNode = output;
                bool arrayReadMode = false;
                KONArray currentArray = null;
                for(int i = currentIndex; i < lines.Length; i++)
                {
                    previousLine = line;
                    line = lines[i].Trim();
                    //Ignore any line that starts with //
                    if(line.StartsWith("//"))
                    {
                        line = previousLine;
                        continue;
                    }
                    //Ignore any part of the line that comes after //
                    line = line.Split("//")[0];
                    //Ignore any blank line-
                    //We filtered out any preceding or trailing whitespace already
                    //So any blank line will be an empty string
                    if(line == "")
                    {
                        line = previousLine;
                        continue;
                    }
                    if(line.Contains("=") && !arrayReadMode)
                    {
                        /*foreach(KeyValuePair<string, Type> typeMarker in TypeMarkers)
                        {
                            if(line.StartsWith(typeMarker.Key))
                            {
                                currentNode.AddValue(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<typeof(typeMarker.Value)>(typeMarker.Value.Parse(GetCase(value, ValueReadMode))))
                            }
                        }*/
                        string[] splitLine = line.Split('=');
                        string key = splitLine[0];
                        string value = "";
                        for(int j = 1; j < splitLine.Length; j++)
                        {
                            value += splitLine[j];
                        }
                        if(line.StartsWith('%'))
                        {
                            if(line.StartsWith("%^"))
                                currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<uint>(uint.Parse(GetCase(value, ValueReadMode))));
                            else
                                currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<int>(int.Parse(GetCase(value, ValueReadMode))));
                            continue;
                        }
                        if(line.StartsWith('&'))
                        {
                            if(line.StartsWith("&^"))
                                currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<ulong>(ulong.Parse(GetCase(value, ValueReadMode))));
                            else
                                currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<long>(long.Parse(GetCase(value, ValueReadMode))));
                            continue;
                        }
                        if(line.StartsWith('!'))
                        {
                            currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<float>(float.Parse(GetCase(value, ValueReadMode))));
                            continue;
                        }
                        if(line.StartsWith('#'))
                        {
                            currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<double>(double.Parse(GetCase(value, ValueReadMode))));
                            continue;
                        }
                        if(line.StartsWith('@'))
                        {
                            currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<bool>(bool.Parse(GetCase(value, ValueReadMode))));
                            continue;
                        }
                        if(line.StartsWith('$'))
                        {
                            currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<string>(GetCase(value, ValueReadMode)));
                            continue;
                        }
                        if(value.ToLower() == "null")
                        {
                            currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<object>(null));
                            continue;
                        }
                        if(int.TryParse(value, out int intResult))
                        {
                            currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<int>(intResult));
                            continue;
                        }
                        else if(uint.TryParse(value, out uint uintResult))
                        {
                            currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<uint>(uintResult));
                            continue;
                        }
                        else if(long.TryParse(value, out long longResult))
                        {
                            currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<long>(longResult));
                            continue;
                        }
                        else if(ulong.TryParse(value, out ulong ulongResult))
                        {
                            currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<ulong>(ulongResult));
                            continue;
                        }
                        else if(float.TryParse(value, out float floatResult))
                        {
                            currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<float>(floatResult));
                            continue;
                        }
                        else if(double.TryParse(value, out double doubleResult))
                        {
                            currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<double>(doubleResult));
                            continue;
                        }
                        else if(bool.TryParse(value, out bool boolResult))
                        {
                            currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<bool>(boolResult));
                            continue;
                        }
                        currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<string>(GetCase(value, ValueReadMode)));
                    }
                    if(line.Contains("{") && !previousLine.Contains("{") && previousLine != output.Name && !arrayReadMode)
                    {
                        KONNode newNode = new KONNode(GetCase(Regex.Replace(previousLine, @"[^\w\-]", "", RegexOptions.None, TimeSpan.FromSeconds(1)), NodeNameReadMode), currentNode);
                        currentNode.AddChild(newNode);
                        currentNode = newNode;
                    }
                    if(line.Contains("[") && !previousLine.Contains("[") && !arrayReadMode)
                    {
                        currentArray = new KONArray(GetCase(Regex.Replace(previousLine, @"[^\w\-]", "", RegexOptions.None, TimeSpan.FromSeconds(1)), NodeNameReadMode), currentNode);
                        currentNode.AddArray(currentArray);
                        arrayReadMode = true;
                    }
                    if(arrayReadMode && !line.Contains("]") && !line.Contains("["))
                    {
                        if(int.TryParse(line, out int intResult))
                        {
                            currentArray.AddItem(new KONValue<int>(intResult));
                            continue;
                        }
                        else if(uint.TryParse(line, out uint uintResult))
                        {
                            currentArray.AddItem(new KONValue<uint>(uintResult));
                            continue;
                        }
                        else if(long.TryParse(line, out long longResult))
                        {
                            currentArray.AddItem(new KONValue<long>(longResult));
                            continue;
                        }
                        else if(ulong.TryParse(line, out ulong ulongResult))
                        {
                            currentArray.AddItem(new KONValue<ulong>(ulongResult));
                            continue;
                        }
                        else if(float.TryParse(line, out float floatResult))
                        {
                            currentArray.AddItem(new KONValue<float>(floatResult));
                            continue;
                        }
                        else if(double.TryParse(line, out double doubleResult))
                        {
                            currentArray.AddItem(new KONValue<double>(doubleResult));
                            continue;
                        }
                        else if(bool.TryParse(line, out bool boolResult))
                        {
                            currentArray.AddItem(new KONValue<bool>(boolResult));
                            continue;
                        }
                        if(line.ToLower() == "null")
                        {
                            currentArray.AddItem(new KONValue<object>(null));
                            continue;
                        }
                        currentArray.AddItem(GetCase(line, ValueReadMode));
                    }
                    if(Regex.IsMatch(line, "(?<!\\)]") && arrayReadMode)
                    {
                        arrayReadMode = false;
                    }
                    if(Regex.IsMatch(line, @"(?<!\\)}"))
                    {
                        currentNode = currentNode.Parent;
                    }
                }
                return output;
            }
            catch
            {
                throw new FormatException();
            }      
        }
        /// <summary>
        /// Attempts to parse the given string as a KON node.
        /// </summary>
        /// <param name="contents">The string representation of the given node.</param>
        /// <param name="output">The result if the operation was successful.</param>
        /// <returns>True if the string represents a valid KON node, false if it does not.</returns>
        public bool TryParse(string contents, out KONNode output)
        {
            try
            {
                output = this.Parse(contents);
                return true;
            }
            catch
            {
                output = null;
                return false;
            }
        }
        /// <summary>
        /// Parses a JSON object into a KON node.
        /// </summary>
        /// <param name="input">The JSON object string.</param>
        /// <returns>The KON representation of the JSON object.</returns>
        public KONNode ParseJSON(string input)
        {
            try
            {
                KONNode output = new KONNode("JSON_OBJECT");
                //Convert JSON syntax to KON-
                //This includes adding line breaks as JSON does not require them,
                //But KON does.
                string convertedString = input
                    .Replace("{", "\n{\n")
                    .Replace("}", "\n}\n");
                Regex commasRegex = new Regex(@"(,)(?=(?:[^""]|""[^""]*"")*$)");
                Regex bracketsRegex = new Regex(@"[\[\]\{\}]");
                Regex quotesRegex = new Regex(@"(?<!\\)""");
                Regex specialCharactersRegex = new Regex(@"[^\w\-]");
                input = $"JSON_OBJECT\n{input}";
                input = commasRegex.Replace(input, "\n");
                foreach(Match match in bracketsRegex.Matches(input))
                {
                    input = input.Replace(match.Value, $"\n{match.Value}\n");
                }
                //convertedString = Regex.Replace(convertedString, "/(,)(?=(?:[^\"]|\"[^\"]*\")*$)/m", "\n");

                string[] lines = input.Split('\n').Where(x => x.Trim() != "").ToArray();
                string previousLine = "";
                string line = lines[0];
                int currentIndex = 0;
                //Skip any preceding comments before using a line as the name
                while(line.StartsWith("//"))
                {
                    currentIndex++;
                    line = lines[currentIndex];
                }
                //Create the KONNode object and find its name based on the current line 
                KONNode currentNode = output;
                bool arrayReadMode = false;
                KONArray currentArray = null;
                for(int i = currentIndex; i < lines.Length; i++)
                {
                    previousLine = line;
                    line = lines[i].Trim();
                    //Ignore any line that starts with //
                    if(line.StartsWith("//"))
                    {
                        line = previousLine;
                        continue;
                    }
                    //Ignore any part of the line that comes after //
                    line = line.Split("//")[0];
                    //Ignore any blank line-
                    //We filtered out any preceding or trailing whitespace already
                    //So any blank line will be an empty string
                    if(line == "")
                    {
                        line = previousLine;
                        continue;
                    }
                    if(Regex.IsMatch(line, @"(?<!\\):") && !arrayReadMode)
                    {
                        /*foreach(KeyValuePair<string, Type> typeMarker in TypeMarkers)
                        {
                            if(line.StartsWith(typeMarker.Key))
                            {
                                currentNode.AddValue(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<typeof(typeMarker.Value)>(typeMarker.Value.Parse(GetCase(value, ValueReadMode))))
                            }
                        }*/
                        string[] splitLine = line.Split(':');
                        string key = splitLine[0].Trim();
                        string value = "";
                        for(int j = 1; j < splitLine.Length; j++)
                        {
                            value += splitLine[j];
                        }
                        value = value.Trim();
                        //Skip anything where the value is empty- it's probably the beginning of an object(node) or array,
                        //and we'll come back to it on the next pass.
                        if(value == "") continue;
                        if(int.TryParse(value, out int intResult))
                        {
                            currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<int>(intResult));
                            continue;
                        }
                        else if(uint.TryParse(value, out uint uintResult))
                        {
                            currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<uint>(uintResult));
                            continue;
                        }
                        else if(long.TryParse(value, out long longResult))
                        {
                            currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<long>(longResult));
                            continue;
                        }
                        else if(ulong.TryParse(value, out ulong ulongResult))
                        {
                            currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<ulong>(ulongResult));
                            continue;
                        }
                        else if(float.TryParse(value, out float floatResult))
                        {
                            currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<float>(floatResult));
                            continue;
                        }
                        else if(double.TryParse(value, out double doubleResult))
                        {
                            currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<double>(doubleResult));
                            continue;
                        }
                        else if(bool.TryParse(value, out bool boolResult))
                        {
                            currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<bool>(boolResult));
                            continue;
                        }
                        if(value.ToLower() == "null")
                        {
                            currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), KeyReadMode), new KONValue<object>(null));
                            continue;
                        }
                        currentNode.Values.Add(GetCase(Regex.Replace(key, @"[^\w\-]", "", RegexOptions.None, TimeSpan.FromSeconds(1)), KeyReadMode), new KONValue<string>(GetCase(quotesRegex.Replace(value, ""), ValueReadMode)));
                    }
                    if(Regex.IsMatch(line, @"(?<!\\){") && !Regex.IsMatch(previousLine, @"(?<!\\){") && specialCharactersRegex.Replace(previousLine, "") != output.Name && !arrayReadMode)
                    {
                        KONNode newNode = new KONNode(GetCase(Regex.Replace(previousLine, @"[^\w\-]", "", RegexOptions.None, TimeSpan.FromSeconds(1)), NodeNameReadMode), currentNode);
                        currentNode.AddChild(newNode);
                        currentNode = newNode;
                    }
                    if(Regex.IsMatch(line, @"(?<!\\)[") && Regex.IsMatch(line, @"(?<!\\){") && !arrayReadMode)
                    {
                        currentArray = new KONArray(GetCase(Regex.Replace(previousLine, @"[^\w\-]", "", RegexOptions.None, TimeSpan.FromSeconds(1)), NodeNameReadMode), currentNode);
                        currentNode.AddArray(currentArray);
                        arrayReadMode = true;
                    }
                    if(arrayReadMode && !Regex.IsMatch(line, @"(?<!\\)[") && !Regex.IsMatch(line, @"(?<!\\)]"))
                    {
                        if(int.TryParse(line, out int intResult))
                        {
                            currentArray.AddItem(new KONValue<int>(intResult));
                            continue;
                        }
                        else if(uint.TryParse(line, out uint uintResult))
                        {
                            currentArray.AddItem(new KONValue<uint>(uintResult));
                            continue;
                        }
                        else if(long.TryParse(line, out long longResult))
                        {
                            currentArray.AddItem(new KONValue<long>(longResult));
                            continue;
                        }
                        else if(ulong.TryParse(line, out ulong ulongResult))
                        {
                            currentArray.AddItem(new KONValue<ulong>(ulongResult));
                            continue;
                        }
                        else if(float.TryParse(line, out float floatResult))
                        {
                            currentArray.AddItem(new KONValue<float>(floatResult));
                            continue;
                        }
                        else if(double.TryParse(line, out double doubleResult))
                        {
                            currentArray.AddItem(new KONValue<double>(doubleResult));
                            continue;
                        }
                        else if(bool.TryParse(line, out bool boolResult))
                        {
                            currentArray.AddItem(new KONValue<bool>(boolResult));
                            continue;
                        }
                        if(line.ToLower() == "null")
                        {
                            currentArray.AddItem(new KONValue<object>(null));
                            continue;
                        }
                        currentArray.AddItem(new KONValue<string>(GetCase(quotesRegex.Replace(line, ""), ValueReadMode)));
                    }
                    if(Regex.IsMatch(line, @"(?<!\\)]") && arrayReadMode)
                    {
                        arrayReadMode = false;
                    }
                    if(Regex.IsMatch(line, @"(?<!\\)}"))
                    {
                        currentNode = currentNode.Parent;
                    }
                }
                return output;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
                //throw new FormatException();
            }
        }
        public bool TryParseJSON(string input, out KONNode output)
        {
            try
            {
                output = this.ParseJSON(input);
                return true;
            }
            catch
            {
                output = null;
                return false;
            }
        }
        private static string GetCase(string str, CaseReadMode crm)
        {
            if(crm == CaseReadMode.ToUpper) return str.ToUpper();
            if(crm == CaseReadMode.ToLower) return str.ToLower();
            if(crm == CaseReadMode.ToTitle) return new CultureInfo("en-US",false).TextInfo.ToTitleCase(str);
            return str; //At the end, the only one left is keep original
        }
        public enum CaseReadMode
        {
            KeepOriginal,
            ToUpper,
            ToLower,
            ToTitle
        }
        public KONParser(CaseReadMode nodeNameReadMode = CaseReadMode.KeepOriginal, CaseReadMode keyReadMode = CaseReadMode.KeepOriginal, CaseReadMode valueReadMode = CaseReadMode.KeepOriginal)
        {
            NodeNameReadMode = nodeNameReadMode;
            KeyReadMode = keyReadMode;
            ValueReadMode = valueReadMode;
        }
        /// <summary>
        /// Takes a string formatted for a KON value and returns its normal version
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string FormatValue(string input)
        {
            return input.Replace(@"\n", "\n") //Replace the literal string \n with a line break- this allows for multi-line values in KON
            .Replace("\\[", "[") //We also escape the miscellaneous KON syntax characters- this allows
            .Replace("\\]", "]") //for values to contain these characters if they are properly escaped.
            .Replace("\\{", "{") //This technically allows you to nest KON files within each other, but please don't.
            .Replace("\\}", "}")
            .Replace("\\:", ":"); 
        }
        private readonly Dictionary<string, Type> TypeMarkers = new Dictionary<string, Type>
        {
            ["%"] = typeof(int),
            ["^%"] = typeof(uint),
            ["&"] = typeof(long),
            ["^&"] = typeof(ulong),
            ["!"] = typeof(float),
            ["#"] = typeof(double),
            ["$"] = typeof(string),
            ["@"] = typeof(bool)
        };
    }
}