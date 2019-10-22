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

        public Dictionary<PModel, int> XesDictionary
        {
            get => xesDictionary;
            set => xesDictionary = value;
        }

        private int capacityExceededLinksNumber;

        public int CapacityExceededLinksNumber
        {
            get => capacityExceededLinksNumber;
            set => capacityExceededLinksNumber = value;
        }


        public SolutionModel(Dictionary<PModel, int> xesDictionary)
        {
            this.xesDictionary = xesDictionary;
        }


    }
}
