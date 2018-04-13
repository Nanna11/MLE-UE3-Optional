using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaiveBayes
{
    class Instance
    {
        string text;
        Result result;
        Dictionary<string, int> Attributes;

        //represents an spam or ham message including results and the attributes defined in the analyzer
        public Instance(string Text, Result Result)
        {
            text = Text;
            result = Result;
            Attributes = Analyzer.Analyze(text);
        }

        public string Text
        {
            get
            {
                return text;
            }
        }

        public Result Result
        {
            get
            {
                return result;
            }
        }

        public Dictionary<string, int> Words
        {
            get
            {
                return Attributes;
            }
        }

        //returns the number of attributes
        public int AttributeCount
        {
            get
            {
                return Attributes.Count;
            }
        }
    }
}