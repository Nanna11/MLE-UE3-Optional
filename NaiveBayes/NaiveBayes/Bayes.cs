using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NaiveBayes
{
    class Bayes
    {
        List<Instance> instances = new List<Instance>();
        List<Instance> _Spam = new List<Instance>();
        List<Instance> _Ham = new List<Instance>();
        Dictionary<string, int> _WordsSpam = new Dictionary<string, int>();
        Dictionary<string, int> _WordsHam = new Dictionary<string, int>();
        Dictionary<string, int> _Words = new Dictionary<string, int>();
        double _pSpam;
        double _pHam;


        public Bayes(string filename)
        {
            //read all instances from a file
            string deploypath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filepath = Path.Combine(deploypath, filename);
            FileStream file = new FileStream(filepath, FileMode.Open);
            StreamReader sr = new StreamReader(file);

            string f;
            while ((f = sr.ReadLine()) != null)
            {
                string[] result = f.Split("\t".ToArray<char>());
                Instance i = new Instance(result[1], (Result)System.Enum.Parse(typeof(Result), result[0]));
                instances.Add(i);
                if (i.Result == Result.ham) _Ham.Add(i);
                else if (i.Result == Result.spam) _Spam.Add(i);
            }

            foreach(Instance i in instances)
            {
                Dictionary<string, int> words = i.Words;

                foreach(KeyValuePair<string, int> kvp in words)
                {
                    if (_Words.ContainsKey(kvp.Key)) _Words[kvp.Key] += kvp.Value;
                    else _Words.Add(kvp.Key, kvp.Value);

                    if(i.Result == Result.ham)
                    {
                        if (_WordsHam.ContainsKey(kvp.Key)) _WordsHam[kvp.Key] += 1;
                        else _WordsHam.Add(kvp.Key, 1);

                        if(!_WordsSpam.ContainsKey(kvp.Key)) _WordsSpam.Add(kvp.Key, 0);
                    }
                    else if (i.Result == Result.spam)
                    {
                        if (_WordsSpam.ContainsKey(kvp.Key)) _WordsSpam[kvp.Key] += 1;
                        else _WordsSpam.Add(kvp.Key, 1);

                        if (!_WordsHam.ContainsKey(kvp.Key)) _WordsHam.Add(kvp.Key, 0);
                    }
                }
            }

            _pHam = (double)_Ham.Count / (double)instances.Count;
            _pSpam = (double)_Spam.Count / (double)instances.Count;
        }

        public Bayes(List<Instance> list)
        {
            instances = list;

            foreach (Instance i in instances)
            {
                if (i.Result == Result.ham) _Ham.Add(i);
                else if (i.Result == Result.spam) _Spam.Add(i);

                Dictionary<string, int> words = i.Words;

                foreach (KeyValuePair<string, int> kvp in words)
                {
                    if (_Words.ContainsKey(kvp.Key)) _Words[kvp.Key] += kvp.Value;
                    else _Words.Add(kvp.Key, kvp.Value);

                    if (i.Result == Result.ham)
                    {
                        if (_WordsHam.ContainsKey(kvp.Key)) _WordsHam[kvp.Key] += 1;
                        else _WordsHam.Add(kvp.Key, 1);
                        
                        if (!_WordsSpam.ContainsKey(kvp.Key)) _WordsSpam.Add(kvp.Key, 0);
                    }
                    else if (i.Result == Result.spam)
                    {
                        if (_WordsSpam.ContainsKey(kvp.Key)) _WordsSpam[kvp.Key] += 1;
                        else _WordsSpam.Add(kvp.Key, 1);

                        if (!_WordsHam.ContainsKey(kvp.Key)) _WordsHam.Add(kvp.Key, 0);
                    }
                }
            }

            _pHam = (double)_Ham.Count / (double)instances.Count;
            _pSpam = (double)_Spam.Count / (double)instances.Count;
        }

        public Result Classify(Instance i)
        {
            Dictionary<string, int> words = i.Words;
            words = words.Where(v => _Words.ContainsKey(v.Key)).ToDictionary(k => k.Key, k => k.Value);
            double pSpam = Spam(words);
            double pHam = Ham(words);
            return pHam > pSpam ? Result.ham : Result.spam;
        }

        public double Spam(Dictionary<string, int> words)
        {
            double pSpam = PSpam(words);
            return pSpam / (pSpam + PHam(words));
        }

        public double Ham(Dictionary<string, int> words)
        {
            double pHam = PHam(words);
            return pHam / (pHam + PSpam(words));
        }

        public double PSpam(Dictionary<string, int> words)
        {
            double pSpam = _pSpam;
            int sum = 0;
            foreach(KeyValuePair<string, int> kvp in words)
            {
                sum += kvp.Value;
                double fact = (double)Factorial(kvp.Value);
                double p = (double)_WordsSpam[kvp.Key] / (double)_Spam.Count;
                double x = (Math.Pow(p, kvp.Value) / fact);
                //if (x != 0)
                pSpam = pSpam * x;
            }
            pSpam *= (double)Factorial(sum);
            return pSpam;
        }

        public double PHam(Dictionary<string, int> words)
        {
            double pHam = _pHam;
            int sum = 0;
            foreach (KeyValuePair<string, int> kvp in words)
            {
                sum += kvp.Value;
                double fact = (double)Factorial(kvp.Value);
                double p = (double)_WordsHam[kvp.Key] / (double)_Ham.Count;
                double x = (Math.Pow(p, kvp.Value) / fact);
                //if (x != 0)
                pHam = pHam * x;
            }
            pHam *= (double)Factorial(sum);
            return pHam;
        }


        long Factorial(int i)
        {
            if (i == 1 || i == 0) return 1;
            else return i * Factorial(i - 1);
        }
    }
}
