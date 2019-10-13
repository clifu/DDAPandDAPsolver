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
        public static List<int> CalculateLinksCapacities(NetworkModel network)
        {
            var linkCapacities = FillTableWithZeros(network.CountOfLinks);

            foreach (var demand in network.Demands)
            {
                foreach (var path in demand.Paths)
                {
                    foreach (var edge in path.Edges)
                    {
                        //Here we get for ex. 1 5 8 -> that means that we add capacity for edge 1 5 and 8
                        //TODO: needed value from solution
                        linkCapacities[edge - 1] += 100;
                    }
                }
            }
        
            return linkCapacities.Select((x, index) => Decimal.ToInt32(Math.Ceiling((decimal)x / (decimal)network.Links[index].NbOfLambdasInFibre))).ToList();
        }

        public static List<int> FillTableWithZeros(int size)
        {
            var temporaryList = new List<int>();

            for(int i=0; i < size; i++)
            {
                temporaryList.Add(0);
            }

            return temporaryList;
        }

        public static ulong GetBinaryCoefficient(ulong N, ulong K)
        {
            ulong r = 1;
            ulong d;
                if (K > N) return 0;
                for (d = 1; d <= K; d++)
                {
                    r *= N--;
                    r /= d;
                }
            return r;
        }

    }
}
