using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAPandDAPsolver.Models
{
    class NetworkModel
    {

        private List<DemandModel> demands;

        public List<DemandModel> Demands
        {
            get => demands;
            set => demands = value;
        }

        private List<LinkModel> links;

        public List<LinkModel> Links
        {
            get => links;
            set => links = value;
        }

        private int countOfDemands;

        public int CountOfDemands
        {
            get => countOfDemands;
            set => countOfDemands = value;
        }

        private int countOfLinks;

        public int CountOfLinks
        {
            get => countOfLinks;
            set => countOfLinks = value;
        }

        public NetworkModel()
        {

        }
    }
}
