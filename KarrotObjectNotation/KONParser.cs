using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace KarrotObjectNotation
{
    public class KONParser
    {
        public static KONParser Default = new KONParser();
        public CaseReadMode NodeNameReadMode { get; set; }
        public CaseReadMode KeyReadMode { get; set; }
        public CaseReadMode ValueReadMode { get; set; }
        public KONNode Parse(string contents)
        {
            try
            {
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
                    if(line.Contains("=") && !arrayReadMode)
                    {
                        currentNode.Values.Add(GetCase(line.Split('=')[0].Trim(), KeyReadMode), GetCase(line.Split('=')[1].Trim(), ValueReadMode));
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
                        currentArray.Items.Add(GetCase(line, ValueReadMode));
                    }
                    if(line.Contains("]") && arrayReadMode)
                    {
                        arrayReadMode = false;
                    }
                    if(line.Contains("}"))
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
        public bool TryParse(string contents, out KONNode output)
        {
            try
            {
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
                output = new KONNode(GetCase(line.Trim(), NodeNameReadMode));
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
                    if(line.Contains("=") && !arrayReadMode)
                    {
                        currentNode.Values.Add(GetCase(line.Split('=')[0].Trim(), KeyReadMode), GetCase(FormatValue(line.Split('=')[1].Trim()), ValueReadMode));
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
                        currentArray.Items.Add(GetCase(FormatValue(line), ValueReadMode));
                    }
                    if(line.Contains("]") && arrayReadMode)
                    {
                        arrayReadMode = false;
                    }
                    if(line.Contains("}"))
                    {
                        currentNode = currentNode.Parent;
                    }
                }
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
            return input.Replace(@"\n", "\n"); //Replace the literal string \n with a line break- this allows for multi-line values in KON
        }
    }
}