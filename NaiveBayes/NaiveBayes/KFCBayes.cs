using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NaiveBayes
{
    class KFCBayes
    {
        List<Instance> instances = new List<Instance>();

        public KFCBayes(int k, string filename)
        {
            string deploypath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filepath = Path.Combine(deploypath, filename);
            FileStream file = new FileStream(filepath, FileMode.Open);
            StreamReader sr = new StreamReader(file);

            //double factorOfLessSpamThanHam = 6.46f;
            //bool isSecondTime = false;

            int helper = 0;
            //todo: read out of config file
            int amountOfSpamInDataSet = 747;

            string f;
            while ((f = sr.ReadLine()) != null)
            {
                string[] result = f.Split("\t".ToArray<char>());
                Instance i = new Instance(result[1], (Result)System.Enum.Parse(typeof(Result), result[0]));
                if (result[0] == "ham")
                {
                    helper++;
                }
                if (!(result[0] == "ham" && helper > amountOfSpamInDataSet))
                {
                    instances.Add(i);
                }
                // //adds every instance of spam approximatly 6.5 times to have about the same amount of ham and spam messages
                // //pretty bad because data is manipulated (6.5 times higher probability for words in spam messages to be in spam than in ham messages)
                //if(result[0] == "spam")
                //{
                //    for (int h = 1; h < factorOfLessSpamThanHam; h++)
                //    {
                //        if(h == 5 && isSecondTime)
                //        {
                //            break;
                //        }
                //        instances.Add(i);
                //    }
                //    if (!isSecondTime) isSecondTime = true;
                //    else isSecondTime = false;
                //}
            }

            int[,] Confusion = new int[3, 3];
            //get kfc packages
            List<List<Instance>> packages = new List<List<Instance>>();
            for (int i = 0; i < k; i++)
            {
                packages.Add(new List<Instance>());
            }

            //divide instances into packages
            for (int i = 0; i < instances.Count; i++)
            {
                packages[i % k].Add(instances[i]);
            }

            //test for every package
            for (int i = 0; i < k; i++)
            {
                TestPackage(i, packages, Confusion);
            }

            //print confusion matrix
            double sum = 0;
            double correct = 0;

            Console.WriteLine("\nConfusion Matrix:");
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    BeautifulConfusionMatrix.AddBlankSpaces(Confusion, Confusion[i, j], j);
                    Console.Write("{0} ", Confusion[i, j]);
                    //add to correct count if actual and calculated result were the same
                    if (i == j) correct += Confusion[i, j];
                    sum += Confusion[i, j];
                }
                Console.WriteLine("");
            }

            Console.WriteLine("");
            //calculate accuracy
            double acc = correct / sum;
            Console.WriteLine("Accuracy: {0}", acc);
        }

        void TestPackage(int i, List<List<Instance>> packages, int[,] Confusion)
        {
            //create lists for learn and test data
            List<Instance> ToTest = packages[i];
            List<Instance> ToLearn = new List<Instance>();
            for (int j = 0; j < packages.Count; j++)
            {
                if (j == i) continue;
                ToLearn = ToLearn.Concat<Instance>(packages[j]).ToList<Instance>();
            }

            Bayes b = new Bayes(ToLearn);

            foreach (Instance instance in ToTest)
            {
                Result? res = b.Classify(instance);
                Confusion[(int)instance.Result, (int)res]++;
            }
        }
    }
}
