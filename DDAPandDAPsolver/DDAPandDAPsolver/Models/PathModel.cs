using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAPandDAPsolver.Models
{
    class PathModel
    {
        private int demandId;

        public int DemandId
        {
            get => demandId;
            set => demandId = value;
        }

        private int pathId;

        public int PathId
        {
            get => pathId;
            set => pathId = value;
        }

        private List<int> edges = new List<int>();

        public List<int> Edges
        {
            get => edges;
            set => edges = value;
        }

        public PathModel(string definingLine, int demandId, int pathId)
        {
            this.demandId = demandId;
            this.pathId = pathId;

            definingLine.Split(' ').Skip(1).ToList().ForEach(x =>
                    { if (x != "")
                            edges.Add(int.Parse(x));
                    });
        }

    }
}
