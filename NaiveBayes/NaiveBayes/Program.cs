using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaiveBayes
{
    class Program
    {
        static void Main(string[] args)
        {
            //instance that tests naive bayes with instances in given file
            KFCBayes kb = new KFCBayes(10, "SMSSpamCollection");
            Console.ReadKey();
        }
    }
}
