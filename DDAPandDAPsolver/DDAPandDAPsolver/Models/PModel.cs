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

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            PModel p = obj as PModel;
            if (p == null)
                return false;
            
            return (demandId == p.demandId) && (pathId == p.pathId);
        }

        public override int GetHashCode()
        {
            return GetHashCodeInternal(demandId.GetHashCode(), pathId.GetHashCode());
        }

        private static int GetHashCodeInternal(int key1, int key2)
        {
            unchecked
            {
                //Seed
                var num = 0x7e53a269;

                //Key 1
                num = (-1521134295 * num) + key1;
                num += (num << 10);
                num ^= (num >> 6);

                //Key 2
                num = ((-1521134295 * num) + key2);
                num += (num << 10);
                num ^= (num >> 6);

                return num;
            }
        }
    }
}
