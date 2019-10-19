using DDAPandDAPsolver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAPandDAPsolver
{
    class AdditionalFunctions
    {
        public static List<int> CalculateLinksCapacities(NetworkModel network, SolutionModel solution)
        {
            var linkCapacities = FillTableWithZeros(network.CountOfLinks);

            foreach (var demand in network.Demands)
            {
                foreach (var path in demand.Paths)
                {
                    foreach (var edge in path.Edges)
                    {
                        //Here we get for ex. 1 5 8 -> that means that we add capacity for edge 1 5 and 8      
                        solution.XesDictionary.TryGetValue(new PModel(demand.DemandId, path.PathId), out int valueOfX);
                        linkCapacities[edge - 1] += valueOfX;
                    }
                }
            }

            return linkCapacities.Select((x, index) => Decimal.ToInt32(Math.Ceiling((decimal)x / (decimal)network.Links[index].NbOfLambdasInFibre))).ToList();
        }

        public static List<int> FillTableWithZeros(int size)
        {
            var temporaryList = new List<int>();

            for (int i = 0; i < size; i++)
            {
                temporaryList.Add(0);
            }

            return temporaryList;
        }

        public static List<int> FillListWithIndexes(int count)
        {
            List<int> combination = new List<int>();
            for (int i = 0; i < count; i++)
            {
                combination.Add(i);
            }
            return combination;
        }

        public static int GetBinaryCoefficient(int N, int K)
        {
            int r = 1;
            int d;
            if (K > N) return 0;
            for (d = 1; d <= K; d++)
            {
                r *= N--;
                r /= d;
            }
            return r;
        }

        public static List<List<T>> GetPermutations<T>(
                      List<List<T>> listOfLists)
        {
            var x = listOfLists.Skip(1)
                .Aggregate(listOfLists.First()
                        .Select(c => new List<T>() { c }),
                    (previous, next) => previous
                        .SelectMany(p => next.Select(d => new List<T>(p) { d }))).Distinct().ToList();
            return x;
        }

    }

}
