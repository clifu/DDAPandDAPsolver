using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DDAPandDAPsolver.Models
{
    class DemandModel
    {
        private int demandId;

        public int DemandId
        {
            get => demandId;
            set => demandId = value;
        }

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

        private int demandVolume;

        public int DemandVolume
        {
            get => demandVolume;
            set => demandVolume = value;
        }

        private int numberOfPaths;

        public int NumberOfPaths
        {
            get => numberOfPaths;
            set => numberOfPaths = value;
        }

        private List<PathModel> paths = new List<PathModel>();

        public List<PathModel> Paths
        {
            get => paths;
            set => paths = value;
        }


        public DemandModel(List<string> definition, int demandId)
        {
            var x = definition[0].Split(' ').ToList();
            definition.RemoveAt(0);
            startNode = int.Parse(x[0]);
            endNode = int.Parse(x[1]);
            demandVolume = int.Parse(x[2]);

            this.demandId = demandId;

            numberOfPaths = int.Parse(definition[0]);
            definition.RemoveAt(0);

            #region CreatingPaths
            var pathId = 0;

            foreach (var lineDefiningPath in definition)
            {
                paths.Add(new PathModel(lineDefiningPath, demandId, ++pathId));
            }

            #endregion

        }

    }
}
