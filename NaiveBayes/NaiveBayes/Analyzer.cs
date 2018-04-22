using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NaiveBayes
{ 
    class Analyzer
    {
        //analyzes instance for words contained in text
        public static Dictionary<string, int> Analyze(string text)
        {
            Dictionary<string, int> Words = new Dictionary<string, int>();

            string t = Number(text);
            t = Link(t);

            //seperate words
            string[] words = t.Split(' ');

            //add to dictionary
            foreach (string s in words)
            {
                if (Words.ContainsKey(s)) Words[s]++;
                else Words.Add(s, 1);
            }

            return Words;
        }

        //replace all links by <<Link>> so they can be considered
        static string Link(string text)
        {
            string pattern = @"(\swww\..*\..*\s)";
            Regex regex = new Regex(pattern);
            return regex.Replace(text, " <<Link>> ");
        }

        //replace all numbers by <<Number>> so they can be considered
        static string Number(string text)
        {
            string pattern = @"([0-9][0-9][0-9][0-9]+)";
            Regex regex = new Regex(pattern);
            return regex.Replace(text, "<<Number>>");
        }
    }
}