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

        //constructor with filename so bayes cannot only be used from KFCBayes
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
                //seperate instances into spam and ham
                if (i.Result == Result.ham) _Ham.Add(i);
                else if (i.Result == Result.spam) _Spam.Add(i);
            }

            foreach(Instance i in instances)
            {
                Dictionary<string, int> words = i.Words;

                foreach(KeyValuePair<string, int> kvp in words)
                {
                    //count how often a word in general appears in all instances
                    if (_Words.ContainsKey(kvp.Key)) _Words[kvp.Key] += kvp.Value;
                    else _Words.Add(kvp.Key, kvp.Value);

                    //count how often words appear in messages classified as ham
                    if(i.Result == Result.ham)
                    {
                        if (_WordsHam.ContainsKey(kvp.Key)) _WordsHam[kvp.Key] += 1;
                        else _WordsHam.Add(kvp.Key, 1);

                        //add to spam with 0 to prevent KeyNotFoundException
                        if(!_WordsSpam.ContainsKey(kvp.Key)) _WordsSpam.Add(kvp.Key, 0);
                    }
                    //count how often words appear in messages classified as spam
                    else if (i.Result == Result.spam)
                    {
                        if (_WordsSpam.ContainsKey(kvp.Key)) _WordsSpam[kvp.Key] += 1;
                        else _WordsSpam.Add(kvp.Key, 1);

                        //add to ham with 0 to prevent KeyNotFoundException
                        if (!_WordsHam.ContainsKey(kvp.Key)) _WordsHam.Add(kvp.Key, 0);
                    }
                }
            }

            //calculate genereal likeleyhood for spam and ham
            _pHam = (double)_Ham.Count / (double)instances.Count;
            _pSpam = (double)_Spam.Count / (double)instances.Count;
        }

        //constructor to be used with existing list of instances
        public Bayes(List<Instance> list)
        {
            instances = list;

            foreach (Instance i in instances)
            {
                //seperate instances into spam and ham
                if (i.Result == Result.ham) _Ham.Add(i);
                else if (i.Result == Result.spam) _Spam.Add(i);

                Dictionary<string, int> words = i.Words;

                foreach (KeyValuePair<string, int> kvp in words)
                {
                    //count how often a word in general appears in all instances
                    if (_Words.ContainsKey(kvp.Key)) _Words[kvp.Key] += kvp.Value;
                    else _Words.Add(kvp.Key, kvp.Value);

                    //count how often words appear in messages classified as ham
                    if (i.Result == Result.ham)
                    {
                        if (_WordsHam.ContainsKey(kvp.Key)) _WordsHam[kvp.Key] += 1;
                        else _WordsHam.Add(kvp.Key, 1);

                        //add to spam with 0 to prevent KeyNotFoundException
                        if (!_WordsSpam.ContainsKey(kvp.Key)) _WordsSpam.Add(kvp.Key, 0);
                    }
                    //count how often words appear in messages classified as spam
                    else if (i.Result == Result.spam)
                    {
                        if (_WordsSpam.ContainsKey(kvp.Key)) _WordsSpam[kvp.Key] += 1;
                        else _WordsSpam.Add(kvp.Key, 1);

                        //add to ham with 0 to prevent KeyNotFoundException
                        if (!_WordsHam.ContainsKey(kvp.Key)) _WordsHam.Add(kvp.Key, 0);
                    }
                }
            }

            //calculate genereal likeleyhood for spam and ham
            _pHam = (double)_Ham.Count / (double)instances.Count;
            _pSpam = (double)_Spam.Count / (double)instances.Count;
        }

        //classify given instance to ham or spam
        public Result Classify(Instance i)
        {
            Dictionary<string, int> words = i.Words;
            words = words.Where(v => _Words.ContainsKey(v.Key)).ToDictionary(k => k.Key, k => k.Value);
            double pSpam = Spam(words);
            double pHam = Ham(words);
            return pHam > pSpam ? Result.ham : Result.spam;
        }

        //calculates propability for result being spam for instance containing words given in dictionary
        public double Spam(Dictionary<string, int> words)
        {
            //P(x|c)*P(c)
            double pSpam = PSpam(words);
            //P(x|c)*P(c)/P(x)
            //             propability that an instance contains these words
            return pSpam / (pSpam + PHam(words));
        }

        //calculates propability for result being ham for instance containing words given in dictionary
        public double Ham(Dictionary<string, int> words)
        {
            //P(x|c)*P(c)
            double pHam = PHam(words);
            //P(x|c)*P(c)/P(x)
            //             propability that an instance contains these words
            return pHam / (pHam + PSpam(words));
        }

        //P(x|c)*P(c)
        public double PSpam(Dictionary<string, int> words)
        {
            //P(c)
            double pSpam = _pSpam;
            int sum = 0;
            foreach(KeyValuePair<string, int> kvp in words)
            {
                //sum of all words contained in instance
                sum += kvp.Value;
                //factorial of how often current word is contained
                double fact = (double)Factorial(kvp.Value);
                //propability for current word occuring in spam instance
                double p = (double)_WordsSpam[kvp.Key] / (double)_Spam.Count;
                //propability for current word occuring in spam instance raised by how often word appeared in current instance divided by factorial
                double x = (Math.Pow(p, kvp.Value) / fact);
                
                //product of all results
                pSpam = pSpam * x;
            }
            //multiply with factorial of number of words contained in instance
            pSpam *= (double)Factorial(sum);
            return pSpam;
        }

        //P(x|c)*P(c)
        public double PHam(Dictionary<string, int> words)
        {
            //P(c)
            double pHam = _pHam;
            int sum = 0;
            foreach (KeyValuePair<string, int> kvp in words)
            {
                //sum of all words contained in instance
                sum += kvp.Value;
                //factorial of how often current word is contained
                double fact = (double)Factorial(kvp.Value);
                //propability for current word occuring in spam instance
                double p = (double)_WordsHam[kvp.Key] / (double)_Ham.Count;
                //propability for current word occuring in spam instance raised by how often word appeared in current instance divided by factorial
                double x = (Math.Pow(p, kvp.Value) / fact);

                //product of all results
                pHam = pHam * x;
            }
            //multiply with factorial of number of words contained in instance
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
