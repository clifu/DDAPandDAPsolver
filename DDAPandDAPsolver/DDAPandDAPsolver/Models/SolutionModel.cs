using DDAPandDAPsolver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAPandDAPsolver
{
    public class SolutionModel
    {
        private double networkCost;
        private Dictionary<PModel, int> mapOfValues;

        public double NetworkCost
        {
            get => networkCost;
            set => networkCost = value;
        }

        private List<int> linkCapacities = new List<int>();

        public List<int> LinkCapacities
        {
            get => linkCapacities;
            set => linkCapacities = value;
        }

        //x(d,p)=a PModel = (d,p), a = value
        private Dictionary<PModel, int> xesDictionary = new Dictionary<PModel, int>();

        private int capacityExceededLinksNumber;

        public int CapacityExceededLinksNumber
        {
            get => capacityExceededLinksNumber;
            set => capacityExceededLinksNumber = value;
        }

        
        public Dictionary<PModel, int> XesDictionary
        {
            get => xesDictionary;
            set => xesDictionary = value;
        }

        public Dictionary<PModel, int> MapOfValues
        {
            get => mapOfValues;
            set => mapOfValues = value;
        }


        public SolutionModel(Dictionary<PModel, int> xesDictionary)
        {
            this.xesDictionary = xesDictionary;
        }

        public Dictionary<PModel, int> GetGene(int geneID)
        {
            Dictionary<PModel, int> gene = new Dictionary<PModel, int>();

            foreach(var entry in mapOfValues)
            {
                if(Object.Equals(entry.Key.DemandId, geneID))
                {
                    gene.Add(entry.Key, entry.Value);
                }
            }
            return gene;
        }

        public int GetNumberOfGenes()
        {
            var uniqueGenes = mapOfValues.Values.Distinct().ToList();

            return uniqueGenes.Count;
        }


    }
}
