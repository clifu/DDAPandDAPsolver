using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAPandDAPsolver.Models
{
    class LinkModel
    {
        private int startNode;
        private int endNode;
        private int nbOfFibrePairs;
        private int fibrePairCost;
        private int nbOfLambdasInFibre;

        public LinkModel(string definingLine)
        {
            List<string> values = definingLine.Split(' ').ToList();
            startNode = int.Parse(values[0]);
            endNode = int.Parse(values[1]);
            nbOfFibrePairs = int.Parse(values[2]);
            fibrePairCost = int.Parse(values[3]);
            nbOfLambdasInFibre = int.Parse(values[4]);
        }
        
    }
}
