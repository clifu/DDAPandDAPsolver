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
            //var BruteForce = new BruteForce(fileReader.ReadFile("net4"));
            
            //var x = BruteForce.DAP(BruteForce.PrepareSolutionsWithLinkCapacities());

            int maxMutationNumber = 50;
            int maxNumberOfContinuousNonBetterSolutions = 20;
            int maxTime = 50;
            NetworkModel network = fileReader.ReadFile("net4");
            int population = 50;
            int maxNumberOfGenerations = 6;
            float pCross = 0.6f;
            float pMutate = 0.4f;
            int seed = 40;
            float percentOfBestChromosomes = 70;

            var evolutionary = new Evolutionary(pCross, pMutate, maxTime, population, percentOfBestChromosomes, maxNumberOfGenerations, maxMutationNumber, maxNumberOfContinuousNonBetterSolutions, seed, network);
            evolutionary.ComputeDDAP();
            //Console.WriteLine($"{x.NetworkCost}");
            Console.WriteLine($"Zakonczono");
            Console.ReadKey();
        }
    }
}