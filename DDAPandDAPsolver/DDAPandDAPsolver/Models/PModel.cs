using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAPandDAPsolver.Models
{

    //P(demand,path)
    public class PModel
    {
        private int demandId;
        private int pathId;

        public int DemandId
        {
            get => demandId;
            set => demandId = value;
        }

        public int PathId
        {
            get => pathId;
            set => pathId = value;
        }

        public PModel(int demandId, int pathId)
        {
            this.demandId = demandId;
            this.pathId = pathId;
        }
    }
}
