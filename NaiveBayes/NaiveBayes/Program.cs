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
            KFCBayes kb = new KFCBayes(10, "SMSSpamCollection");
            Console.ReadKey();
        }
    }
}
