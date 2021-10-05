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
using System.Numerics;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Text.Json;

namespace KarrotObjectNotation
{
    public class KONParser
    {
        /// <summary>
        /// A static instance of the KONParser class with default settings.
        /// </summary>
        public static KONParser Default = new KONParser(KONParserOptions.Default);
        public KONParserOptions Options { get; set; }
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
                Regex bracketsRegex = new Regex(@"(?<!\\)[\[\]\{\}]");
                Regex startingTypeCharactersRegex = new Regex(@"^[%\^#!@\$&~*]{1,2}");
                foreach(Match match in bracketsRegex.Matches(contents))
                {
                    contents = contents.Replace(match.Value, $"\n{match.Value}\n");
                }
                contents = Regex.Replace(contents, @"(?<!\\);", "\n");
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
                KONNode output = new KONNode(GetCase(line.Trim(), Options.NodeNameReadMode));
                KONNode currentNode = output;
                bool arrayReadMode = false;
                KONArray currentArray = null;
                for(int i = currentIndex; i < lines.Length; i++)
                {
                    try
                    {
                        previousLine = line;
                        line = lines[i].Trim();
                        //Ignore any line that starts with //
                        if(Regex.IsMatch(line, @"^//"))
                        {
                            line = previousLine;
                            continue;
                        }
                        //Ignore any part of the line that comes after //
                        line = Regex.Split(line, @"(?<![:\\])//")[0];
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
                                    currentNode.AddValue(GetCase(specialCharactersRegex.Replace(key, ""), Options.KeyReadMode), new KONValue<typeof(typeMarker.Value)>(typeMarker.Value.Parse(GetCase(value, Options.ValueReadMode))))
                                }
                            }*/
                            string[] splitLine = line.Split('=');
                            string key = splitLine[0];
                            string value = "";
                            for(int j = 1; j < splitLine.Length; j++)
                            {
                                value += splitLine[j];
                            }
                            value = value.Trim();
                            if(line.StartsWith('%'))
                            {
                                if(line.StartsWith("%^"))
                                    currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), Options.KeyReadMode), uint.Parse(GetCase(value, Options.ValueReadMode)));
                                else
                                    currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), Options.KeyReadMode), int.Parse(GetCase(value, Options.ValueReadMode)));
                                continue;
                            }
                            if(line.StartsWith('&'))
                            {
                                if(line.StartsWith("&^"))
                                    currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), Options.KeyReadMode), ulong.Parse(GetCase(value, Options.ValueReadMode)));
                                if(line.StartsWith("&*"))
                                    currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), Options.KeyReadMode), BigInteger.Parse(GetCase(value, Options.ValueReadMode)));
                                else
                                    currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), Options.KeyReadMode), long.Parse(GetCase(value, Options.ValueReadMode)));
                                continue;
                            }
                            if(line.StartsWith('~'))
                            {
                                if(line.StartsWith("~^"))
                                    currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), Options.KeyReadMode), ushort.Parse(GetCase(value, Options.ValueReadMode)));
                                else
                                    currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), Options.KeyReadMode), short.Parse(GetCase(value, Options.ValueReadMode)));
                                continue;
                            }
                            if(line.StartsWith('!'))
                            {
                                currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), Options.KeyReadMode),float.Parse(GetCase(value, Options.ValueReadMode)));
                                continue;
                            }
                            if(line.StartsWith('#'))
                            {
                                currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), Options.KeyReadMode), double.Parse(GetCase(value, Options.ValueReadMode)));
                                continue;
                            }
                            if(line.StartsWith('@'))
                            {
                                currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), Options.KeyReadMode), bool.Parse(GetCase(value, Options.ValueReadMode)));
                                continue;
                            }
                            if(line.StartsWith('$'))
                            {
                                currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), Options.KeyReadMode), GetCase(value, Options.ValueReadMode));
                                continue;
                            }
                            if(value.ToLower() == "null")
                            {
                                currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), Options.KeyReadMode), null);
                                continue;
                            }
                            currentNode.Values.Add(GetCase(specialCharactersRegex.Replace(key, ""), Options.KeyReadMode), GetValue(value));
                        }
                        if(Regex.IsMatch(line, @"(?<!\\){") && !Regex.IsMatch(previousLine, @"(?<!\\){") && previousLine != output.Name && !arrayReadMode)
                        {
                            KONNode newNode = new KONNode(GetCase(Regex.Replace(previousLine, @"[^\w\-]", "", RegexOptions.None, TimeSpan.FromSeconds(1)), Options.NodeNameReadMode), currentNode);
                            currentNode.AddChild(newNode);
                            currentNode = newNode;
                        }
                        if(Regex.IsMatch(line, @"(?<!\\)\[") && !Regex.IsMatch(previousLine, @"(?<!\\)\[") && !arrayReadMode)
                        {
                            currentArray = new KONArray(GetCase(Regex.Replace(previousLine, @"[^\w\-]", "", RegexOptions.None, TimeSpan.FromSeconds(1)), Options.NodeNameReadMode), currentNode);
                            currentNode.AddArray(currentArray);
                            arrayReadMode = true;
                        }
                        if(arrayReadMode && !Regex.IsMatch(line, @"(?<!\\)\]") && !Regex.IsMatch(line, @"(?<!\\)\["))
                        {
                            if(line.StartsWith('%'))
                            {
                                if(line.StartsWith("%^"))
                                    currentArray.AddItem(uint.Parse(startingTypeCharactersRegex.Replace(line, "")));
                                else
                                    currentArray.AddItem(int.Parse(startingTypeCharactersRegex.Replace(line, "")));
                                continue;
                            }
                            if(line.StartsWith('&'))
                            {
                                if(line.StartsWith("&^"))
                                    currentArray.AddItem(ulong.Parse(startingTypeCharactersRegex.Replace(line, "")));
                                if(line.StartsWith("&*"))
                                    currentArray.AddItem(BigInteger.Parse(startingTypeCharactersRegex.Replace(line, "")));
                                else
                                    currentArray.AddItem(long.Parse(startingTypeCharactersRegex.Replace(line, "")));
                                continue;
                            }
                            if(line.StartsWith('~'))
                            {
                                if(line.StartsWith("~^"))
                                    currentArray.AddItem(ushort.Parse(startingTypeCharactersRegex.Replace(line, "")));
                                else
                                    currentArray.AddItem(short.Parse(startingTypeCharactersRegex.Replace(line, "")));
                                continue;
                            }
                            if(line.StartsWith('!'))
                            {
                                currentArray.AddItem(float.Parse(startingTypeCharactersRegex.Replace(line, "")));
                                continue;
                            }
                            if(line.StartsWith('#'))
                            {
                                currentArray.AddItem(double.Parse(startingTypeCharactersRegex.Replace(line, "")));
                                continue;
                            }
                            if(line.StartsWith('@'))
                            {
                                currentArray.AddItem(bool.Parse(startingTypeCharactersRegex.Replace(line, "")));
                                continue;
                            }
                            if(line.StartsWith('$'))
                            {
                                currentArray.AddItem(startingTypeCharactersRegex.Replace(line, ""));
                                continue;
                            }
                            currentArray.AddItem(GetValue(line));
                        }
                        if(Regex.IsMatch(line, @"(?<!\\)\]") && arrayReadMode)
                        {
                            arrayReadMode = false;
                        }
                        if(Regex.IsMatch(line, @"(?<!\\)}"))
                        {
                            currentNode = currentNode.Parent;
                        }
                    }
                    catch
                    {
                        throw new FormatException($"KON Syntax Error at '{line}', line {i}.");
                    }
                }
                return output;
            }
            catch(FormatException)
            {
                throw;
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
                //But KON does- though it now has inline syntax,
                //The parser always reads line-by-line.
                Regex commasRegex = new Regex(@"(,)(?=(?:[^""]|""[^""]*"")*$)");
                Regex bracketsRegex = new Regex(@"(?<!\\)[\[\]\{\}]");
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
                    try
                    {
                        previousLine = line;
                        line = lines[i].Trim();
                        //Ignore any line that starts with //
                        if(line.StartsWith("//"))
                        {
                            line = previousLine;
                            continue;
                        }
                        //Ignore any part of the line that comes after //, unless the // is preceded by :
                        //Which probably means it's  a URL.
                        line = Regex.Split(line, @"(?<![:\\])//")[0];
                        //Ignore any blank line-
                        //We filtered out any preceding or trailing whitespace already
                        //So any blank line will be an empty string
                        if(line == "")
                        {
                            line = previousLine;
                            continue;
                        }
                        if(Regex.IsMatch(line, @"(?<!\\):.") && !arrayReadMode)
                        {
                            /*foreach(KeyValuePair<string, Type> typeMarker in TypeMarkers)
                            {
                                if(line.StartsWith(typeMarker.Key))
                                {
                                    currentNode.AddValue(GetCase(specialCharactersRegex.Replace(key, ""), Options.KeyReadMode), new KONValue<typeof(typeMarker.Value)>(typeMarker.Value.Parse(GetCase(value, Options.ValueReadMode))))
                                }
                            }*/
                            int firstColonIndex = line.IndexOf(':');
                            string key = line.Substring(0, firstColonIndex);
                            string value = line.Substring(firstColonIndex + 1);
                            value = value.Trim();
                            //Skip anything where the value is empty- it's probably the beginning of an object(node) or array,
                            //and we'll come back to it on the next pass.
                            if(value == "") continue;
                            currentNode.Values.Add(GetCase(Regex.Replace(key, @"[^\w\-]", "", RegexOptions.None, TimeSpan.FromSeconds(1)), Options.KeyReadMode), GetValue(value, true));
                        }
                        if(Regex.IsMatch(line, @"(?<!\\){") && !Regex.IsMatch(previousLine, @"(?<!\\){") && specialCharactersRegex.Replace(previousLine, "") != output.Name && !arrayReadMode)
                        {
                            KONNode newNode = new KONNode(GetCase(Regex.Replace(previousLine, @"[^\w\-]", "", RegexOptions.None, TimeSpan.FromSeconds(1)), Options.NodeNameReadMode), currentNode);
                            currentNode.AddChild(newNode);
                            currentNode = newNode;
                        }
                        if(Regex.IsMatch(line, @"(?<!\\)\[") && !arrayReadMode)
                        {
                            currentArray = new KONArray(GetCase(Regex.Replace(previousLine, @"[^\w\-]", "", RegexOptions.None, TimeSpan.FromSeconds(1)), Options.NodeNameReadMode), currentNode);
                            currentNode.AddArray(currentArray);
                            arrayReadMode = true;
                        }
                        if(arrayReadMode && !Regex.IsMatch(line, @"(?<!\\)\[") && !Regex.IsMatch(line, @"(?<!\\)\]"))
                        {
                            currentArray.AddItem(GetValue(line, true));
                        }
                        if(Regex.IsMatch(line, @"(?<!\\)\]") && arrayReadMode)
                        {
                            arrayReadMode = false;
                        }
                        if(Regex.IsMatch(line, @"(?<!\\)}"))
                        {
                            currentNode = currentNode.Parent;
                        }
                    }
                    catch
                    {
                        throw new FormatException($"JSON syntax error at '{line}'.");
                    }
                }
                return output;
            }
            catch(FormatException)
            {
                throw;
            }
            catch
            {
                throw new FormatException();
            }
        }
        /// <summary>
        /// Takes a string, determines if it is valid JSON, and returns its KONNode equivalent if it is.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Takes a value's string representation, determines the correct type for the value, and returns the appropriate object. Only used in implicit typing.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public object GetValue(string input, bool isJSON = false)
        {
            Regex quotesRegex = new Regex(@"(?<!\\)""");
            if(Options.AllowImplicitTyping || isJSON)
            {
                if(int.TryParse(input, out int intResult))
                {
                    return intResult;
                }
                if(long.TryParse(input, out long longResult))
                {
                    return longResult;
                }
                if(BigInteger.TryParse(input, out BigInteger bigIntegerResult))
                {
                    return bigIntegerResult;
                }
                if(float.TryParse(input, out float floatResult))
                {
                    return floatResult;
                }
                if(double.TryParse(input, out double doubleResult))
                {
                    return doubleResult;
                }
                if(input.ToLower() == "null")
                {
                    return null;
                }
                if(bool.TryParse(input, out bool boolResult))
                {
                    return boolResult;
                }
                //We check for the unsigned types last just to make sure that, for example,
                //Int32.MaxValue + 1 doesn't get read as uint
                if(uint.TryParse(input, out uint uintResult))
                {
                    return uintResult;
                }
                if(ulong.TryParse(input, out ulong ulongResult))
                {
                    return ulongResult;
                }
            }
            return isJSON ? GetCase(quotesRegex.Replace(input, ""), Options.ValueReadMode) : GetCase(input, Options.ValueReadMode);
        }
        private static string GetCase(string str, KONParserOptions.CaseReadMode crm)
        {
            if(crm == KONParserOptions.CaseReadMode.ToUpper) return str.ToUpper();
            if(crm == KONParserOptions.CaseReadMode.ToLower) return str.ToLower();
            if(crm == KONParserOptions.CaseReadMode.ToTitle) return new CultureInfo("en-US",false).TextInfo.ToTitleCase(str);
            return str; //At the end, the only one left is keep original
        }
        
        public KONParser(KONParserOptions options)
        {
            Options = options;
        }
        /// <summary>
        /// Takes a string formatted for a KON value and returns its normal version
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
                input = input.Replace($"\\{reservedCharacters[i]}", reservedCharacters[i]).Replace(@"\n", "\n");
            }
            return input;
        }
    }
    /// <summary>
    /// Configuration for a KON parser.
    /// </summary>
    public class KONParserOptions
    {
        /// <summary>
        /// Represents the default configuration options for a KONParser.
        /// </summary>
        /// <returns></returns>
        public static KONParserOptions Default = new KONParserOptions();

        /// <summary>
        /// The case the parser will use when reading node and array names.
        /// </summary>
        /// <value></value>
        public CaseReadMode NodeNameReadMode { get; set; }
        /// <summary>
        /// The case the parser will use when reading key names.
        /// </summary>
        /// <value></value>
        public CaseReadMode KeyReadMode { get; set; }
        /// <summary>
        /// The case the parser will use when reading value names.
        /// </summary>
        /// <value></value>
        public CaseReadMode ValueReadMode { get; set; }

        /// <summary>
        /// Whether or not to allow implicit typing. If false, any value of an unspecified type will be read as a string.
        /// </summary>
        /// <value></value>
        public bool AllowImplicitTyping { get; set; }

        public KONParserOptions(CaseReadMode nodeNameReadMode = CaseReadMode.KeepOriginal, CaseReadMode keyReadMode = CaseReadMode.KeepOriginal, CaseReadMode valueReadMode = CaseReadMode.KeepOriginal, bool allowImplicitTyping = true)
        {
            NodeNameReadMode = nodeNameReadMode;
            KeyReadMode = keyReadMode;
            ValueReadMode = valueReadMode;
            AllowImplicitTyping = allowImplicitTyping;
        }

        public enum CaseReadMode
        {
            KeepOriginal,
            ToUpper,
            ToLower,
            ToTitle
        }
    }
}