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

        public int StartNode
        {
            get => startNode;
            set => startNode = value;
        }

        private int endNode;

        public int EndNode
        {
            get => endNode;
            set => endNode = value;
        }

        private int nbOfFibrePairs;

        public int NbOfFibrePairs
        {
            get => nbOfFibrePairs;
            set => nbOfFibrePairs = value;
        }

        private int fibrePairCost;

        public int FibrePairCost
        {
            get => fibrePairCost;
            set => fibrePairCost = value;
        }

        private int nbOfLambdasInFibre;

        public int NbOfLambdasInFibre
        {
            get => nbOfLambdasInFibre;
            set => nbOfLambdasInFibre = value;
        }


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
