using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAPandDAPsolver
{
    class FileWriter
    {

        public void WriteToFile(SolutionModel model, string fileName)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(fileName+".txt"))
            {
                        
                file.WriteLine($"Network cost :== {model.NetworkCost}");
                file.WriteLine($"Overload: {model.CapacityExceededLinksNumber}");
                        file.WriteLine("Links");

                for (int i = 0;  i < model.LinkCapacities.Count; i++)
                {
                    file.WriteLine($"{i + 1} -> {model.LinkCapacities.ElementAt(i)}");
                }
                var demandId = model.XesDictionary.ElementAt(0).Key.DemandId;
                file.WriteLine("");
                file.WriteLine("[Demand] {pathId} - > value;");
                file.Write($"[{demandId}]");

                foreach (var item in model.XesDictionary)
                {
                    if (item.Key.DemandId != demandId)
                    {
                        file.WriteLine("");
                        file.Write($"[{item.Key.DemandId}]");
                        demandId = item.Key.DemandId;
                    }
                    file.Write($"{item.Key.PathId} -> {item.Value};");
                }


            }
        }
    }
}
