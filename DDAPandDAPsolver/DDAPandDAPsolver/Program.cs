using DDAPandDAPsolver.Models;
using DDAPandDAPsolver.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAPandDAPsolver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Czas na zabawe!");

            FileReader fileReader = new FileReader(new NetworkModel());
            var BruteForce = new BruteForce(fileReader.ReadFile("net4"));
            
            var x = BruteForce.DAP(BruteForce.PrepareSolutionsWithLinkCapacities());
            Console.WriteLine($"{x.NetworkCost}");
            Console.ReadKey();
        }
    }
}