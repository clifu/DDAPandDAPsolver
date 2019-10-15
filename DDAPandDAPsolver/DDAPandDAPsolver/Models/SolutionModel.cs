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
        private float networkCost;

        public float NetworkCost
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
        private Dictionary<PModel, int> xesList = new Dictionary<PModel, int>();

        public Dictionary<PModel, int> XesList
        {
            get => xesList;
            set => xesList = value;
        }


        public SolutionModel(Dictionary<PModel, int> xesList)
        {
            this.xesList = xesList;
        }


    }
}
