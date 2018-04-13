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
        //analyzes the text an returns a dictionary with ascending, ongoing numbers of attributes and
        //correspoding bool indicating if attribute is true or false
        public static Dictionary<string, int> Analyze(string text)
        {
            Dictionary<string, int> Words = new Dictionary<string, int>();

            string t = Number(text);
            t = Link(t);

            string[] words = t.Split(' ');

            foreach (string s in words)
            {
                if (Words.ContainsKey(s)) Words[s]++;
                else Words.Add(s, 1);
            }

            return Words;
        }

        static string Link(string text)
        {
            string pattern = @"(\swww\..*\..*\s)";
            Regex regex = new Regex(pattern);
            return regex.Replace(text, " <<Link>> ");
        }

        static string Number(string text)
        {
            string pattern = @"([0-9][0-9][0-9][0-9]+)";
            Regex regex = new Regex(pattern);
            return regex.Replace(text, "<<Number>>");
        }
    }
}