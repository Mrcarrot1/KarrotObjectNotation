using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace KarrotObjectNotation
{
    public class KONParser
    {
        public static KONNode Parse(string contents)
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
                KONNode output = new KONNode(line.Trim());
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
                        currentNode.Values.Add(line.Split('=')[0].Trim(), line.Split('=')[1].Trim());
                    }
                    if(line.Contains("{") && !previousLine.Contains("{") && previousLine != output.Name && !arrayReadMode)
                    {
                        KONNode newNode = new KONNode(Regex.Replace(previousLine, @"[^\w\-]", "", RegexOptions.None, TimeSpan.FromSeconds(1)), currentNode);
                        currentNode.AddChild(newNode);
                        currentNode = newNode;
                    }
                    if(line.Contains("[") && !previousLine.Contains("[") && !arrayReadMode)
                    {
                        currentArray = new KONArray(Regex.Replace(previousLine, @"[^\w\-]", "", RegexOptions.None, TimeSpan.FromSeconds(1)), currentNode);
                        currentNode.AddArray(currentArray);
                        arrayReadMode = true;
                    }
                    if(arrayReadMode && !line.Contains("]") && !line.Contains("["))
                    {
                        currentArray.Items.Add(line);
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
                return null;
            }      
        }
        public static bool TryParse(string contents, out KONNode output)
        {
            try
            {
                string[] lines = contents.Split('\n');
                string previousLine = "";
                string line = lines[0];
                int currentIndex = 0;
                //Skip any preceding comments before using a line as the name
                while(line.StartsWith("#"))
                {
                    currentIndex++;
                    line = lines[currentIndex];
                }
                //Create the KONNode object and find its name based on the current line 
                output = new KONNode(line.Trim());
                KONNode currentNode = output;
                bool arrayReadMode = false;
                KONArray currentArray = null;
                for(int i = currentIndex; i < lines.Length; i++)
                {
                    previousLine = line;
                    line = lines[i].Trim();
                    if(line.Contains("=") && !arrayReadMode)
                    {
                        currentNode.Values.Add(line.Split('=')[0].Trim(), line.Split('=')[1].Trim());
                    }
                    if(line.Contains("{") && !previousLine.Contains("{") && previousLine != output.Name && !arrayReadMode)
                    {
                        KONNode newNode = new KONNode(Regex.Replace(previousLine, @"[^\w\-]", "", RegexOptions.None, TimeSpan.FromSeconds(1)), currentNode);
                        currentNode.AddChild(newNode);
                        currentNode = newNode;
                    }
                    if(line.Contains("[") && !previousLine.Contains("[") && !arrayReadMode)
                    {
                        currentArray = new KONArray(Regex.Replace(previousLine, @"[^\w\-]", "", RegexOptions.None, TimeSpan.FromSeconds(1)), currentNode);
                        currentNode.AddArray(currentArray);
                        arrayReadMode = true;
                    }
                    if(arrayReadMode && !line.Contains("]") && !line.StartsWith("#"))
                    {
                        currentArray.Items.Add(line);
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
    }
}