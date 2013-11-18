using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS497
{
    static class Okapi
    {
        public static double ComputeWeight(Dictionary<string,Corpus> corpus, string professorName, string query, int documentTermFrequency, double queryTermWeight)
        {
            int docLength = corpus[professorName].document.Length;
            int avgLength = 0;
            int documentsWithTerm = 0;
            int totalDocuments = corpus.Count;

            foreach(var value in corpus.Values)
            {
                avgLength += value.document.Length;
                if (value.document.Contains(query))
                {
                    documentsWithTerm++;
                }
            }

            avgLength = avgLength / corpus.Count;
            double weight = (documentTermFrequency / (documentTermFrequency + 0.5 + (1.5 * docLength / avgLength))) * Math.Log10(((double)totalDocuments - ((double)documentsWithTerm + 0.5)) / ((double)documentsWithTerm + 0.5)) * ((8 + queryTermWeight) / (7 + queryTermWeight));
            return Math.Abs(weight);
        }
    }
}
