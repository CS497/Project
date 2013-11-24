using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS497
{
    class QueryTester
    {
        public static Dictionary<string, List<string>> Queries = new Dictionary<string, List<string>>()
        {
            {
                "bioinformatics", new List<string>()
                {
                    "Ananth Grama", 
                    "Daisuke Kihara",
                    "Alex Pothen",
                    "Luo Si",
                    "Robert D. Skeel",
                    "Wojciech Szpankowski"
                } 
            },
            {
                "data mining", new List<string>()
                {
                    "Walid G. Aref",
                    "Elisa Bertino",
                    "Bharat Bhargava",
                    "Christopher W. Clifton",
                    "Ahmed K. Elmagarmid",
                    "Susanne E. Hambrusch",
                    "Jennifer Neville",
                    "Sunil Prabhakar",
                    "Luo Si"
                }
            },
            {
                "programming languages", new List<string>()
                {
                    "Patrick Thomas Eugster",
                    "Antony Hosking",
                    "Suresh Jagannathan",
                    "Zhiyuan Li",
                    "Jan Vitek",
                    "Xiangyu Zhang"
                }
            }
        };

        public List<string> PredictedTrue;
        public List<string> PredictedFalse;
        public List<string> ActualTrue;
        public List<string> ActualFalse;
        public int a, b, c, d;

        public QueryTester(List<string> predictedTrue, List<string> actualTrue)
        {
            PredictedTrue = predictedTrue;
            PredictedFalse = Globals.Professors.Except(predictedTrue).ToList();
            ActualTrue = actualTrue;
            ActualFalse = Globals.Professors.Except(predictedTrue).ToList();
            a = PredictedTrue.Count(x => ActualTrue.Contains(x));
            b = PredictedTrue.Count - a;
            c = PredictedFalse.Count(x => ActualTrue.Contains(x));
            d = PredictedFalse.Count - c;
        }
        public double Precision()
        {
            return (float)(PredictedTrue.Count(x => ActualTrue.Contains(x))) / PredictedTrue.Count;
        }

        public double Recall()
        {
            return (float)(PredictedTrue.Count(x => ActualTrue.Contains(x))) / ActualTrue.Count;
        }
    }
}
